namespace SA2DGE.Engine.ECS;

public abstract class System
{
    public bool Enabled { get; set; } = true;

    public virtual void Initialize(World world)
    {
        ArgumentNullException.ThrowIfNull(world);
    }

    public virtual void Update(
        World world,
        float deltaTime)
    {
        ArgumentNullException.ThrowIfNull(world);
    }

    public virtual void Shutdown(World world)
    {
        ArgumentNullException.ThrowIfNull(world);
    }
}