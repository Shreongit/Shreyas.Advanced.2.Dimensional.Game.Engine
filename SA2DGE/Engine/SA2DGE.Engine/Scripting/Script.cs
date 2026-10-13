using SA2DGE.Engine.ECS;

namespace SA2DGE.Engine.Scripting;

public abstract class Script
{
    public Entity? Entity { get; private set; }

    public World? World =>
        Entity?.World;

    public bool IsAttached =>
        Entity is not null &&
        Entity.IsValid;

    public bool Enabled { get; set; } = true;

    internal void Attach(
        Entity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (Entity is not null)
        {
            throw new InvalidOperationException(
                "Script is already attached to an entity.");
        }

        Entity = entity;

        OnAttach();
    }

    internal void Detach()
    {
        if (Entity is null)
        {
            return;
        }

        OnDetach();

        Entity = null;
    }

    public virtual void Initialize()
    {
    }

    public virtual void Update(
        float deltaTime)
    {
    }

    public virtual void FixedUpdate(
        float deltaTime)
    {
    }

    public virtual void LateUpdate(
        float deltaTime)
    {
    }

    public virtual void Shutdown()
    {
    }

    protected virtual void OnAttach()
    {
    }

    protected virtual void OnDetach()
    {
    }

    protected void RequireEntity()
    {
        if (!IsAttached)
        {
            throw new InvalidOperationException(
                "Script is not attached to a valid entity.");
        }
    }

    protected World RequireWorld()
    {
        RequireEntity();

        return Entity!.World;
    }

    protected T GetComponent<T>()
        where T : Component
    {
        RequireEntity();

        return Entity!.Get<T>();
    }

    protected bool TryGetComponent<T>(
        out T? component)
        where T : Component
    {
        if (!IsAttached)
        {
            component = null;
            return false;
        }

        return Entity!.TryGet(
            out component);
    }

    protected Entity CreateEntity()
    {
        return RequireWorld()
            .CreateEntity();
    }
}