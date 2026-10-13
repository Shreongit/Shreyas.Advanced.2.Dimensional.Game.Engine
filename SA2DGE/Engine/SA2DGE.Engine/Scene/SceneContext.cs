using SA2DGE.Engine.ECS;

namespace SA2DGE.Engine.Scene;

public sealed class SceneContext
{
    public Scene Scene { get; }

    public World World =>
        Scene.World;

    public string Name =>
        Scene.Name;

    public bool IsInitialized =>
        Scene.IsInitialized;

    public SceneContext(Scene scene)
    {
        ArgumentNullException.ThrowIfNull(scene);

        Scene = scene;
    }

    public Entity CreateEntity()
    {
        return World.CreateEntity();
    }

    public Entity GetEntity(EntityId id)
    {
        return World.GetEntity(id);
    }

    public IEnumerable<Entity> GetEntities()
    {
        return World.GetEntities();
    }

    public EntityQuery Query<T>()
        where T : Component
    {
        return new EntityQuery(
            World,
            typeof(T));
    }

    public EntityQuery Query<T1, T2>()
        where T1 : Component
        where T2 : Component
    {
        return new EntityQuery(
            World,
            typeof(T1),
            typeof(T2));
    }

    public EntityQuery Query<T1, T2, T3>()
        where T1 : Component
        where T2 : Component
        where T3 : Component
    {
        return new EntityQuery(
            World,
            typeof(T1),
            typeof(T2),
            typeof(T3));
    }
}