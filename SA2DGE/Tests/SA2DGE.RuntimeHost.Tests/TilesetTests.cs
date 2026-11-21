
using SA2DGE.Engine.Graphics.Textures;
using SA2DGE.Engine.Graphics.Tiles;
using Xunit;

public sealed class TilesetTests
{
    private static Texture2D CreateTexture()
    {
        return new Texture2D(256, 256);
    }

    [Fact]
    public void Constructor_StoresTexture()
    {
        var texture = CreateTexture();
        var tileset = new Tileset(texture);

        Assert.Same(texture, tileset.Texture);
        Assert.Equal(0, tileset.Count);
    }

    [Fact]
    public void Constructor_NullTexture_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new Tileset(null!));
    }

    [Fact]
    public void Add_RegistersTile()
    {
        var tileset = new Tileset(CreateTexture());

        TileDefinition tile = tileset.Add(
            0, 0, 0, 32, 32);

        Assert.Equal(1, tileset.Count);
        Assert.True(tileset.Contains(0));
        Assert.Same(tile, tileset.Get(0));
    }

    [Fact]
    public void Add_ExistingTileDefinition_RegistersTile()
    {
        var texture = CreateTexture();
        var tileset = new Tileset(texture);

        var region = new TextureRegion(
            texture, 0, 0, 32, 32);

        var tile = new TileDefinition(5, region);

        tileset.Add(tile);

        Assert.Same(tile, tileset.Get(5));
        Assert.Equal(1, tileset.Count);
    }

    [Fact]
    public void Add_DuplicateId_Throws()
    {
        var tileset = new Tileset(CreateTexture());

        tileset.Add(1, 0, 0, 32, 32);

        Assert.Throws<InvalidOperationException>(
            () => tileset.Add(1, 32, 0, 32, 32));

        Assert.Equal(1, tileset.Count);
    }

    [Fact]
    public void Add_TileFromDifferentTexture_Throws()
    {
        var tileset = new Tileset(CreateTexture());
        var differentTexture = CreateTexture();

        var region = new TextureRegion(
            differentTexture, 0, 0, 32, 32);

        var tile = new TileDefinition(1, region);

        Assert.Throws<ArgumentException>(
            () => tileset.Add(tile));

        Assert.Equal(0, tileset.Count);
    }

    [Fact]
    public void Get_ExistingId_ReturnsCorrectTile()
    {
        var tileset = new Tileset(CreateTexture());

        var expected = tileset.Add(
            10, 32, 32, 16, 16);

        TileDefinition actual = tileset.Get(10);

        Assert.Same(expected, actual);
    }

    [Fact]
    public void Get_MissingId_Throws()
    {
        var tileset = new Tileset(CreateTexture());

        Assert.Throws<KeyNotFoundException>(
            () => tileset.Get(99));
    }

    [Fact]
    public void TryGet_ExistingId_ReturnsTrue()
    {
        var tileset = new Tileset(CreateTexture());

        var expected = tileset.Add(
            2, 0, 0, 32, 32);

        bool result = tileset.TryGet(
            2, out TileDefinition? actual);

        Assert.True(result);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void TryGet_MissingId_ReturnsFalse()
    {
        var tileset = new Tileset(CreateTexture());

        bool result = tileset.TryGet(
            99, out TileDefinition? actual);

        Assert.False(result);
        Assert.Null(actual);
    }

    [Fact]
    public void Contains_RegisteredId_ReturnsTrue()
    {
        var tileset = new Tileset(CreateTexture());

        tileset.Add(3, 0, 0, 32, 32);

        Assert.True(tileset.Contains(3));
    }

    [Fact]
    public void Contains_MissingId_ReturnsFalse()
    {
        var tileset = new Tileset(CreateTexture());

        Assert.False(tileset.Contains(99));
    }

    [Fact]
    public void Remove_ExistingId_RemovesTile()
    {
        var tileset = new Tileset(CreateTexture());

        tileset.Add(4, 0, 0, 32, 32);

        bool result = tileset.Remove(4);

        Assert.True(result);
        Assert.False(tileset.Contains(4));
        Assert.Equal(0, tileset.Count);
    }

    [Fact]
    public void Remove_MissingId_ReturnsFalse()
    {
        var tileset = new Tileset(CreateTexture());

        Assert.False(tileset.Remove(99));
    }

    [Fact]
    public void Clear_RemovesAllTiles()
    {
        var tileset = new Tileset(CreateTexture());

        tileset.Add(0, 0, 0, 32, 32);
        tileset.Add(1, 32, 0, 32, 32);
        tileset.Add(2, 64, 0, 32, 32);

        tileset.Clear();

        Assert.Equal(0, tileset.Count);
        Assert.Empty(tileset.Tiles);
    }

    [Fact]
    public void Add_InvalidRegion_Throws()
    {
        var tileset = new Tileset(CreateTexture());

        Assert.Throws<ArgumentException>(
            () => tileset.Add(
                1, 240, 240, 32, 32));
    }
    
    
}
