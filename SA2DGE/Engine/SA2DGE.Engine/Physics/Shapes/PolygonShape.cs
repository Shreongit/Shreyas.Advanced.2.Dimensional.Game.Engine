using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Physics.Shapes;

public sealed class PolygonShape
{
    private readonly Vector2[] _vertices;

    public IReadOnlyList<Vector2> Vertices =>
        _vertices;

    public int VertexCount =>
        _vertices.Length;

    public float Area =>
        CalculateArea();

    public PolygonShape(
        IEnumerable<Vector2> vertices)
    {
        ArgumentNullException.ThrowIfNull(vertices);

        _vertices = vertices.ToArray();

        if (_vertices.Length < 3)
        {
            throw new ArgumentException(
                "A polygon must contain at least three vertices.",
                nameof(vertices));
        }
    }

    public PolygonShape(
        params Vector2[] vertices)
        : this((IEnumerable<Vector2>)vertices)
    {
    }

    public Bounds GetBounds(
        Vector2 position)
    {
        Vector2 min = _vertices[0] + position;
        Vector2 max = min;

        for (int i = 1; i < _vertices.Length; i++)
        {
            Vector2 vertex =
                _vertices[i] + position;

            min = new Vector2(
                MathF.Min(min.X, vertex.X),
                MathF.Min(min.Y, vertex.Y));

            max = new Vector2(
                MathF.Max(max.X, vertex.X),
                MathF.Max(max.Y, vertex.Y));
        }

        return new Bounds(min, max);
    }

    public bool Contains(
        Vector2 position,
        Vector2 point)
    {
        bool inside = false;

        for (int i = 0, j = _vertices.Length - 1;
             i < _vertices.Length;
             j = i++)
        {
            Vector2 current =
                _vertices[i] + position;

            Vector2 previous =
                _vertices[j] + position;

            bool crosses =
                (current.Y > point.Y) !=
                (previous.Y > point.Y);

            if (!crosses)
            {
                continue;
            }

            float intersectionX =
                previous.X +
                (point.Y - previous.Y) *
                (current.X - previous.X) /
                (current.Y - previous.Y);

            if (point.X < intersectionX)
            {
                inside = !inside;
            }
        }

        return inside;
    }

    public Vector2 GetCentroid(
        Vector2 position)
    {
        float signedArea = 0.0f;
        float centroidX = 0.0f;
        float centroidY = 0.0f;

        for (int i = 0;
             i < _vertices.Length;
             i++)
        {
            Vector2 current = _vertices[i];
            Vector2 next =
                _vertices[(i + 1) % _vertices.Length];

            float cross =
                current.X * next.Y -
                next.X * current.Y;

            signedArea += cross;

            centroidX +=
                (current.X + next.X) * cross;

            centroidY +=
                (current.Y + next.Y) * cross;
        }

        signedArea *= 0.5f;

        if (MathF.Abs(signedArea) <= MathUtils.Epsilon)
        {
            return position;
        }

        float factor =
            1.0f / (6.0f * signedArea);

        return new Vector2(
            position.X + centroidX * factor,
            position.Y + centroidY * factor);
    }

    public PolygonShape Translated(
        Vector2 offset)
    {
        Vector2[] vertices =
            new Vector2[_vertices.Length];

        for (int i = 0; i < _vertices.Length; i++)
        {
            vertices[i] =
                _vertices[i] + offset;
        }

        return new PolygonShape(vertices);
    }

    private float CalculateArea()
    {
        float area = 0.0f;

        for (int i = 0;
             i < _vertices.Length;
             i++)
        {
            Vector2 current = _vertices[i];
            Vector2 next =
                _vertices[(i + 1) % _vertices.Length];

            area +=
                current.X * next.Y -
                next.X * current.Y;
        }

        return MathF.Abs(area) * 0.5f;
    }
}