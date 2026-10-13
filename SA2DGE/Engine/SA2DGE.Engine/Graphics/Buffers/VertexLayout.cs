namespace SA2DGE.Engine.Graphics.Buffers;

public sealed class VertexLayout
{
    private readonly List<VertexAttribute> _attributes = new();

    public IReadOnlyList<VertexAttribute> Attributes =>
        _attributes;

    public int Stride { get; private set; }

    public bool IsEmpty =>
        _attributes.Count == 0;

    public void Add(
        VertexAttribute attribute)
    {
        if (_attributes.Any(existing =>
                existing.SemanticName.Equals(
                    attribute.SemanticName,
                    StringComparison.OrdinalIgnoreCase) &&
                existing.SemanticIndex == attribute.SemanticIndex))
        {
            throw new InvalidOperationException(
                $"Vertex attribute '{attribute.SemanticName}{attribute.SemanticIndex}' is already defined.");
        }

        _attributes.Add(attribute);

        int attributeEnd =
            attribute.Offset +
            GetSizeInBytes(attribute.Type);

        if (attributeEnd > Stride)
        {
            Stride = attributeEnd;
        }
    }

    public void Clear()
    {
        _attributes.Clear();
        Stride = 0;
    }

    private static int GetSizeInBytes(
        VertexAttributeType type)
    {
        return type switch
        {
            VertexAttributeType.Float => 4,
            VertexAttributeType.Float2 => 8,
            VertexAttributeType.Float3 => 12,
            VertexAttributeType.Float4 => 16,

            VertexAttributeType.Int => 4,
            VertexAttributeType.Int2 => 8,
            VertexAttributeType.Int3 => 12,
            VertexAttributeType.Int4 => 16,

            VertexAttributeType.UInt => 4,
            VertexAttributeType.UInt2 => 8,
            VertexAttributeType.UInt3 => 12,
            VertexAttributeType.UInt4 => 16,

            _ => throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "Unsupported vertex attribute type.")
        };
    }
}