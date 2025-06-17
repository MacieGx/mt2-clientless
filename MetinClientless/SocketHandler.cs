using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using MetinClientless.Handlers;
using MetinClientless.Libs;
using MetinClientless.Packets;
using Starksoft.Aspen.Proxy;


namespace MetinClientless;

public class SocketHandler
{
    private TcpClient _client;
    private NetworkStream _stream;
    private readonly PacketHandlerRegistry _registry;
    private byte[] _undecryptedBuffer = [];
    private byte[] _buffer = [];
    private byte[] initialBuffer = [];
    
    private readonly Dictionary<EServerToClient, List<Func<byte[], bool>>> _packetCallbacks = new();

    private long lastPacketReceivedUpdatedAtMs = 0;


    private static SocketHandler _instance;
    public static SocketHandler GetInstance(PacketHandlerRegistry? registry)
    {
        if (_instance == null)
        {
            _instance = new SocketHandler(registry ?? new PacketHandlerRegistry());
        }

        return _instance;
    }

    private SocketHandler(PacketHandlerRegistry registry)
    {
        _registry = registry;
    }

    public async Task ConnectAsync(string host, int port)
    {
        if (_client != null)
        {
            _client.Dispose();
        }
        
        if (Configuration.Proxy.Enabled)
        {
            _client = GetProxiedTcpClient(Configuration.Proxy.Host, Configuration.Proxy.Port, Configuration.Proxy.Username, Configuration.Proxy.Password, host, port);
            Console.WriteLine("Connected to server via proxy");
        }
        else
        {
            Console.WriteLine("Cannot connect to server without proxy to prevent ban");
            Environment.Exit(0);
        }
        
        
        _stream = _client.GetStream();
        Console.WriteLine("Connected to server");
        
        _buffer = [];
        await ReceiveDataAsync();
    }

    private TcpClient GetProxiedTcpClient(string proxyHost, int proxyPort, string proxyUsername, string proxyPassword, string destinationHost, int destinationPort)
    {
        
        var proxyClient = new Socks5ProxyClient(proxyHost, proxyPort, proxyUsername, proxyPassword);
        Console.WriteLine("Connecting to proxy");
        return proxyClient.CreateConnection(destinationHost, destinationPort);
    }
    
