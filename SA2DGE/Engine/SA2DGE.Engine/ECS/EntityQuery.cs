namespace SA2DGE.Engine.ECS;

public sealed class EntityQuery
{
    private readonly World _world;
    private readonly Type[] _componentTypes;

    internal EntityQuery(
        World world,
        params Type[] componentTypes)
    {
        ArgumentNullException.ThrowIfNull(world);
        ArgumentNullException.ThrowIfNull(componentTypes);

        if (componentTypes.Length == 0)
        {
            throw new ArgumentException(
                "A query must contain at least one component type.",
                nameof(componentTypes));
        }

        foreach (Type componentType in componentTypes)
        {
            ArgumentNullException.ThrowIfNull(componentType);

            if (!typeof(Component).IsAssignableFrom(componentType))
            {
                throw new ArgumentException(
                    $"Type '{componentType.FullName}' is not a Component.",
                    nameof(componentTypes));
            }
        }

        _world = world;
        _componentTypes = componentTypes;
    }

    public IEnumerable<Entity> GetEntities()
    {
        foreach (Entity entity in _world.GetEntities())
        {
            if (Matches(entity.Id))
            {
                yield return entity;
            }
        }
    }

    public bool Matches(EntityId entity)
    {
        if (!_world.IsEntityAlive(entity))
        {
            return false;
        }

        foreach (Type componentType in _componentTypes)
        {
            if (!HasComponent(entity, componentType))
            {
                return false;
            }
        }

        return true;
    }

    private bool HasComponent(
        EntityId entity,
        Type componentType)
    {
        var method = typeof(World).GetMethod(
            nameof(World.HasComponent));

        if (method is null)
        {
            return false;
        }

        var genericMethod = method.MakeGenericMethod(
            componentType);

        return (bool)genericMethod.Invoke(
            _world,
            new object[] { entity })!;
    }
}