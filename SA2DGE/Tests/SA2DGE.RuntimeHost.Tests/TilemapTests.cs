
using SA2DGE.Engine.Graphics.Textures;
using SA2DGE.Engine.Graphics.Tiles;
using Xunit;

public sealed class TilemapTests
{
    private static Tileset CreateTileset()
    {
        var texture = new Texture2D(256, 256);
        var tileset = new Tileset(texture);

        tileset.Add(0, 0, 0, 32, 32);
        tileset.Add(1, 32, 0, 32, 32);
        tileset.Add(2, 64, 0, 32, 32);

        return tileset;
    }

    private static Tilemap CreateTilemap()
    {
        return new Tilemap(CreateTileset(), 5, 5);
    }

    [Fact]
    public void Constructor_InitializesAllCellsAsEmpty()
    {
        var map = CreateTilemap();

        Assert.Equal(Tilemap.EmptyTile, map.GetTile(0, 0));
        Assert.True(map.IsEmpty(2, 2));
    }

    [Fact]
    public void Constructor_InvalidWidth_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Tilemap(CreateTileset(), 0, 5));
    }

    [Fact]
    public void Constructor_InvalidHeight_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Tilemap(CreateTileset(), 5, -1));
    }

    [Fact]
    public void Constructor_NullTileset_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new Tilemap(null!, 5, 5));
    }

    [Fact]
    public void SetTile_InvalidTileId_Throws()
    {
        var map = CreateTilemap();

        Assert.Throws<ArgumentException>(
            () => map.SetTile(0, 0, 99));
    }

    [Fact]
    public void SetTile_EmptyTile_IsAllowed()
    {
        var map = CreateTilemap();

        map.SetTile(0, 0, Tilemap.EmptyTile);

        Assert.True(map.IsEmpty(0, 0));
    }

    [Fact]
    public void GetTile_InvalidCoordinates_Throws()
    {
        var map = CreateTilemap();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => map.GetTile(-1, 0));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => map.GetTile(0, 5));
    }

    [Fact]
    public void Fill_InvalidTileId_Throws()
    {
        var map = CreateTilemap();

        Assert.Throws<ArgumentException>(
            () => map.Fill(99));
    }

    [Fact]
    public void FillRectangle_InvalidDimensions_Throws()
    {
        var map = CreateTilemap();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => map.FillRectangle(0, 0, 0, 2, 1));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => map.FillRectangle(0, 0, 2, -1, 1));
    }

    [Fact]
    public void FillRectangle_OutsideMap_Throws()
    {
        var map = CreateTilemap();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => map.FillRectangle(4, 4, 2, 2, 1));
    }

    [Fact]
    public void GetRow_ReturnsCopy()
    {
        var map = CreateTilemap();

        map.SetTile(0, 1, 1);

        int[] row = map.GetRow(1);
        row[0] = 2;

        Assert.Equal(1, map.GetTile(0, 1));
    }

    [Fact]
    public void GetRow_InvalidIndex_Throws()
    {
        var map = CreateTilemap();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => map.GetRow(5));
    }

    [Fact]
    public void GetTileDefinition_EmptyCell_ReturnsNull()
    {
        var map = CreateTilemap();

        Assert.Null(map.GetTileDefinition(0, 0));
    }

    [Fact]
    public void GetTileDefinition_ExistingTile_ReturnsDefinition()
    {
        var map = CreateTilemap();

        map.SetTile(1, 1, 2);

        Assert.Equal(2, map.GetTileDefinition(1, 1)!.Id);
    }

    [Fact]
    public void TryGetTileDefinition_EmptyCell_ReturnsFalse()
    {
        var map = CreateTilemap();

        bool result = map.TryGetTileDefinition(
            0, 0, out TileDefinition? tile);

        Assert.False(result);
        Assert.Null(tile);
    }

    [Fact]
    public void TryGetTileDefinition_ExistingTile_ReturnsTrue()
    {
        var map = CreateTilemap();

        map.SetTile(2, 2, 1);

        bool result = map.TryGetTileDefinition(
            2, 2, out TileDefinition? tile);

        Assert.True(result);
        Assert.Equal(1, tile!.Id);
    }

    [Fact]
    public void GetTileDefinition_RemovedDefinition_Throws()
    {
        var tileset = CreateTileset();
        var map = new Tilemap(tileset, 5, 5);

        map.SetTile(1, 1, 2);
        tileset.Remove(2);

        Assert.Throws<KeyNotFoundException>(
            () => map.GetTileDefinition(1, 1));
    }

    [Fact]
    public void TryGetTileDefinition_RemovedDefinition_ReturnsFalse()
    {
        var tileset = CreateTileset();
        var map = new Tilemap(tileset, 5, 5);

        map.SetTile(1, 1, 2);
        tileset.Remove(2);

        Assert.False(
            map.TryGetTileDefinition(1, 1, out _));
    }
}
