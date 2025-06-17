using System.Runtime.InteropServices;

namespace MetinClientless.Libs;


using CIPHER_C_OBJECT = IntPtr;

internal class CppCipher
    {
        [DllImport("Cipher.dll", EntryPoint = "CreateTestClass")]
        public static extern CIPHER_C_OBJECT MakeCipher();

        [DllImport("Cipher.dll", EntryPoint = "DisposeTestClass", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DisposeCipher(CIPHER_C_OBJECT obj);

        [DllImport("Cipher.dll", EntryPoint = "Prepare", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Prepare(CIPHER_C_OBJECT obj, byte[] buffer, int length);

        [DllImport("Cipher.dll", EntryPoint = "Activate", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Activate(CIPHER_C_OBJECT pObject, bool bPolarity, uint uAgreedLength, byte[] c_pBuffer,
            uint uLength);

        [DllImport("Cipher.dll", EntryPoint = "Encrypt", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Encrypt(CIPHER_C_OBJECT obj, byte[] buffer, int length);

        [DllImport("Cipher.dll", EntryPoint = "Decrypt", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Decrypt(CIPHER_C_OBJECT obj, byte[] buffer, int length);
    }
    public class Cipher
    {
        public bool isStarted;
        public bool activated;
        public int agreed_length;
        public byte[] Key;
        public CIPHER_C_OBJECT obj;

        private static Cipher? instance;
        public static Cipher getInstance()
        {
            if (instance == null)
            {
                instance = new Cipher();
                instance.Prepare();
            }
            
            return instance;
        }
        
        public static void Dispose()
        {
            if (instance != null)
            {
                CppCipher.DisposeCipher(instance.obj);
                instance.activated = false;
                instance.isStarted = false;
                instance = null;
            }
        }

        private Cipher()
        {
            obj = CppCipher.MakeCipher();
            agreed_length = 0;
            activated = false;
            isStarted = false;
        }

        public bool Activated => activated;
        public byte[] GetKey()
        {
            return Key;
        }
        public int Prepare()
        {
            Key = new byte[256];

            agreed_length = CppCipher.Prepare(obj, Key, Key.Length);
            return agreed_length;
        }

        public bool Activate(bool polarity, uint agreed_length, byte[] public_key)
        {
            if (this.agreed_length == 0)
                throw new ArithmeticException();
            activated = CppCipher.Activate(obj, polarity, agreed_length, public_key, (uint)public_key.Length);
            return activated;
        }

        public byte[] Encrypt(byte[] buffer)
        {
            var length = buffer.Length;
            var ret = new byte[length];
            buffer.CopyTo(ret, 0);
            if (CppCipher.Encrypt(obj, ret, length))
                return ret;
            throw new ArithmeticException();
        }

        public byte[] Decrypt(byte[] buffer)
        {
            var length = buffer.Length;
            var ret = new byte[length];
            buffer.CopyTo(ret, 0);
            if (CppCipher.Decrypt(obj, ret, length))
                return ret;
            throw new ArithmeticException();
        }
    }