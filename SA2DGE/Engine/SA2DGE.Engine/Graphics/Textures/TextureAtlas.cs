using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics.Textures;

public sealed class TextureAtlas
{
    private readonly Dictionary<string, TextureRegion> _regions =
        new(StringComparer.OrdinalIgnoreCase);

    public Texture2D Texture { get; }

    public int Count => _regions.Count;

    public IReadOnlyDictionary<string, TextureRegion> Regions =>
        _regions;

    public TextureAtlas(Texture2D texture)
    {
        ArgumentNullException.ThrowIfNull(texture);

        Texture = texture;
    }

    public TextureRegion Add(
        string name,
        Rectangle bounds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_regions.ContainsKey(name))
        {
            throw new InvalidOperationException(
                $"A texture region named '{name}' already exists.");
        }

        TextureRegion region =
            new(Texture, bounds);

        _regions.Add(name, region);

        return region;
    }

    public TextureRegion Add(
        string name,
        int x,
        int y,
        int width,
        int height)
    {
        return Add(
            name,
            new Rectangle(
                x,
                y,
                width,
                height));
    }

    public bool Remove(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return _regions.Remove(name);
    }

    public TextureRegion Get(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!_regions.TryGetValue(
                name,
                out TextureRegion region))
        {
            throw new KeyNotFoundException(
                $"Texture region '{name}' was not found.");
        }

        return region;
    }

    public bool TryGet(
        string name,
        out TextureRegion region)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            region = default;
            return false;
        }

        return _regions.TryGetValue(
            name,
            out region);
    }

    public bool Contains(string name)
    {
        return !string.IsNullOrWhiteSpace(name) &&
               _regions.ContainsKey(name);
    }

    public void Clear()
    {
        _regions.Clear();
    }

    public void AddGrid(
        string prefix,
        int cellWidth,
        int cellHeight,
        int columns,
        int rows,
        int startX = 0,
        int startY = 0,
        int spacingX = 0,
        int spacingY = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);

        if (cellWidth <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(cellWidth));
        }

        if (cellHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(cellHeight));
        }

        if (columns <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(columns));
        }

        if (rows <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rows));
        }

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int x =
                    startX +
                    column * (cellWidth + spacingX);

                int y =
                    startY +
                    row * (cellHeight + spacingY);

                string name =
                    $"{prefix}_{column}_{row}";

                Add(
                    name,
                    x,
                    y,
                    cellWidth,
                    cellHeight);
            }
        }
    }
}