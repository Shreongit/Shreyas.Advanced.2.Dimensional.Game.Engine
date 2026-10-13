using System.Text.Json;
using System.Text.Json.Serialization;
using SA2DGE.Engine.ECS;

namespace SA2DGE.Engine.Scene;

public sealed class SceneSerializer
{
    private readonly JsonSerializerOptions _options;

    public SceneSerializer()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            IncludeFields = true,
            DefaultIgnoreCondition =
                JsonIgnoreCondition.WhenWritingNull
        };
    }

    public string Serialize(Scene scene)
    {
        ArgumentNullException.ThrowIfNull(scene);

        SceneData data = CreateSceneData(scene);

        return JsonSerializer.Serialize(
            data,
            _options);
    }

    public void Serialize(
        Scene scene,
        string path)
    {
        ArgumentNullException.ThrowIfNull(scene);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = Serialize(scene);

        File.WriteAllText(path, json);
    }

    public Scene Deserialize(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        SceneData? data =
            JsonSerializer.Deserialize<SceneData>(
                json,
                _options);

        if (data is null)
        {
            throw new InvalidOperationException(
                "The scene data could not be deserialized.");
        }

        return CreateScene(data);
    }

    public Scene DeserializeFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "Scene file was not found.",
                path);
        }

        string json = File.ReadAllText(path);

        return Deserialize(json);
    }

    private static SceneData CreateSceneData(Scene scene)
    {
        SceneData data = new()
        {
            Name = scene.Name
        };

        foreach (Entity entity in scene.World.GetEntities())
        {
            data.Entities.Add(
                new EntityData
                {
                    Id = entity.Id.Value
                });
        }

        return data;
    }

    private static Scene CreateScene(SceneData data)
    {
        if (string.IsNullOrWhiteSpace(data.Name))
        {
            throw new InvalidOperationException(
                "Scene data must contain a valid name.");
        }

        Scene scene = new(data.Name);

        foreach (EntityData entityData in data.Entities)
        {
            if (entityData.Id == 0)
            {
                continue;
            }

            scene.World.CreateEntity();
        }

        return scene;
    }

    private sealed class SceneData
    {
        public string Name { get; set; } = string.Empty;

        public List<EntityData> Entities { get; set; } = new();
    }

    private sealed class EntityData
    {
        public uint Id { get; set; }
    }
}