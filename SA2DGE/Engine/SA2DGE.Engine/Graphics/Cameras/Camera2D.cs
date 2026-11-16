using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics.Cameras;

public sealed class Camera2D : Camera
{
    private float _width;
    private float _height;
    private float _zoom = 1.0f;
    private float _rotation;

    public float Width
    {
        get => _width;
        set
        {
            if (!float.IsFinite(value) || value <= 0.0f)
                throw new ArgumentOutOfRangeException(nameof(value));

            _width = value;
            Update();
        }
    }

    public float Height
    {
        get => _height;
        set
        {
            if (!float.IsFinite(value) || value <= 0.0f)
                throw new ArgumentOutOfRangeException(nameof(value));

            _height = value;
            Update();
        }
    }

    public float Zoom
    {
        get => _zoom;
        set
        {
            if (!float.IsFinite(value) || value <= 0.0f)
                throw new ArgumentOutOfRangeException(nameof(value));

            _zoom = value;
            Update();
        }
    }

    public float Rotation
    {
        get => _rotation;
        set
        {
            if (!float.IsFinite(value))
                throw new ArgumentOutOfRangeException(nameof(value));

            _rotation = value;
            Update();
        }
    }
    
    public Vector2 FollowOffset { get; set; } = Vector2.Zero;

    public bool PixelPerfect { get; set; }

    public Camera2D(float width, float height)
    {
        if (!float.IsFinite(width) || width <= 0.0f)
            throw new ArgumentOutOfRangeException(nameof(width));

        if (!float.IsFinite(height) || height <= 0.0f)
            throw new ArgumentOutOfRangeException(nameof(height));

        _width = width;
        _height = height;
        _zoom = 1.0f;
        _rotation = 0.0f;
        PixelPerfect = false;

        Position = new Vector3(0.0f, 0.0f, 1.0f);

        Target = Vector3.Zero;
        Up = -Vector3.UnitY;

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
    protected override void UpdateViewMatrix()
    {
        float cos = MathF.Cos(Rotation);
        float sin = MathF.Sin(Rotation);

        Vector3 rotatedUp = new Vector3(
            Up.X * cos - Up.Y * sin,
            Up.X * sin + Up.Y * cos,
            Up.Z);

        SetViewMatrixFromUp(rotatedUp);
    }
    
    /// <summary>
    /// Moves the camera to follow a world-space position
    /// while preserving its current viewing direction.
    /// Call this method with the latest target position
    /// during each update.
    /// </summary>
    public void Follow(Vector2 targetPosition)
    {
        if (!float.IsFinite(targetPosition.X) ||
            !float.IsFinite(targetPosition.Y))
        {
            throw new ArgumentOutOfRangeException(
                nameof(targetPosition),
                "Follow target coordinates must be finite.");
        }

        float cos = MathF.Cos(Rotation);
        float sin = MathF.Sin(Rotation);

        // Convert the desired screen-space offset
        // into a world-space camera offset.
        Vector2 rotatedOffset = new(
            (FollowOffset.X * cos - FollowOffset.Y * sin) / Zoom,
            (FollowOffset.X * sin + FollowOffset.Y * cos) / Zoom);

        Vector2 cameraPosition = new(
            targetPosition.X - rotatedOffset.X,
            targetPosition.Y - rotatedOffset.Y);

        Position = new Vector3(
            cameraPosition.X,
            cameraPosition.Y,
            Position.Z);

        Target = new Vector3(
            cameraPosition.X,
            cameraPosition.Y,
            Target.Z);
    }
    
    /// <summary>
    /// Follows a target while keeping the camera inside
    /// the specified world boundaries.
    /// </summary>
    public void FollowWithinBounds(
        Vector2 targetPosition,
        float worldWidth,
        float worldHeight)
    {
        if (!float.IsFinite(worldWidth) ||
            !float.IsFinite(worldHeight) ||
            worldWidth <= 0.0f ||
            worldHeight <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(worldWidth),
                "World dimensions must be finite and positive.");
        }

        // First calculate the normal follow position.
        Follow(targetPosition);

        // Calculate the visible half-size in world coordinates.
        float halfWidth = Width / (2.0f * Zoom);
        float halfHeight = Height / (2.0f * Zoom);

        // Account for camera rotation when calculating boundaries.
        float cos = MathF.Abs(MathF.Cos(Rotation));
        float sin = MathF.Abs(MathF.Sin(Rotation));

        float extentX =
            cos * halfWidth + sin * halfHeight;

        float extentY =
            sin * halfWidth + cos * halfHeight;

        // Calculate valid camera center positions.
        float minX = extentX;
        float maxX = worldWidth - extentX;

        float minY = extentY;
        float maxY = worldHeight - extentY;

        // If the viewport is larger than the world,
        // center the camera on that axis.
        float cameraX = minX > maxX
            ? worldWidth * 0.5f
            : System.Math.Clamp(Position.X, minX, maxX);

        float cameraY = minY > maxY
            ? worldHeight * 0.5f
            : System.Math.Clamp(Position.Y, minY, maxY);

        // Update camera position and target together.
        Position = new Vector3(
            cameraX,
            cameraY,
            Position.Z);

        Target = new Vector3(
            cameraX,
            cameraY,
            Target.Z);
    }
}