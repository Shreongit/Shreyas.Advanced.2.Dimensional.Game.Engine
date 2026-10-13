using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics.Textures;

public readonly struct TextureRegion : IEquatable<TextureRegion>
{
    public Texture2D Texture { get; }

    public Rectangle Bounds { get; }

    public Vector2 TopLeftUv { get; }

    public Vector2 BottomRightUv { get; }

    public int X => (int)Bounds.X;

    public int Y => (int)Bounds.Y;

    public int Width => (int)Bounds.Width;

    public int Height => (int)Bounds.Height;

    public TextureRegion(
        Texture2D texture,
        Rectangle bounds)
    {
        ArgumentNullException.ThrowIfNull(texture);

        if (bounds.Width <= 0.0f ||
            bounds.Height <= 0.0f)
        {
            throw new ArgumentException(
                "Texture region dimensions must be greater than zero.",
                nameof(bounds));
        }

        if (bounds.Left < 0.0f ||
            bounds.Top < 0.0f ||
            bounds.Right > texture.Width ||
            bounds.Bottom > texture.Height)
        {
            throw new ArgumentException(
                "Texture region must be inside the texture bounds.",
                nameof(bounds));
        }

        Texture = texture;
        Bounds = bounds;

        TopLeftUv = new Vector2(
            bounds.Left / texture.Width,
            bounds.Top / texture.Height);

        BottomRightUv = new Vector2(
            bounds.Right / texture.Width,
            bounds.Bottom / texture.Height);
    }

    public TextureRegion(
        Texture2D texture,
        int x,
        int y,
        int width,
        int height)
        : this(
            texture,
            new Rectangle(
                x,
                y,
                width,
                height))
    {
    }

    public Vector2 GetUv(Vector2 normalizedPosition)
    {
        return new Vector2(
            MathUtils.Lerp(
                TopLeftUv.X,
                BottomRightUv.X,
                normalizedPosition.X),
            MathUtils.Lerp(
                TopLeftUv.Y,
                BottomRightUv.Y,
                normalizedPosition.Y));
    }

    public bool Equals(TextureRegion other)
    {
        return ReferenceEquals(Texture, other.Texture) &&
               Bounds == other.Bounds;
    }

    public override bool Equals(object? obj)
    {
        return obj is TextureRegion other &&
               Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Texture,
            Bounds);
    }

    public static bool operator ==(
        TextureRegion left,
        TextureRegion right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(
        TextureRegion left,
        TextureRegion right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return $"Region: {Bounds}, UV: {TopLeftUv} -> {BottomRightUv}";
    }
}