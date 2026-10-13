using System.Text.Json;

namespace SA2DGE.Engine.Serialization;

public sealed class Serializer
{
    private readonly JsonSerializerOptions _options;

    public JsonSerializerOptions Options =>
        _options;

    public Serializer(
        JsonSerializerOptions? options = null)
    {
        _options = options ?? new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };
    }

    public string Serialize<T>(
        T value)
    {
        return JsonSerializer.Serialize(
            value,
            _options);
    }

    public void Serialize<T>(
        T value,
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string? directory =
            Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(
            path,
            Serialize(value));
    }

    public byte[] SerializeToBytes<T>(
        T value)
    {
        return JsonSerializer.SerializeToUtf8Bytes(
            value,
            _options);
    }

    public void SerializeToStream<T>(
        T value,
        Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        JsonSerializer.Serialize(
            stream,
            value,
            _options);
    }

    public string Serialize(
        object value,
        Type type)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(type);

        return JsonSerializer.Serialize(
            value,
            type,
            _options);
    }

    public void Serialize(
        object value,
        Type type,
        Stream stream)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(stream);

        JsonSerializer.Serialize(
            stream,
            value,
            type,
            _options);
    }
}