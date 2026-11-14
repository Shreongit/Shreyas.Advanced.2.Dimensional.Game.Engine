using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics.Cameras;

public abstract class Camera
{
    private Matrix4 _viewMatrix;
    private Matrix4 _projectionMatrix;

    private Vector3 _position;
    private Vector3 _target;
    private Vector3 _up;

    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            UpdateViewMatrix();
        }
    }

    public Vector3 Target
    {
        get => _target;
        set
        {
            _target = value;
            UpdateViewMatrix();
        }
    }

    public Vector3 Up
    {
        get => _up;
        set
        {
            _up = value;
            UpdateViewMatrix();
        }
    }

    public Matrix4 ViewMatrix => _viewMatrix;

    public Matrix4 ProjectionMatrix => _projectionMatrix;

    public Matrix4 ViewProjectionMatrix =>
        _projectionMatrix * _viewMatrix;
    
    protected Camera()
    {
        _position = new Vector3(0.0f, 0.0f, 1.0f);
        _target = Vector3.Zero;
        _up = Vector3.UnitY;

        _viewMatrix = Matrix4.Identity;
        _projectionMatrix = Matrix4.Identity;
    }

    public void LookAt(
        Vector3 target)
    {
        Target = target;

        UpdateViewMatrix();
    }

    public void SetPosition(
        Vector3 position)
    {
        Position = position;

        UpdateViewMatrix();
    }

    public void SetUp(
        Vector3 up)
    {
        Up = up;

        UpdateViewMatrix();
    }

    public void Update()
    {
        UpdateViewMatrix();
        UpdateProjectionMatrix();
    }

    protected void SetProjectionMatrix(
        Matrix4 projectionMatrix)
    {
        _projectionMatrix = projectionMatrix;
    }

    protected virtual void UpdateViewMatrix()
    {
        SetViewMatrixFromUp(Up);
    }

    protected void SetViewMatrixFromUp(Vector3 up)
    {
        _viewMatrix =
            CreateLookAt(
                Position,
                Target,
                up);
    }

    protected abstract void UpdateProjectionMatrix();

    private static Matrix4 CreateLookAt(
        Vector3 position,
        Vector3 target,
        Vector3 up)
    {
        Vector3 forward =
            (target - position).Normalized();

        Vector3 right =
            Vector3.Cross(
                up,
                forward).Normalized();

        Vector3 correctedUp =
            Vector3.Cross(
                forward,
                right);

        return new Matrix4(
            right.X,
            right.Y,
            right.Z,
            -Vector3.Dot(right, position),

            correctedUp.X,
            correctedUp.Y,
            correctedUp.Z,
            -Vector3.Dot(correctedUp, position),

            forward.X,
            forward.Y,
            forward.Z,
            -Vector3.Dot(forward, position),

            0.0f,
            0.0f,
            0.0f,
            1.0f);
    }
}