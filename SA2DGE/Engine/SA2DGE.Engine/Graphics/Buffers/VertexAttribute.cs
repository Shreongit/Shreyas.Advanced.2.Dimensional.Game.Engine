namespace SA2DGE.Engine.Graphics.Buffers;

public readonly struct VertexAttribute
{
    public string SemanticName { get; }

    public int SemanticIndex { get; }

    public VertexAttributeType Type { get; }

    public int Offset { get; }

    public bool PerInstance { get; }

    public VertexAttribute(
        string semanticName,
        int semanticIndex,
        VertexAttributeType type,
        int offset,
        bool perInstance = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            semanticName);

        if (semanticIndex < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(semanticIndex));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(offset));
        }

        SemanticName = semanticName;
        SemanticIndex = semanticIndex;
        Type = type;
        Offset = offset;
        PerInstance = perInstance;
    }
}