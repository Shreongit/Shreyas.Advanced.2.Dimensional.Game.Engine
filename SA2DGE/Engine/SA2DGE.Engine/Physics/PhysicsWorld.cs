using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Physics;

public sealed class PhysicsWorld : IDisposable
{
    private readonly List<Rigidbody> _bodies = new();
    private readonly List<Collider> _colliders = new();

    private bool _disposed;

    public Vector2 Gravity { get; set; }

    public IReadOnlyList<Rigidbody> Bodies =>
        _bodies;

    public IReadOnlyList<Collider> Colliders =>
        _colliders;

    public int BodyCount =>
        _bodies.Count;

    public int ColliderCount =>
        _colliders.Count;

    public PhysicsWorld(
        Vector2? gravity = null)
    {
        Gravity = gravity ??
                  new Vector2(0.0f, -9.81f);
    }

    public void AddBody(
        Rigidbody body)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(body);

        if (_bodies.Contains(body))
        {
            return;
        }

        _bodies.Add(body);
    }

    public bool RemoveBody(
        Rigidbody body)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(body);

        return _bodies.Remove(body);
    }

    public void AddCollider(
        Collider collider)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(collider);

        if (_colliders.Contains(collider))
        {
            return;
        }

        _colliders.Add(collider);

        if (collider.Rigidbody is not null)
        {
            AddBody(collider.Rigidbody);
        }
    }

    public bool RemoveCollider(
        Collider collider)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(collider);

        return _colliders.Remove(collider);
    }

    public void Step(
        float deltaTime)
    {
        ThrowIfDisposed();

        if (deltaTime < 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(deltaTime));
        }

        foreach (Rigidbody body in _bodies)
        {
            body.Integrate(
                Gravity,
                deltaTime);
        }
    }

    public bool Raycast(
        Ray ray,
        float maxDistance,
        out RaycastHit hit)
    {
        ThrowIfDisposed();

        if (maxDistance < 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxDistance));
        }

        bool foundHit = false;
        float closestDistance = maxDistance;
        RaycastHit closestHit = default;

        foreach (Collider collider in _colliders)
        {
            if (!TryRaycast(
                    ray,
                    collider,
                    closestDistance,
                    out RaycastHit currentHit))
            {
                continue;
            }

            foundHit = true;
            closestDistance = currentHit.Distance;
            closestHit = currentHit;
        }

        hit = closestHit;
        return foundHit;
    }

    public bool CheckCollision(
        Collider colliderA,
        Collider colliderB,
        out Collision collision)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(colliderA);
        ArgumentNullException.ThrowIfNull(colliderB);

        if (ReferenceEquals(
                colliderA,
                colliderB))
        {
            collision = default;
            return false;
        }

        return TryCreateCollision(
            colliderA,
            colliderB,
            out collision);
    }

    public IEnumerable<Collision> DetectCollisions()
    {
        ThrowIfDisposed();

        for (int i = 0;
             i < _colliders.Count;
             i++)
        {
            Collider colliderA =
                _colliders[i];

            for (int j = i + 1;
                 j < _colliders.Count;
                 j++)
            {
                Collider colliderB =
                    _colliders[j];

                if (ReferenceEquals(
                        colliderA.Rigidbody,
                        colliderB.Rigidbody) &&
                    colliderA.Rigidbody is not null)
                {
                    continue;
                }

                if (TryCreateCollision(
                        colliderA,
                        colliderB,
                        out Collision collision))
                {
                    yield return collision;
                }
            }
        }
    }

    public void Clear()
    {
        ThrowIfDisposed();

        _colliders.Clear();
        _bodies.Clear();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Clear();

        _disposed = true;
    }

    private static bool TryCreateCollision(
        Collider a,
        Collider b,
        out Collision collision)
    {
        Vector2 positionA = a.Position;
        Vector2 positionB = b.Position;

        if (a.Shape is Shapes.CircleShape circleA &&
            b.Shape is Shapes.CircleShape circleB)
        {
            return CircleCircle(
                a,
                b,
                circleA,
                circleB,
                positionA,
                positionB,
                out collision);
        }

        if (a.Shape is Shapes.BoxShape boxA &&
            b.Shape is Shapes.BoxShape boxB)
        {
            return BoxBox(
                a,
                b,
                boxA,
                boxB,
                positionA,
                positionB,
                out collision);
        }

        if (a.Shape is Shapes.CircleShape circle &&
            b.Shape is Shapes.BoxShape box)
        {
            return CircleBox(
                a,
                b,
                circle,
                box,
                positionA,
                positionB,
                out collision);
        }

        if (a.Shape is Shapes.BoxShape boxOther &&
            b.Shape is Shapes.CircleShape circleOther)
        {
            bool result = CircleBox(
                b,
                a,
                circleOther,
                boxOther,
                positionB,
                positionA,
                out Collision reverseCollision);

            if (!result)
            {
                collision = default;
                return false;
            }

            collision = new Collision(
                a,
                b,
                -reverseCollision.Normal,
                reverseCollision.ContactPoint,
                reverseCollision.Penetration);

            return true;
        }

        Bounds boundsA = a.GetBounds();
        Bounds boundsB = b.GetBounds();

        if (!boundsA.Intersects(boundsB))
        {
            collision = default;
            return false;
        }

        Vector2 delta =
            positionB - positionA;

        Vector2 normal =
            delta.LengthSquared >
            MathUtils.Epsilon
                ? delta.Normalized()
                : Vector2.UnitX;

        collision = new Collision(
            a,
            b,
            normal,
            (positionA + positionB) * 0.5f,
            0.0f);

        return true;
    }

    private static bool CircleCircle(
        Collider a,
        Collider b,
        Shapes.CircleShape shapeA,
        Shapes.CircleShape shapeB,
        Vector2 positionA,
        Vector2 positionB,
        out Collision collision)
    {
        Vector2 delta =
            positionB - positionA;

        float distanceSquared =
            delta.LengthSquared;

        float combinedRadius =
            shapeA.Radius +
            shapeB.Radius;

        if (distanceSquared >
            combinedRadius * combinedRadius)
        {
            collision = default;
            return false;
        }

        float distance =
            MathF.Sqrt(distanceSquared);

        Vector2 normal =
            distance > MathUtils.Epsilon
                ? delta / distance
                : Vector2.UnitX;

        float penetration =
            combinedRadius - distance;

        Vector2 contactPoint =
            positionA +
            normal * (shapeA.Radius - penetration * 0.5f);

        collision = new Collision(
            a,
            b,
            normal,
            contactPoint,
            penetration);

        return true;
    }

    private static bool BoxBox(
        Collider a,
        Collider b,
        Shapes.BoxShape shapeA,
        Shapes.BoxShape shapeB,
        Vector2 positionA,
        Vector2 positionB,
        out Collision collision)
    {
        Vector2 halfA = shapeA.HalfSize;
        Vector2 halfB = shapeB.HalfSize;

        float deltaX =
            positionB.X - positionA.X;

        float deltaY =
            positionB.Y - positionA.Y;

        float overlapX =
            halfA.X +
            halfB.X -
            MathF.Abs(deltaX);

        float overlapY =
            halfA.Y +
            halfB.Y -
            MathF.Abs(deltaY);

        if (overlapX <= 0.0f ||
            overlapY <= 0.0f)
        {
            collision = default;
            return false;
        }

        Vector2 normal;
        float penetration;

        if (overlapX < overlapY)
        {
            normal = new Vector2(
                deltaX >= 0.0f ? 1.0f : -1.0f,
                0.0f);

            penetration = overlapX;
        }
        else
        {
            normal = new Vector2(
                0.0f,
                deltaY >= 0.0f ? 1.0f : -1.0f);

            penetration = overlapY;
        }

        Vector2 contactPoint =
            (positionA + positionB) * 0.5f;

        collision = new Collision(
            a,
            b,
            normal,
            contactPoint,
            penetration);

        return true;
    }

    private static bool CircleBox(
        Collider circleCollider,
        Collider boxCollider,
        Shapes.CircleShape circle,
        Shapes.BoxShape box,
        Vector2 circlePosition,
        Vector2 boxPosition,
        out Collision collision)
    {
        Rectangle rectangle =
            box.GetRectangle(boxPosition);

        float closestX =
            MathF.Max(
                rectangle.Left,
                MathF.Min(
                    circlePosition.X,
                    rectangle.Right));

        float closestY =
            MathF.Max(
                rectangle.Top,
                MathF.Min(
                    circlePosition.Y,
                    rectangle.Bottom));

        Vector2 closestPoint =
            new(closestX, closestY);

        Vector2 delta =
            circlePosition -
            closestPoint;

        float distanceSquared =
            delta.LengthSquared;

        if (distanceSquared >
            circle.Radius * circle.Radius)
        {
            collision = default;
            return false;
        }

        Vector2 normal;
        float penetration;

        if (distanceSquared >
            MathUtils.Epsilon)
        {
            float distance =
                MathF.Sqrt(distanceSquared);

            normal =
                -delta / distance;

            penetration =
                circle.Radius - distance;
        }
        else
        {
            Vector2 offset =
                circlePosition - boxPosition;

            float distanceX =
                box.HalfSize.X -
                MathF.Abs(offset.X);

            float distanceY =
                box.HalfSize.Y -
                MathF.Abs(offset.Y);

            if (distanceX < distanceY)
            {
                normal = new Vector2(
                    offset.X >= 0.0f
                        ? 1.0f
                        : -1.0f,
                    0.0f);

                penetration =
                    circle.Radius +
                    distanceX;
            }
            else
            {
                normal = new Vector2(
                    0.0f,
                    offset.Y >= 0.0f
                        ? 1.0f
                        : -1.0f);

                penetration =
                    circle.Radius +
                    distanceY;
            }

            closestPoint =
                circlePosition -
                normal * circle.Radius;
        }

        collision = new Collision(
            circleCollider,
            boxCollider,
            normal,
            closestPoint,
            penetration);

        return true;
    }

    private static bool TryRaycast(
        Ray ray,
        Collider collider,
        float maxDistance,
        out RaycastHit hit)
    {
        if (collider.Shape is Shapes.CircleShape circle)
        {
            return RaycastCircle(
                ray,
                collider,
                circle,
                maxDistance,
                out hit);
        }

        if (collider.Shape is Shapes.BoxShape box)
        {
            return RaycastBox(
                ray,
                collider,
                box,
                maxDistance,
                out hit);
        }

        Bounds bounds =
            collider.GetBounds();

        return RaycastBounds(
            ray,
            collider,
            bounds,
            maxDistance,
            out hit);
    }

    private static bool RaycastCircle(
        Ray ray,
        Collider collider,
        Shapes.CircleShape circle,
        float maxDistance,
        out RaycastHit hit)
    {
        Vector2 offset =
            ray.Origin -
            collider.Position;

        float b =
            Vector2.Dot(
                offset,
                ray.Direction);

        float c =
            offset.LengthSquared -
            circle.Radius *
            circle.Radius;

        float discriminant =
            b * b - c;

        if (discriminant < 0.0f)
        {
            hit = default;
            return false;
        }

        float sqrt =
            MathF.Sqrt(discriminant);

        float distance =
            -b - sqrt;

        if (distance < 0.0f)
        {
            distance =
                -b + sqrt;
        }

        if (distance < 0.0f ||
            distance > maxDistance)
        {
            hit = default;
            return false;
        }

        Vector2 point =
            ray.GetPoint(distance);

        Vector2 normal =
            (point - collider.Position)
            .Normalized();

        hit = new RaycastHit(
            collider,
            point,
            normal,
            distance);

        return true;
    }

    private static bool RaycastBox(
        Ray ray,
        Collider collider,
        Shapes.BoxShape box,
        float maxDistance,
        out RaycastHit hit)
    {
        return RaycastBounds(
            ray,
            collider,
            box.GetBounds(
                collider.Position),
            maxDistance,
            out hit);
    }

    private static bool RaycastBounds(
        Ray ray,
        Collider collider,
        Bounds bounds,
        float maxDistance,
        out RaycastHit hit)
    {
        float tMin = 0.0f;
        float tMax = maxDistance;

        if (!UpdateSlab(
                ray.Origin.X,
                ray.Direction.X,
                bounds.Min.X,
                bounds.Max.X,
                ref tMin,
                ref tMax))
        {
            hit = default;
            return false;
        }

        if (!UpdateSlab(
                ray.Origin.Y,
                ray.Direction.Y,
                bounds.Min.Y,
                bounds.Max.Y,
                ref tMin,
                ref tMax))
        {
            hit = default;
            return false;
        }

        if (tMin < 0.0f ||
            tMin > maxDistance)
        {
            hit = default;
            return false;
        }

        Vector2 point =
            ray.GetPoint(tMin);

        Vector2 normal =
            GetBoundsNormal(
                point,
                bounds);

        hit = new RaycastHit(
            collider,
            point,
            normal,
            tMin);

        return true;
    }

    private static bool UpdateSlab(
        float origin,
        float direction,
        float min,
        float max,
        ref float tMin,
        ref float tMax)
    {
        if (MathF.Abs(direction) <=
            MathUtils.Epsilon)
        {
            return origin >= min &&
                   origin <= max;
        }

        float inverse =
            1.0f / direction;

        float t1 =
            (min - origin) * inverse;

        float t2 =
            (max - origin) * inverse;

        if (t1 > t2)
        {
            (t1, t2) = (t2, t1);
        }

        tMin =
            MathF.Max(tMin, t1);

        tMax =
            MathF.Min(tMax, t2);

        return tMin <= tMax;
    }

    private static Vector2 GetBoundsNormal(
        Vector2 point,
        Bounds bounds)
    {
        float left =
            MathF.Abs(
                point.X - bounds.Min.X);

        float right =
            MathF.Abs(
                point.X - bounds.Max.X);

        float top =
            MathF.Abs(
                point.Y - bounds.Min.Y);

        float bottom =
            MathF.Abs(
                point.Y - bounds.Max.Y);

        float minimum =
            MathF.Min(
                MathF.Min(left, right),
                MathF.Min(top, bottom));

        if (minimum == left)
            return new Vector2(-1.0f, 0.0f);

        if (minimum == right)
            return new Vector2(1.0f, 0.0f);

        if (minimum == top)
            return new Vector2(0.0f, -1.0f);

        return new Vector2(0.0f, 1.0f);
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}