        public void SendGatekeeperRequest()
    {
        try
        {
            var tcpClient = GetProxiedTcpClient(Configuration.Proxy.Host, Configuration.Proxy.Port, Configuration.Proxy.Username, Configuration.Proxy.Password, Configuration.GameServer.GateKeeperIP, 80);
            using var networkStream = tcpClient.GetStream();
            
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string nonce = GenerateNonce();
            string content = "player_request_data";
            
            StringBuilder request = new StringBuilder();
            request.AppendLine("POST /gatekeeper.php HTTP/1.1");
            request.AppendLine($"Host: {Configuration.GameServer.GateKeeperIP}");
            request.AppendLine("User-Agent: Boost.Beast/347");
            request.AppendLine("Content-Type: text/plain");
            request.AppendLine($"X-Timestamp: {timestamp}");
            request.AppendLine($"X-Nonce: {nonce}");
            request.AppendLine($"Content-Length: {Encoding.ASCII.GetByteCount(content)}");
            request.AppendLine();
            request.Append(content);

            byte[] requestBytes = Encoding.ASCII.GetBytes(request.ToString());
            networkStream.Write(requestBytes, 0, requestBytes.Length);
            networkStream.Flush();

            byte[] buffer = new byte[4096];
            StringBuilder responseBuilder = new StringBuilder();
            int bytesRead;

            while ((bytesRead = networkStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                responseBuilder.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));
                if (responseBuilder.ToString().Contains("\r\n\r\n"))
                    break;
            }

            Console.WriteLine($"Gatekeeper Response: {responseBuilder}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Gatekeeper request failed: {ex.Message}");
            throw;
        }
    }
        
    private static string GenerateNonce()
    {
        Random random = new Random();
        const string chars = "ABCDEFabcdef0123456789";
        return new string(Enumerable.Repeat(chars, 16)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    
    
private async Task ReceiveDataAsync()
{
    int newPort = 0;

    while (_client.Connected)
    {
        var chunkBuffer = new byte[1024 * 32];
        var bytesRead = await _stream.ReadAsync(chunkBuffer);

        if (bytesRead != 0)
        {
            var useTeaEncryption = true;
            if (useTeaEncryption && GameState.CurrentPhase != EPhase.PHASE_HANDSHAKE)
            {
                // Combine previous undecrypted bytes with newly received data
                var totalBytes = _undecryptedBuffer.Length + bytesRead;
                var dataToDecrypt = new byte[totalBytes];

                Buffer.BlockCopy(_undecryptedBuffer, 0, dataToDecrypt, 0, _undecryptedBuffer.Length);
                Buffer.BlockCopy(chunkBuffer, 0, dataToDecrypt, _undecryptedBuffer.Length, bytesRead);

                int decryptableLength = (totalBytes / 8) * 8;
                int leftoverBytes = totalBytes - decryptableLength;

                if (decryptableLength > 0)
                {
                    byte[] decryptedChunk = TEA.Decrypt(dataToDecrypt[..decryptableLength]);
                    
                    var combinedBuffer = new byte[_buffer.Length + decryptedChunk.Length];
                    Buffer.BlockCopy(_buffer, 0, combinedBuffer, 0, _buffer.Length);
                    Buffer.BlockCopy(decryptedChunk, 0, combinedBuffer, _buffer.Length, decryptedChunk.Length);
                    _buffer = combinedBuffer;
                }

                // Keep leftover undecrypted bytes
                _undecryptedBuffer = leftoverBytes > 0
                    ? dataToDecrypt[decryptableLength..]
                    : [];
            }
            else
            {
                var combinedBuffer = new byte[_buffer.Length + bytesRead];
                Buffer.BlockCopy(_buffer, 0, combinedBuffer, 0, _buffer.Length);
                Buffer.BlockCopy(chunkBuffer, 0, combinedBuffer, _buffer.Length, bytesRead);
                _buffer = combinedBuffer;
            }
        }

        while (_buffer.Length > 0)
        {
            if (_buffer[0] == 0x00)
            {
                _buffer = _buffer[1..];
                continue;
            }

            var header = _buffer[0];

            if (!PacketInfo.PacketKnown(header))
            {
                Console.WriteLine("Unknown packet header: " + header);
                DebugRecv(_buffer);
                Environment.Exit(1);
            }

            var packetInfo = PacketInfo.GetForHeader(header);

            if (packetInfo.IsDynamicSize && _buffer.Length < 3)
                break;

            ushort packetSize = packetInfo.IsDynamicSize ? BitConverter.ToUInt16(_buffer[1..3]) : (ushort)packetInfo.Size;

            if (packetSize == 0 || packetSize > _buffer.Length)
                break;

            newPort = await ProcessData(_buffer[..packetSize]);

            if (newPort != 0)
            {
                _buffer = [];
                break;
            }

            _buffer = _buffer[packetSize..];
        }

        if (newPort != 0)
            break;
    }

    Console.WriteLine("Disconnected from server");

    if (newPort != 0)
    {
        if (initialBuffer != null)
            await SendAsync(initialBuffer);

        await ChangePort(newPort);
    }
}

    
    private void DebugRecv(byte[] data)
    {
        var header = data[0];
        
        List<EServerToClient> ignoreHeaders = new List<EServerToClient>
        {
            EServerToClient.HEADER_GC_VIEW_EQUIP,
            EServerToClient.HEADER_GC_CHARACTER_MOVE,
            EServerToClient.HEADER_GC_CHAT,
            EServerToClient.HEADER_GC_MT2009_SHOP_DATA
        };
        
        if (ignoreHeaders.Contains((EServerToClient)header))
        {
            return;
        }
        
        var headerName = Enum.GetName(typeof(EServerToClient), header) ?? "UNKNOWN_" + header;
        Console.WriteLine($"[RECV] [{headerName}] [S:{data.Length}] {BitConverter.ToString(data).Replace("-", " ")}");
    }
   
    private async Task<int> ProcessData(byte[] data)
    {
        DebugRecv(data);
        
        if (GameState.CurrentPhase == EPhase.PHASE_GAME)
        {
            if (lastPacketReceivedUpdatedAtMs + 2500 < DateTimeOffset.Now.ToUnixTimeMilliseconds())
            {
                lastPacketReceivedUpdatedAtMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                _ = UpdateLastPacketReceivedAt();
            }
        }
        
        int header = data[0];
        var response = await _registry.ProcessPacketAsync(header, data);
        
        if (Enum.IsDefined(typeof(EServerToClient), header))
        {
            var serverToClient = (EServerToClient)header;
            if (_packetCallbacks.ContainsKey(serverToClient))
            {
                var callbacks = _packetCallbacks[(EServerToClient)header]?.ToList() ?? [];
                for (int i = callbacks.Count - 1; i >= 0; i--)
                {
                    var callback = callbacks[i];
                    if (callback(data))
                    {
                        _packetCallbacks[(EServerToClient)header].RemoveAt(i);
                    }
                }
                
                if (_packetCallbacks[(EServerToClient)header]?.Count == 0)
                {
                    _packetCallbacks.Remove((EServerToClient)header);
                }
            }
        }

        if (response.NewPort != 0)
        {
            initialBuffer = response.Data;   
            return response.NewPort;
        }

        if (response.Data != null && response.Data.Length > 0)
        {
            await SendAsync(response.Data);
        }

        return 0;
    }
    
    private async Task<bool> UpdateLastPacketReceivedAt()
    {
        var statusQuery = $"UPDATE bots SET last_packet_received_at = CURRENT_TIMESTAMP WHERE id = @BotId::uuid;";
        var parameters = new Dictionary<string, object>
        {
            { "@BotId", GameState.BotId }
        };
        
        
        await DatabaseService.ExecuteQueryAsync(statusQuery, parameters);
        return true;
    }
    
    private async Task ChangePort(int port, bool cipherDispose = true)
    {
        if (GameState.CurrentPhase != EPhase.PHASE_SELECT) Cipher.Dispose();
        SequenceTable.ResetIndex();
        await ConnectAsync(Configuration.GameServer.IP, port);
    }

    public async Task SendAsync(byte[] data)
    {
        var header = data[0];
        var headerName = Enum.GetName(typeof(EClientToServer), header) ?? "UNKNOWN_" + header;
        Console.WriteLine($"[SEND][{headerName}] {BitConverter.ToString(data).Replace("-", " ")}");
        
        // var cipher = Cipher.getInstance();
        // if (cipher.isStarted)
        // {
        //     data = cipher.Encrypt(data);
        // }
        
        
        var useTeaEncryption = true;
        if (useTeaEncryption && GameState.CurrentPhase != EPhase.PHASE_HANDSHAKE)
        {
            data = TEA.Encrypt(data, null);
        }
        
        
        await _stream.WriteAsync(data, 0, data.Length);
    }
    
    public void OnPacket(EServerToClient packetType, Func<byte[], bool> callback)
    {
        if (!_packetCallbacks.ContainsKey(packetType))
        {
            _packetCallbacks[packetType] = new List<Func<byte[], bool>>();
        }
    
        _packetCallbacks[packetType].Add(callback);
    }
}