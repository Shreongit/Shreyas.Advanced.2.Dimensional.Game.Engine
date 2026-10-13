namespace SA2DGE.Engine.Scripting;

public sealed class ScriptComponent : ECS.Component
{
    private readonly List<Script> _scripts = new();

    public IReadOnlyList<Script> Scripts =>
        _scripts;

    public int Count =>
        _scripts.Count;

    public bool Add(Script script)
    {
        ArgumentNullException.ThrowIfNull(script);

        if (_scripts.Contains(script))
        {
            return false;
        }

        if (script.IsAttached)
        {
            throw new InvalidOperationException(
                "Script is already attached to an entity.");
        }

        _scripts.Add(script);
        return true;
    }

    public T Add<T>()
        where T : Script, new()
    {
        T script = new();

        Add(script);

        return script;
    }

    public bool Remove(Script script)
    {
        ArgumentNullException.ThrowIfNull(script);

        if (!_scripts.Remove(script))
        {
            return false;
        }

        script.Shutdown();
        script.Detach();

        return true;
    }

    public bool Remove<T>()
        where T : Script
    {
        for (int i = 0; i < _scripts.Count; i++)
        {
            if (_scripts[i] is not T script)
            {
                continue;
            }

            _scripts.RemoveAt(i);

            script.Shutdown();
            script.Detach();

            return true;
        }

        return false;
    }

    public T Get<T>()
        where T : Script
    {
        foreach (Script script in _scripts)
        {
            if (script is T typedScript)
            {
                return typedScript;
            }
        }

        throw new KeyNotFoundException(
            $"Script of type '{typeof(T).Name}' was not found.");
    }

    public bool TryGet<T>(
        out T? script)
        where T : Script
    {
        foreach (Script candidate in _scripts)
        {
            if (candidate is T typedScript)
            {
                script = typedScript;
                return true;
            }
        }

        script = null;
        return false;
    }

    internal void AttachAll(
        ECS.Entity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        foreach (Script script in _scripts)
        {
            if (!script.IsAttached)
            {
                script.Attach(entity);
                script.Initialize();
            }
        }
    }

    internal void UpdateAll(
        float deltaTime)
    {
        foreach (Script script in _scripts)
        {
            if (script.Enabled &&
                script.IsAttached)
            {
                script.Update(deltaTime);
            }
        }
    }

    internal void FixedUpdateAll(
        float deltaTime)
    {
        foreach (Script script in _scripts)
        {
            if (script.Enabled &&
                script.IsAttached)
            {
                script.FixedUpdate(deltaTime);
            }
        }
    }

    internal void LateUpdateAll(
        float deltaTime)
    {
        foreach (Script script in _scripts)
        {
            if (script.Enabled &&
                script.IsAttached)
            {
                script.LateUpdate(deltaTime);
            }
        }
    }

    internal void ShutdownAll()
    {
        foreach (Script script in _scripts)
        {
            if (script.IsAttached)
            {
                script.Shutdown();
                script.Detach();
            }
        }
    }

    public void Clear()
    {
        ShutdownAll();
        _scripts.Clear();
    }
}