using System.Text.Json;

namespace DataAccess;

public class DatabaseReadWriter<T> : IDisposable
{
    public DatabaseReadWriter(string file)
    {
        stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
    }

    public void Dispose()
    {
        stream.Dispose();
    }

    public T? Read()
    {
        stream.Position = 0;
        using StreamReader reader = new StreamReader(stream, leaveOpen: true);
        try
        {
            return JsonSerializer.Deserialize<T>(reader.ReadToEnd());
        }
        catch (JsonException e)
        {
            return default;
        }
    }

    public void Save(T data)
    {
        stream.SetLength(0);
        using StreamWriter writer = new StreamWriter(stream, leaveOpen: true);
        writer.Write(JsonSerializer.Serialize(data));
    }

    private readonly FileStream stream;
}
