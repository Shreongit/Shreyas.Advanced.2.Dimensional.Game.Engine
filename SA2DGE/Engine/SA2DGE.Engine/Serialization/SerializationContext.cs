using System.Text.Json;

namespace SA2DGE.Engine.Serialization;

public sealed class SerializationContext
{
    private readonly Dictionary<string, object?> _values =
        new(StringComparer.Ordinal);

    public JsonSerializerOptions Options { get; }

    public SerializationContext(
        JsonSerializerOptions? options = null)
    {
        Options = options ?? new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };
    }

    public void Set<T>(
        string key,
        T value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        _values[key] = value;
    }

    public T Get<T>(
        string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        if (!_values.TryGetValue(
                key,
                out object? value))
        {
            throw new KeyNotFoundException(
                $"Serialization context value '{key}' was not found.");
        }

        if (value is T typedValue)
        {
            return typedValue;
        }

        throw new InvalidOperationException(
            $"Context value '{key}' is not of type '{typeof(T).Name}'.");
    }

    public bool TryGet<T>(
        string key,
        out T? value)
    {
        value = default;

        if (string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        if (!_values.TryGetValue(
                key,
                out object? storedValue))
        {
            return false;
        }

        if (storedValue is not T typedValue)
        {
            return false;
        }

        value = typedValue;
        return true;
    }

    public bool Contains(
        string key)
    {
        return !string.IsNullOrWhiteSpace(key) &&
               _values.ContainsKey(key);
    }

    public bool Remove(
        string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        return _values.Remove(key);
    }

    public void Clear()
    {
        _values.Clear();
    }

    public IReadOnlyDictionary<string, object?> Values =>
        _values;
}