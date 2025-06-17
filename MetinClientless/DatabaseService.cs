using System.Reflection;
using Npgsql;

namespace MetinClientless;

public static class DatabaseService
{
    public static readonly NpgsqlDataSource _dataSource;

    static DatabaseService()
    {
        _dataSource = NpgsqlDataSource.Create(Configuration.PostgresConnectionString);
    }

    public static async Task ExecuteQueryAsync(string query, Dictionary<string, object> parameters = null)
    {
        await using var command = _dataSource.CreateCommand(query);
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }
        }

        await command.ExecuteNonQueryAsync();
    }

    public static async Task<T?> ExecuteQueryReturningAsync<T>(string query, Dictionary<string, object> parameters = null) where T : struct
    {
        await using var command = _dataSource.CreateCommand(query);
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }
        }

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync() && !reader.IsDBNull(0))
        {
            return (T)reader.GetValue(0);
        }

        return null;
    }

    public static async Task<(T1?, T2?)> ExecuteQueryReturningTupleAsync<T1, T2>(string query, Dictionary<string, object> parameters = null) 
        where T1 : struct 
        where T2 : struct
    {
        await using var command = _dataSource.CreateCommand(query);
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }
        }

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync() && !reader.IsDBNull(0) && !reader.IsDBNull(1))
        {
            return ((T1)reader.GetValue(0), (T2)reader.GetValue(1));
        }

        return (null, null);
    }

    public static async Task<T> ExecuteQueryReturningObjectAsync<T>(string query, Dictionary<string, object> parameters = null) where T : class, new()
    {
        await using var command = _dataSource.CreateCommand(query);
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }
        }

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var item = new T();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var property = typeof(T).GetProperty(reader.GetName(i), 
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            
                if (property != null && !reader.IsDBNull(i))
                {
                    var value = reader.GetValue(i);
                    property.SetValue(item, Convert.ChangeType(value, property.PropertyType));
                }
            }
            return item;
        }

        return null;
    }
}