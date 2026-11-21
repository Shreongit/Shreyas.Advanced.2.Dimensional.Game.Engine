using SA2DGE.Engine.Graphics.Textures;

namespace SA2DGE.Engine.Graphics.Tiles;

/// <summary>
/// Defines a reusable tile inside a tileset.
/// A tile definition references a region of a shared texture.
/// </summary>
public sealed class TileDefinition
{
    /// <summary>
    /// Gets the unique identifier of this tile.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the texture region occupied by this tile.
    /// </summary>
    public TextureRegion Region { get; }

    public TileDefinition(
        int id,
        TextureRegion region)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(id),
                "Tile ID cannot be negative.");
        }

        if (region.Texture is null)
        {
            throw new ArgumentException(
                "Tile region must reference a valid texture.",
                nameof(region));
        }

        Id = id;
        Region = region;
    }
}