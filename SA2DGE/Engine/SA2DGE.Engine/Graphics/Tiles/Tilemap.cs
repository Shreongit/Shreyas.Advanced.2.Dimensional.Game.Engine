
namespace SA2DGE.Engine.Graphics.Tiles;

/// <summary>
/// Represents a two-dimensional grid of tile IDs.
/// Each cell references a tile definition in a Tileset.
/// </summary>
public sealed class Tilemap
{
    public const int EmptyTile = -1;

    private readonly int[,] _tiles;

    /// <summary>
    /// Gets the tileset used by this tilemap.
    /// </summary>
    public Tileset Tileset { get; }

    /// <summary>
    /// Gets the number of columns.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the number of rows.
    /// </summary>
    public int Height { get; }

    public Tilemap(Tileset tileset, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(tileset);

        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        Tileset = tileset;
        Width = width;
        Height = height;

        _tiles = new int[width, height];

        Clear();
    }

    /// <summary>
    /// Gets the tile ID at the specified grid position.
    /// </summary>
    public int GetTile(int x, int y)
    {
        ValidateCoordinates(x, y);

        return _tiles[x, y];
    }

    /// <summary>
    /// Sets the tile ID at the specified grid position.
    /// </summary>
    public void SetTile(int x, int y, int tileId)
    {
        ValidateCoordinates(x, y);

        ValidateTileId(tileId);
        _tiles[x, y] = tileId;
    }

    /// <summary>
    /// Checks whether the specified cell is empty.
    /// </summary>
    public bool IsEmpty(int x, int y)
    {
        return GetTile(x, y) == EmptyTile;
    }
    
    
    
    
/// <summary>
/// Fills every cell in the tilemap with the specified tile ID.
/// Use EmptyTile to clear the entire map.
/// </summary>
public void Fill(int tileId)
{
    ValidateTileId(tileId);

    for (int y = 0; y < Height; y++)
    {
        for (int x = 0; x < Width; x++)
        {
            _tiles[x, y] = tileId;
        }
    }
}

/// <summary>
/// Fills a rectangular area with the specified tile ID.
/// Coordinates represent the top-left corner.
/// Width and height represent the rectangle dimensions.
/// </summary>
public void FillRectangle(
    int x,
    int y,
    int width,
    int height,
    int tileId)
{
    ValidateTileId(tileId);

    if (width <= 0)
        throw new ArgumentOutOfRangeException(nameof(width));

    if (height <= 0)
        throw new ArgumentOutOfRangeException(nameof(height));

    if (x < 0 || y < 0 ||
        x > Width - width ||
        y > Height - height)
    {
        throw new ArgumentOutOfRangeException(
            nameof(x),
            "Rectangle must fit entirely inside the tilemap.");
    }

    for (int row = y; row < y + height; row++)
    {
        for (int column = x; column < x + width; column++)
        {
            _tiles[column, row] = tileId;
        }
    }
}

/// <summary>
/// Gets the tile definition at the specified grid position.
/// Returns null when the cell is empty.
/// </summary>
public TileDefinition? GetTileDefinition(int x, int y)
{
    int tileId = GetTile(x, y);

    if (tileId == EmptyTile)
        return null;

    return Tileset.Get(tileId);
}

/// <summary>
/// Attempts to retrieve the tile definition at a grid position.
/// Returns false for empty cells.
/// </summary>
/// <summary>
/// Attempts to retrieve the tile definition at a grid position.
/// Returns false for empty cells or missing tile definitions.
/// </summary>
public bool TryGetTileDefinition(
    int x,
    int y,
    out TileDefinition? tile)
{
    int tileId = GetTile(x, y);

    if (tileId == EmptyTile)
    {
        tile = null;
        return false;
    }

    return Tileset.TryGet(tileId, out tile);
}

/// <summary>
/// Returns a copy of the specified row of tile IDs.
/// </summary>
public int[] GetRow(int y)
{
    if (y < 0 || y >= Height)
        throw new ArgumentOutOfRangeException(nameof(y));

    int[] row = new int[Width];

    for (int x = 0; x < Width; x++)
    {
        row[x] = _tiles[x, y];
    }

    return row;
}

/// <summary>
/// Validates that a tile ID is registered or represents an empty cell.
/// </summary>
private void ValidateTileId(int tileId)
{
    if (tileId != EmptyTile && !Tileset.Contains(tileId))
    {
        throw new ArgumentException(
            $"Tile ID '{tileId}' does not exist in the tileset.",
            nameof(tileId));
    }
}


    /// <summary>
    /// Clears every cell in the tilemap.
    /// </summary>
    public void Clear()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                _tiles[x, y] = EmptyTile;
            }
        }
    }

    private void ValidateCoordinates(int x, int y)
    {
        if (x < 0 || x >= Width)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }

        if (y < 0 || y >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }
    }
}
