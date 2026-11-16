
using SA2DGE.Engine.Graphics.Cameras;
using SA2DGE.Engine.Math;
using Xunit;

public sealed class Camera2DTests
{
    private const float Tolerance = 0.001f;

    [Fact]
    public void Constructor_InitializesDefaultValues()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        Assert.Equal(1280.0f, camera.Width);
        Assert.Equal(720.0f, camera.Height);
        Assert.Equal(1.0f, camera.Zoom);
        Assert.Equal(0.0f, camera.Rotation);
        Assert.False(camera.PixelPerfect);
        Assert.Equal(0.0f, camera.Position.X);
        Assert.Equal(0.0f, camera.Position.Y);
    }

    [Fact]
    public void Constructor_InvalidWidth_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Camera2D(0.0f, 720.0f));
    }

    [Fact]
    public void Constructor_InvalidHeight_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Camera2D(1280.0f, 0.0f));
    }

    [Fact]
    public void WorldToScreen_CameraCenter_MapsToViewportCenter()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        Vector2 result = camera.WorldToScreen(
            new Vector2(0.0f, 0.0f));

        Assert.Equal(640.0f, result.X);
        Assert.Equal(360.0f, result.Y);
    }

    [Fact]
    public void ScreenToWorld_ViewportCenter_MapsToCameraPosition()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        Vector2 result = camera.ScreenToWorld(
            new Vector2(640.0f, 360.0f));

        Assert.Equal(camera.Position.X, result.X);
        Assert.Equal(camera.Position.Y, result.Y);
    }

    [Fact]
    public void WorldToScreen_RespectsCameraZoom()
    {
        var camera = new Camera2D(1280.0f, 720.0f)
        {
            Zoom = 2.0f
        };

        Vector2 result = camera.WorldToScreen(
            new Vector2(100.0f, 50.0f));

        Assert.Equal(840.0f, result.X);
        Assert.Equal(460.0f, result.Y);
    }

    [Fact]
    public void ScreenToWorld_RespectsCameraZoom()
    {
        var camera = new Camera2D(1280.0f, 720.0f)
        {
            Zoom = 2.0f
        };

        Vector2 result = camera.ScreenToWorld(
            new Vector2(840.0f, 460.0f));

        Assert.Equal(100.0f, result.X);
        Assert.Equal(50.0f, result.Y);
    }

    [Fact]
    public void WorldToScreenAndScreenToWorld_RoundTrip()
    {
        var camera = new Camera2D(1280.0f, 720.0f)
        {
            Zoom = 1.5f,
            Rotation = 0.35f
        };

        Vector2 original = new(250.0f, 180.0f);

        Vector2 screen = camera.WorldToScreen(original);
        Vector2 restored = camera.ScreenToWorld(screen);

        Assert.InRange(
            MathF.Abs(original.X - restored.X),
            0.0f,
            Tolerance);

        Assert.InRange(
            MathF.Abs(original.Y - restored.Y),
            0.0f,
            Tolerance);
    }

    [Fact]
    public void WorldToScreen_RespectsCameraPosition()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        camera.Position = new Vector3(100.0f, 50.0f, 1.0f);

        Vector2 result = camera.WorldToScreen(
            new Vector2(100.0f, 50.0f));

        Assert.Equal(640.0f, result.X);
        Assert.Equal(360.0f, result.Y);
    }
    
    
    [Fact]
    public void Update_ChangesProjectionMatrix_WhenZoomChanges()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        float originalM11 = camera.ProjectionMatrix.M11;

        camera.Zoom = 2.0f;
        camera.Update();

        float updatedM11 = camera.ProjectionMatrix.M11;

        Assert.NotEqual(originalM11, updatedM11);
        Assert.Equal(0.003125f, updatedM11, 6);
    }

    [Fact]
    public void Update_ChangesProjectionMatrix_WhenWidthChanges()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        float originalM11 = camera.ProjectionMatrix.M11;

        camera.Width = 1600.0f;
        camera.Update();

        float updatedM11 = camera.ProjectionMatrix.M11;

        Assert.NotEqual(originalM11, updatedM11);
        Assert.Equal(0.00125f, updatedM11, 6);
    }

    [Fact]
    public void Update_ChangesProjectionMatrix_WhenHeightChanges()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        float originalM22 = camera.ProjectionMatrix.M22;

        camera.Height = 900.0f;
        camera.Update();

        float updatedM22 = camera.ProjectionMatrix.M22;

        Assert.NotEqual(originalM22, updatedM22);
        Assert.Equal(
            2.0f / 900.0f,
            MathF.Abs(updatedM22),
            6);
    }

    [Fact]
    public void Update_PixelPerfectMode_ProducesValidProjection()
    {
        var camera = new Camera2D(1280.0f, 720.0f)
        {
            PixelPerfect = true
        };

        camera.Update();

        Assert.True(float.IsFinite(camera.ProjectionMatrix.M11));
        Assert.True(float.IsFinite(camera.ProjectionMatrix.M22));
        Assert.NotEqual(0.0f, camera.ProjectionMatrix.M11);
        Assert.NotEqual(0.0f, camera.ProjectionMatrix.M22);
    }

    [Fact]
    public void Width_InvalidValue_Throws()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => camera.Width = 0.0f);
    }

    [Fact]
    public void Height_InvalidValue_Throws()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => camera.Height = float.NaN);
    }

    [Fact]
    public void Zoom_InvalidValue_Throws()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => camera.Zoom = -1.0f);
    }

    [Fact]
    public void Rotation_NonFiniteValue_Throws()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => camera.Rotation = float.PositiveInfinity);
    }
    
    
    [Fact]
    public void Position_ChangesViewMatrix()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        Matrix4 originalViewMatrix = camera.ViewMatrix;

        camera.Position = new Vector3(100.0f, 50.0f, 1.0f);

        Assert.NotEqual(
            originalViewMatrix,
            camera.ViewMatrix);
    }
    
    
    [Fact]
    public void Rotation_ChangesViewMatrix()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        Matrix4 originalView = camera.ViewMatrix;

        camera.Rotation = MathF.PI / 2.0f;

        Assert.NotEqual(originalView, camera.ViewMatrix);
    }

    [Fact]
    public void Rotation_ZeroDegrees_PreservesOriginalView()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        Matrix4 originalView = camera.ViewMatrix;

        camera.Rotation = 0.0f;

        Assert.Equal(originalView, camera.ViewMatrix);
    }

    [Fact]
    public void Rotation_UpdatesViewMatrix_WhenPositionChanges()
    {
        var camera = new Camera2D(1280.0f, 720.0f)
        {
            Rotation = MathF.PI / 4.0f
        };

        Matrix4 originalView = camera.ViewMatrix;

        camera.Position = new Vector3(100.0f, 50.0f, 1.0f);

        Assert.NotEqual(originalView, camera.ViewMatrix);
    }
    
    
    [Fact]
    public void ViewProjectionMatrix_MapsCameraCenterToViewportCenter()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        Vector3 worldPosition = new(
            camera.Position.X,
            camera.Position.Y,
            0.0f);

        Vector3 clipPosition = Matrix4.Transform(
            worldPosition,
            camera.ViewProjectionMatrix);

        float screenX = (clipPosition.X + 1.0f) * camera.Width * 0.5f;
        float screenY = (1.0f - clipPosition.Y) * camera.Height * 0.5f;

        Assert.InRange(MathF.Abs(screenX - 640.0f), 0.0f, Tolerance);
        Assert.InRange(MathF.Abs(screenY - 360.0f), 0.0f, Tolerance);
    }
    
    [Fact]
    public void ViewProjectionMatrix_ConsistentWithWorldToScreen()
    {
        // Arrange
        var camera = new Camera2D(1280.0f, 720.0f);

        Vector2 worldPosition = new(
            camera.Position.X + 100.0f,
            camera.Position.Y + 50.0f);

        // Act: Direct camera conversion
        Vector2 expectedScreenPosition =
            camera.WorldToScreen(worldPosition);

        // Act: Matrix-based conversion
        Vector3 clipPosition = Matrix4.Transform(
            new Vector3(worldPosition.X, worldPosition.Y, 0.0f),
            camera.ViewProjectionMatrix);

        float actualScreenX =
            (clipPosition.X + 1.0f) * camera.Width * 0.5f;

        float actualScreenY =
            (1.0f - clipPosition.Y) * camera.Height * 0.5f;

        // Assert
        Assert.InRange(
            MathF.Abs(actualScreenX - expectedScreenPosition.X),
            0.0f,
            Tolerance);

        Assert.InRange(
            MathF.Abs(actualScreenY - expectedScreenPosition.Y),
            0.0f,
            Tolerance);
    }
    
    [Fact]
    public void ViewProjectionMatrix_ConsistentWithCameraTranslation()
    {
        // Arrange
        var camera = new Camera2D(1280.0f, 720.0f);

        camera.Position = new Vector3(150.0f, 100.0f, 1.0f);
        camera.Target = new Vector3(150.0f, 100.0f, 0.0f);
        Vector2 worldPosition = new(250.0f, 180.0f);

        // Act: Direct transformation
        Vector2 expectedScreenPosition =
            camera.WorldToScreen(worldPosition);

        // Act: Matrix transformation
        Vector3 clipPosition = Matrix4.Transform(
            new Vector3(worldPosition.X, worldPosition.Y, 0.0f),
            camera.ViewProjectionMatrix);

        float actualScreenX =
            (clipPosition.X + 1.0f) * camera.Width * 0.5f;

        float actualScreenY =
            (1.0f - clipPosition.Y) * camera.Height * 0.5f;

        // Assert
        Assert.InRange(
            MathF.Abs(actualScreenX - expectedScreenPosition.X),
            0.0f,
            Tolerance);

        Assert.InRange(
            MathF.Abs(actualScreenY - expectedScreenPosition.Y),
            0.0f,
            Tolerance);
    }
    
    [Fact]
    public void ViewProjectionMatrix_ConsistentWithCameraRotation()
    {
        // Arrange
        var camera = new Camera2D(1280.0f, 720.0f);

        camera.Rotation = MathF.PI / 6.0f; // 30 degrees

        Vector2 worldPosition = new(100.0f, 50.0f);

        // Act: Direct transformation
        Vector2 expectedScreenPosition =
            camera.WorldToScreen(worldPosition);

        // Act: Matrix transformation
        Vector3 clipPosition = Matrix4.Transform(
            new Vector3(worldPosition.X, worldPosition.Y, 0.0f),
            camera.ViewProjectionMatrix);

        float actualScreenX =
            (clipPosition.X + 1.0f) * camera.Width * 0.5f;

        float actualScreenY =
            (1.0f - clipPosition.Y) * camera.Height * 0.5f;

        // Assert
        Assert.InRange(
            MathF.Abs(actualScreenX - expectedScreenPosition.X),
            0.0f,
            Tolerance);

        Assert.InRange(
            MathF.Abs(actualScreenY - expectedScreenPosition.Y),
            0.0f,
            Tolerance);
    }
    
    [Fact]
    public void ViewProjectionMatrix_ConsistentWithCameraZoom()
    {
        // Arrange
        var camera = new Camera2D(1280.0f, 720.0f);

        camera.Zoom = 2.0f;

        Vector2 worldPosition = new(100.0f, 50.0f);

        // Act: Direct transformation
        Vector2 expectedScreenPosition =
            camera.WorldToScreen(worldPosition);

        // Act: Matrix transformation
        Vector3 clipPosition = Matrix4.Transform(
            new Vector3(worldPosition.X, worldPosition.Y, 0.0f),
            camera.ViewProjectionMatrix);

        float actualScreenX =
            (clipPosition.X + 1.0f) * camera.Width * 0.5f;

        float actualScreenY =
            (1.0f - clipPosition.Y) * camera.Height * 0.5f;

        // Assert
        Assert.InRange(
            MathF.Abs(actualScreenX - expectedScreenPosition.X),
            0.0f,
            Tolerance);

        Assert.InRange(
            MathF.Abs(actualScreenY - expectedScreenPosition.Y),
            0.0f,
            Tolerance);
    }
    
    [Fact]
    public void ViewProjectionMatrix_ConsistentWithViewportDimensions()
    {
        // Arrange
        var camera = new Camera2D(1280.0f, 720.0f);

        camera.Width = 1920.0f;
        camera.Height = 1080.0f;

        Vector2 worldPosition = new(100.0f, 50.0f);

        // Act: Direct transformation
        Vector2 expectedScreenPosition =
            camera.WorldToScreen(worldPosition);

        // Act: Matrix transformation
        Vector3 clipPosition = Matrix4.Transform(
            new Vector3(worldPosition.X, worldPosition.Y, 0.0f),
            camera.ViewProjectionMatrix);

        float actualScreenX =
            (clipPosition.X + 1.0f) * camera.Width * 0.5f;

        float actualScreenY =
            (1.0f - clipPosition.Y) * camera.Height * 0.5f;

        // Assert
        Assert.InRange(
            MathF.Abs(actualScreenX - expectedScreenPosition.X),
            0.0f,
            Tolerance);

        Assert.InRange(
            MathF.Abs(actualScreenY - expectedScreenPosition.Y),
            0.0f,
            Tolerance);
    }
    
    [Fact]
    public void Follow_MovesCameraToTargetPosition()
    {
        // Arrange
        var camera = new Camera2D(1280.0f, 720.0f);

        Vector2 targetPosition = new(500.0f, 300.0f);

        // Act
        camera.Follow(targetPosition);

        // Assert: Camera position
        Assert.InRange(
            MathF.Abs(camera.Position.X - 500.0f),
            0.0f,
            Tolerance);

        Assert.InRange(
            MathF.Abs(camera.Position.Y - 300.0f),
            0.0f,
            Tolerance);

        // Assert: Camera target
        Assert.InRange(
            MathF.Abs(camera.Target.X - 500.0f),
            0.0f,
            Tolerance);

        Assert.InRange(
            MathF.Abs(camera.Target.Y - 300.0f),
            0.0f,
            Tolerance);

        // Assert: Camera depth remains unchanged
        Assert.InRange(
            MathF.Abs(camera.Position.Z - 1.0f),
            0.0f,
            Tolerance);

        Assert.InRange(
            MathF.Abs(camera.Target.Z),
            0.0f,
            Tolerance);

        // Assert: Follow target remains at viewport centre
        Vector2 screenPosition =
            camera.WorldToScreen(targetPosition);

        Assert.InRange(
            MathF.Abs(screenPosition.X - 640.0f),
            0.0f,
            Tolerance);

        Assert.InRange(
            MathF.Abs(screenPosition.Y - 360.0f),
            0.0f,
            Tolerance);
    }
    
    [Fact]
    public void Follow_DefaultOffset_KeepsTargetAtViewportCenter()
    {
        var camera = new Camera2D(1280.0f, 720.0f);

        Vector2 targetPosition = new(500.0f, 300.0f);

        camera.Follow(targetPosition);

        Vector2 screenPosition = camera.WorldToScreen(targetPosition);

        Assert.InRange(
            MathF.Abs(screenPosition.X - 640.0f),
            0.0f,
            Tolerance);

        Assert.InRange(
            MathF.Abs(screenPosition.Y - 360.0f),
            0.0f,
            Tolerance);
    }
    
    [Fact]
    public void Follow_HorizontalOffset_ShiftsTargetOnScreen()
    {
        var camera = new Camera2D(1280.0f, 720.0f)
        {
            FollowOffset = new Vector2(-150.0f, 0.0f)
        };

        Vector2 targetPosition = new(500.0f, 300.0f);

        camera.Follow(targetPosition);

        Vector2 screenPosition = camera.WorldToScreen(targetPosition);

        Assert.InRange(
            MathF.Abs(screenPosition.X - 490.0f),
            0.0f,
            Tolerance);

        Assert.InRange(
            MathF.Abs(screenPosition.Y - 360.0f),
            0.0f,
            Tolerance);
    }
    
    [Fact]
    public void Follow_VerticalOffset_ShiftsTargetOnScreen()
    {
        var camera = new Camera2D(1280.0f, 720.0f)
        {
            FollowOffset = new Vector2(0.0f, 100.0f)
        };

        Vector2 targetPosition = new(500.0f, 300.0f);

        camera.Follow(targetPosition);

        Vector2 screenPosition = camera.WorldToScreen(targetPosition);

        Assert.InRange(
            MathF.Abs(screenPosition.X - 640.0f),
            0.0f,
            Tolerance);

        Assert.InRange(
            MathF.Abs(screenPosition.Y - 460.0f),
            0.0f,
            Tolerance);
    }
    
    [Fact]
    public void Follow_WithOffset_ViewProjectionMatchesWorldToScreen()
    {
        var camera = new Camera2D(1280.0f, 720.0f)
        {
            FollowOffset = new Vector2(-100.0f, 50.0f),
            Zoom = 1.5f,
            Rotation = MathF.PI / 6.0f
        };

        Vector2 targetPosition = new(500.0f, 300.0f);

        camera.Follow(targetPosition);

        // Direct coordinate transformation
        Vector2 directScreenPosition =
            camera.WorldToScreen(targetPosition);

        // Matrix-based transformation
        Vector3 clipPosition = Matrix4.Transform(
            new Vector3(targetPosition.X, targetPosition.Y, 0.0f),
            camera.ViewProjectionMatrix);

        float matrixScreenX =
            (clipPosition.X + 1.0f) * camera.Width / 2.0f;

        float matrixScreenY =
            (1.0f - clipPosition.Y) * camera.Height / 2.0f;

        // Compare both transformation paths
        Assert.InRange(
            MathF.Abs(matrixScreenX - directScreenPosition.X),
            0.0f,
            Tolerance);

        Assert.InRange(
            MathF.Abs(matrixScreenY - directScreenPosition.Y),
            0.0f,
            Tolerance);
    }
    

}
