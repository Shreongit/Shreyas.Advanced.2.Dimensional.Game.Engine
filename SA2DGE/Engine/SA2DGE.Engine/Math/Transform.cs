namespace SA2DGE.Engine.Math;

public struct Transform
{
    public Vector2 Position { get; set; }

    public float Rotation { get; set; }

    public Vector2 Scale { get; set; }

    public static Transform Identity => new()
    {
        Position = Vector2.Zero,
        Rotation = 0.0f,
        Scale = Vector2.One
    };

    public Transform(
        Vector2 position,
        float rotation = 0.0f,
        Vector2? scale = null)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale ?? Vector2.One;
    }

    public Matrix3 ToMatrix()
    {
        Matrix3 translation =
            Matrix3.CreateTranslation(Position);

        Matrix3 rotation =
            Matrix3.CreateRotation(Rotation);

        Matrix3 scale =
            Matrix3.CreateScale(Scale);

        return translation * rotation * scale;
    }

    public Vector2 Forward
    {
        get
        {
            float cos = MathF.Cos(Rotation);
            float sin = MathF.Sin(Rotation);

            return new Vector2(cos, sin);
        }
    }

    public void Translate(Vector2 amount)
    {
        Position += amount;
    }

    public void Rotate(float amount)
    {
        Rotation += amount;
    }

    public void ScaleBy(Vector2 amount)
    {
        Scale = new Vector2(
            Scale.X * amount.X,
            Scale.Y * amount.Y);
    }

    public override readonly string ToString()
    {
        return $"Position: {Position}, Rotation: {Rotation}, Scale: {Scale}";
    }
}