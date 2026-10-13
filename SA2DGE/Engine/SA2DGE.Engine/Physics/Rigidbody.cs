using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Physics;

public sealed class Rigidbody
{
    public Vector2 Position { get; set; }

    public Vector2 Velocity { get; set; }

    public Vector2 Force { get; private set; }

    public float Rotation { get; set; }

    public float AngularVelocity { get; set; }

    public float Torque { get; private set; }

    public float Mass { get; private set; }

    public float InverseMass =>
        IsStatic || Mass <= 0.0f
            ? 0.0f
            : 1.0f / Mass;

    public float GravityScale { get; set; } = 1.0f;

    public float LinearDamping { get; set; } = 0.0f;

    public float AngularDamping { get; set; } = 0.0f;

    public bool IsStatic { get; private set; }

    public bool IsKinematic { get; private set; }

    public bool IsAwake { get; private set; } = true;

    public PhysicsMaterial Material { get; set; }

    public Rigidbody(
        float mass = 1.0f,
        bool isStatic = false)
    {
        if (mass <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(mass));
        }

        Mass = mass;
        IsStatic = isStatic;
        Material = PhysicsMaterial.Default;
    }

    public void SetMass(float mass)
    {
        if (mass <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(mass));
        }

        Mass = mass;
    }

    public void SetStatic(bool isStatic)
    {
        IsStatic = isStatic;

        if (isStatic)
        {
            Velocity = Vector2.Zero;
            AngularVelocity = 0.0f;
            ClearForces();
        }

        Wake();
    }

    public void SetKinematic(bool isKinematic)
    {
        IsKinematic = isKinematic;

        if (isKinematic)
        {
            ClearForces();
        }

        Wake();
    }

    public void AddForce(Vector2 force)
    {
        if (IsStatic || IsKinematic)
        {
            return;
        }

        Force += force;
        Wake();
    }

    public void AddImpulse(Vector2 impulse)
    {
        if (IsStatic || IsKinematic)
        {
            return;
        }

        Velocity += impulse * InverseMass;
        Wake();
    }

    public void AddTorque(float torque)
    {
        if (IsStatic || IsKinematic)
        {
            return;
        }

        Torque += torque;
        Wake();
    }

    public void AddAngularImpulse(float impulse)
    {
        if (IsStatic || IsKinematic)
        {
            return;
        }

        AngularVelocity += impulse * InverseMass;
        Wake();
    }

    public void Integrate(
        Vector2 gravity,
        float deltaTime)
    {
        if (deltaTime < 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(deltaTime));
        }

        if (deltaTime == 0.0f ||
            IsStatic ||
            IsKinematic ||
            !IsAwake)
        {
            return;
        }

        Vector2 acceleration =
            gravity * GravityScale +
            Force * InverseMass;

        Velocity += acceleration * deltaTime;

        float linearDampingFactor =
            MathF.Max(
                0.0f,
                1.0f - LinearDamping * deltaTime);

        Velocity *= linearDampingFactor;

        Position += Velocity * deltaTime;

        float angularAcceleration =
            Torque * InverseMass;

        AngularVelocity +=
            angularAcceleration * deltaTime;

        float angularDampingFactor =
            MathF.Max(
                0.0f,
                1.0f - AngularDamping * deltaTime);

        AngularVelocity *= angularDampingFactor;

        Rotation +=
            AngularVelocity * deltaTime;

        ClearForces();
    }

    public void ClearForces()
    {
        Force = Vector2.Zero;
        Torque = 0.0f;
    }

    public void Wake()
    {
        IsAwake = true;
    }

    public void Sleep()
    {
        IsAwake = false;
        Velocity = Vector2.Zero;
        AngularVelocity = 0.0f;
        ClearForces();
    }

    public void Move(
        Vector2 position)
    {
        Position = position;
        Wake();
    }

    public void Rotate(
        float rotation)
    {
        Rotation = rotation;
        Wake();
    }
}