using SA2DGE.Engine.ECS;

namespace SA2DGE.Engine.Scripting;

public sealed class ScriptManager
{
    private readonly List<ScriptComponent> _components = new();

    private World? _world;
    private bool _initialized;

    public World? World =>
        _world;

    public bool IsInitialized =>
        _initialized;

    public int ComponentCount =>
        _components.Count;

    public IReadOnlyList<ScriptComponent> Components =>
        _components;

    public void Initialize(
        World world)
    {
        ArgumentNullException.ThrowIfNull(world);

        if (_initialized)
        {
            return;
        }

        _world = world;
        _initialized = true;
    }

    public void Register(
        ScriptComponent component,
        Entity entity)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentNullException.ThrowIfNull(entity);

        EnsureInitialized();

        if (!entity.IsValid)
        {
            throw new InvalidOperationException(
                "Cannot register scripts for an invalid entity.");
        }

        if (!_components.Contains(component))
        {
            _components.Add(component);
        }

        component.AttachAll(entity);
    }

    public bool Unregister(
        ScriptComponent component)
    {
        ArgumentNullException.ThrowIfNull(component);

        if (!_components.Remove(component))
        {
            return false;
        }

        component.ShutdownAll();

        return true;
    }

    public void Update(
        float deltaTime)
    {
        EnsureInitialized();

        foreach (ScriptComponent component in _components)
        {
            component.UpdateAll(deltaTime);
        }
    }

    public void FixedUpdate(
        float deltaTime)
    {
        EnsureInitialized();

        foreach (ScriptComponent component in _components)
        {
            component.FixedUpdateAll(deltaTime);
        }
    }

    public void LateUpdate(
        float deltaTime)
    {
        EnsureInitialized();

        foreach (ScriptComponent component in _components)
        {
            component.LateUpdateAll(deltaTime);
        }
    }

    public void Shutdown()
    {
        if (!_initialized)
        {
            return;
        }

        foreach (ScriptComponent component in _components)
        {
            component.ShutdownAll();
        }

        _components.Clear();
        _world = null;
        _initialized = false;
    }

    private void EnsureInitialized()
    {
        if (!_initialized ||
            _world is null)
        {
            throw new InvalidOperationException(
                "Script manager has not been initialized.");
        }
    }
}