using System.Text.Json;

namespace SA2DGE.Engine.Serialization;

public sealed class Deserializer
{
    private readonly JsonSerializerOptions _options;

    public JsonSerializerOptions Options =>
        _options;

    public Deserializer(
        JsonSerializerOptions? options = null)
    {
        _options = options ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };
    }

    public T Deserialize<T>(
        string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        T? value =
            JsonSerializer.Deserialize<T>(
                json,
                _options);

        if (value is null)
        {
            throw new JsonException(
                $"Unable to deserialize JSON into '{typeof(T).Name}'.");
        }

        return value;
    }

    public T Deserialize<T>(
        ReadOnlySpan<byte> utf8Json)
    {
        if (utf8Json.IsEmpty)
        {
            throw new ArgumentException(
                "JSON data cannot be empty.",
                nameof(utf8Json));
        }

        T? value =
            JsonSerializer.Deserialize<T>(
                utf8Json,
                _options);

        if (value is null)
        {
            throw new JsonException(
                $"Unable to deserialize JSON into '{typeof(T).Name}'.");
        }

        return value;
    }

    public T DeserializeFromFile<T>(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "The serialization file was not found.",
                path);
        }

        return Deserialize<T>(
            File.ReadAllText(path));
    }

    public T DeserializeFromStream<T>(
        Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        T? value =
            JsonSerializer.Deserialize<T>(
                stream,
                _options);

        if (value is null)
        {
            throw new JsonException(
                $"Unable to deserialize JSON into '{typeof(T).Name}'.");
        }

        return value;
    }

    public object Deserialize(
        string json,
        Type type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        ArgumentNullException.ThrowIfNull(type);

        object? value =
            JsonSerializer.Deserialize(
                json,
                type,
                _options);

        if (value is null)
        {
            throw new JsonException(
                $"Unable to deserialize JSON into '{type.Name}'.");
        }

        return value;
    }

    public object DeserializeFromFile(
        string path,
        Type type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(type);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "The serialization file was not found.",
                path);
        }

        return Deserialize(
            File.ReadAllText(path),
            type);
    }

    public bool TryDeserialize<T>(
        string json,
        out T? value)
    {
        value = default;

        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            value =
                JsonSerializer.Deserialize<T>(
                    json,
                    _options);

            return value is not null;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}