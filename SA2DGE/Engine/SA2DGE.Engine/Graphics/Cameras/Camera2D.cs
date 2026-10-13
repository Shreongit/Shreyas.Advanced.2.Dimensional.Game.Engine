using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics.Cameras;

public sealed class Camera2D : Camera
{
    public float Width { get; set; }

    public float Height { get; set; }

    public float Zoom { get; set; }

    public float Rotation { get; set; }

    public bool PixelPerfect { get; set; }

    public Camera2D(
        float width,
        float height)
    {
        if (width <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(width));
        }

        if (height <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(height));
        }

        Width = width;
        Height = height;
        Zoom = 1.0f;
        Rotation = 0.0f;
        PixelPerfect = false;

        Position = new Vector3(
            0.0f,
            0.0f,
            1.0f);

        Target = Vector3.Zero;
        Up = Vector3.UnitY;

        Update();
    }

    public Vector2 ScreenToWorld(
        Vector2 screenPosition)
    {
        float effectiveZoom =
            MathF.Max(
                Zoom,
                MathUtils.Epsilon);

        float x =
            (screenPosition.X - Width * 0.5f) /
            effectiveZoom;

        float y =
            (screenPosition.Y - Height * 0.5f) /
            effectiveZoom;

        float cos = MathF.Cos(Rotation);
        float sin = MathF.Sin(Rotation);

        float rotatedX =
            x * cos - y * sin;

        float rotatedY =
            x * sin + y * cos;

        return new Vector2(
            Position.X + rotatedX,
            Position.Y + rotatedY);
    }

    public Vector2 WorldToScreen(
        Vector2 worldPosition)
    {
        float x =
            worldPosition.X - Position.X;

        float y =
            worldPosition.Y - Position.Y;

        float cos = MathF.Cos(-Rotation);
        float sin = MathF.Sin(-Rotation);

        float rotatedX =
            x * cos - y * sin;

        float rotatedY =
            x * sin + y * cos;

        return new Vector2(
            rotatedX * Zoom + Width * 0.5f,
            rotatedY * Zoom + Height * 0.5f);
    }

    protected override void UpdateProjectionMatrix()
    {
        float effectiveZoom =
            MathF.Max(
                Zoom,
                MathUtils.Epsilon);

        float halfWidth =
            Width * 0.5f / effectiveZoom;

        float halfHeight =
            Height * 0.5f / effectiveZoom;

        if (PixelPerfect)
        {
            halfWidth =
                MathF.Floor(halfWidth);

            halfHeight =
                MathF.Floor(halfHeight);
        }

        SetProjectionMatrix(
            Matrix4.CreateOrthographicOffCenter(
                -halfWidth,
                halfWidth,
                -halfHeight,
                halfHeight,
                -1.0f,
                1.0f));
    }
}