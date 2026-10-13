namespace SA2DGE.Engine.Animation;

public sealed class Animation
{
    private readonly Dictionary<string, AnimationClip> _clips =
        new(StringComparer.OrdinalIgnoreCase);

    public string Name { get; }

    public int ClipCount =>
        _clips.Count;

    public IReadOnlyDictionary<string, AnimationClip> Clips =>
        _clips;

    public Animation(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    public void AddClip(AnimationClip clip)
    {
        ArgumentNullException.ThrowIfNull(clip);

        if (_clips.ContainsKey(clip.Name))
        {
            throw new InvalidOperationException(
                $"An animation clip named '{clip.Name}' already exists.");
        }

        _clips.Add(
            clip.Name,
            clip);
    }

    public bool RemoveClip(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return _clips.Remove(name);
    }

    public AnimationClip GetClip(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!_clips.TryGetValue(
                name,
                out AnimationClip? clip))
        {
            throw new KeyNotFoundException(
                $"Animation clip '{name}' was not found.");
        }

        return clip;
    }

    public bool TryGetClip(
        string name,
        out AnimationClip? clip)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            clip = null;
            return false;
        }

        return _clips.TryGetValue(
            name,
            out clip);
    }

    public bool ContainsClip(string name)
    {
        return !string.IsNullOrWhiteSpace(name) &&
               _clips.ContainsKey(name);
    }

    public void Clear()
    {
        _clips.Clear();
    }

    public override string ToString()
    {
        return $"{Name} ({ClipCount} clips)";
    }
}