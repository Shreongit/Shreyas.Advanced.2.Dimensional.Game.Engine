
using SA2DGE.Engine.Graphics.Textures;
using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics.Tiles;

/// <summary>
/// Manages reusable tile definitions that share one texture.
/// </summary>
public sealed class Tileset
{
    private readonly Dictionary<int, TileDefinition> _tiles = new();

    /// <summary>
    /// Gets the shared texture used by this tileset.
    /// </summary>
    public Texture2D Texture { get; }

    /// <summary>
    /// Gets the number of registered tiles.
    /// </summary>
    public int Count => _tiles.Count;

    /// <summary>
    /// Gets all registered tile definitions.
    /// </summary>
    public IReadOnlyDictionary<int, TileDefinition> Tiles => _tiles;

    public Tileset(Texture2D texture)
    {
        ArgumentNullException.ThrowIfNull(texture);

        Texture = texture;
    }

    /// <summary>
    /// Registers an existing tile definition.
    /// </summary>
    public void Add(TileDefinition tile)
    {
        ArgumentNullException.ThrowIfNull(tile);

        if (!ReferenceEquals(tile.Region.Texture, Texture))
        {
            throw new ArgumentException(
                "Tile must reference the texture owned by this tileset.",
                nameof(tile));
        }

        if (_tiles.ContainsKey(tile.Id))
        {
            throw new InvalidOperationException(
                $"A tile with ID '{tile.Id}' already exists.");
        }

        _tiles.Add(tile.Id, tile);
    }

    /// <summary>
    /// Creates and registers a tile using pixel coordinates.
    /// </summary>
    public TileDefinition Add(
        int id,
        int x,
        int y,
        int width,
        int height)
    {
        TextureRegion region = new(
            Texture,
            x,
            y,
            width,
            height);

        TileDefinition tile = new(id, region);

        Add(tile);

        return tile;
    }

    /// <summary>
    /// Removes a tile definition by ID.
    /// </summary>
    public bool Remove(int id)
    {
        return _tiles.Remove(id);
    }

    /// <summary>
    /// Gets a tile definition by ID.
    /// Throws when the ID does not exist.
    /// </summary>
    public TileDefinition Get(int id)
    {
        if (!_tiles.TryGetValue(id, out TileDefinition? tile))
        {
            throw new KeyNotFoundException(
                $"Tile with ID '{id}' was not found.");
        }

        return tile;
    }

    /// <summary>
    /// Attempts to retrieve a tile definition.
    /// </summary>
    public bool TryGet(
        int id,
        out TileDefinition? tile)
    {
        return _tiles.TryGetValue(id, out tile);
    }

    /// <summary>
    /// Checks whether a tile ID is registered.
    /// </summary>
    public bool Contains(int id)
    {
        return _tiles.ContainsKey(id);
    }

    /// <summary>
    /// Removes all tile definitions.
    /// </summary>
    public void Clear()
    {
        _tiles.Clear();
    }
}
