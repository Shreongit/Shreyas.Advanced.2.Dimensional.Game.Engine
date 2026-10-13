namespace SA2DGE.Engine.ECS;

public sealed class World : IDisposable
{
    private readonly Dictionary<Type, object> _componentPools = new();
    private readonly Dictionary<EntityId, Entity> _entities = new();
    private readonly HashSet<EntityId> _aliveEntities = new();

    private uint _nextEntityId = 1;
    private bool _disposed;

    public int EntityCount => _aliveEntities.Count;

    public Entity CreateEntity()
    {
        ThrowIfDisposed();

        if (_nextEntityId == uint.MaxValue)
        {
            throw new InvalidOperationException(
                "The world has exhausted its available entity IDs.");
        }

        EntityId id = new(_nextEntityId++);

        Entity entity = new(this, id);

        _entities.Add(id, entity);
        _aliveEntities.Add(id);

        return entity;
    }

    public bool IsEntityAlive(EntityId id)
    {
        return !_disposed &&
               id.IsValid &&
               _aliveEntities.Contains(id);
    }

    public Entity GetEntity(EntityId id)
    {
        ThrowIfDisposed();

        if (!_entities.TryGetValue(id, out Entity? entity) ||
            !_aliveEntities.Contains(id))
        {
            throw new KeyNotFoundException(
                $"Entity {id} does not exist.");
        }

        return entity;
    }

    public void DestroyEntity(EntityId id)
    {
        ThrowIfDisposed();

        if (!_aliveEntities.Remove(id))
        {
            return;
        }

        foreach (object pool in _componentPools.Values)
        {
            RemoveComponentFromPool(pool, id);
        }
    }

    public void AddComponent<T>(
        EntityId entity,
        T component)
        where T : Component
    {
        ThrowIfDisposed();
        EnsureEntityAlive(entity);
        ArgumentNullException.ThrowIfNull(component);

        ComponentPool<T> pool = GetOrCreatePool<T>();

        pool.Add(entity, component);
    }

    public void SetComponent<T>(
        EntityId entity,
        T component)
        where T : Component
    {
        ThrowIfDisposed();
        EnsureEntityAlive(entity);
        ArgumentNullException.ThrowIfNull(component);

        ComponentPool<T> pool = GetOrCreatePool<T>();

        pool.Set(entity, component);
    }

    public T GetComponent<T>(
        EntityId entity)
        where T : Component
    {
        ThrowIfDisposed();
        EnsureEntityAlive(entity);

        ComponentPool<T> pool = GetOrCreatePool<T>();

        return pool.Get(entity);
    }

    public bool TryGetComponent<T>(
        EntityId entity,
        out T? component)
        where T : Component
    {
        if (_disposed ||
            !IsEntityAlive(entity))
        {
            component = null;
            return false;
        }

        if (!_componentPools.TryGetValue(
                typeof(T),
                out object? value))
        {
            component = null;
            return false;
        }

        ComponentPool<T> pool = (ComponentPool<T>)value;

        return pool.TryGet(
            entity,
            out component);
    }

    public bool HasComponent<T>(
        EntityId entity)
        where T : Component
    {
        if (_disposed ||
            !IsEntityAlive(entity))
        {
            return false;
        }

        if (!_componentPools.TryGetValue(
                typeof(T),
                out object? value))
        {
            return false;
        }

        ComponentPool<T> pool = (ComponentPool<T>)value;

        return pool.Has(entity);
    }

    public bool RemoveComponent<T>(
        EntityId entity)
        where T : Component
    {
        ThrowIfDisposed();
        EnsureEntityAlive(entity);

        if (!_componentPools.TryGetValue(
                typeof(T),
                out object? value))
        {
            return false;
        }

        ComponentPool<T> pool = (ComponentPool<T>)value;

        return pool.Remove(entity);
    }

    public IEnumerable<Entity> GetEntities()
    {
        ThrowIfDisposed();

        foreach (EntityId id in _aliveEntities)
        {
            yield return _entities[id];
        }
    }

    public ComponentPool<T> GetComponentPool<T>()
        where T : Component
    {
        ThrowIfDisposed();

        return GetOrCreatePool<T>();
    }

    public void Clear()
    {
        ThrowIfDisposed();

        foreach (object pool in _componentPools.Values)
        {
            ClearComponentPool(pool);
        }

        _aliveEntities.Clear();
        _entities.Clear();

        _nextEntityId = 1;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Clear();

        _componentPools.Clear();

        _disposed = true;
    }

    private ComponentPool<T> GetOrCreatePool<T>()
        where T : Component
    {
        Type type = typeof(T);

        if (_componentPools.TryGetValue(
                type,
                out object? existing))
        {
            return (ComponentPool<T>)existing;
        }

        ComponentPool<T> pool = new();

        _componentPools.Add(type, pool);

        return pool;
    }

    private void EnsureEntityAlive(EntityId id)
    {
        if (!IsEntityAlive(id))
        {
            throw new InvalidOperationException(
                $"Entity {id} is not alive in this world.");
        }
    }

    private static void RemoveComponentFromPool(
        object pool,
        EntityId entity)
    {
        var method = pool.GetType().GetMethod(
            nameof(ComponentPool<Component>.Remove));

        method?.Invoke(
            pool,
            new object[] { entity });
    }

    private static void ClearComponentPool(object pool)
    {
        var method = pool.GetType().GetMethod(
            nameof(ComponentPool<Component>.Clear));

        method?.Invoke(pool, null);
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}