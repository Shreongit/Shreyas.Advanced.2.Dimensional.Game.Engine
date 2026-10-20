
namespace SA2DGE.Engine.Graphics;

/// <summary>
/// Manages named sprite animations and the currently active animation.
/// </summary>
public sealed class SpriteAnimationController
{
    private readonly Dictionary<string, SpriteAnimation> _animations =
        new(StringComparer.Ordinal);

    public string? CurrentAnimationName { get; private set; }

    public SpriteAnimation? CurrentAnimation { get; private set; }

    public int AnimationCount => _animations.Count;

    /// <summary>
    /// Registers an animation under a unique name.
    /// </summary>
    public void AddAnimation(string name, SpriteAnimation animation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(animation);

        if (!_animations.TryAdd(name, animation))
        {
            throw new ArgumentException(
                $"An animation named '{name}' is already registered.",
                nameof(name));
        }
    }

    /// <summary>
    /// Switches to a named animation and starts it from frame zero.
    /// </summary>
    public void Play(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!_animations.TryGetValue(name, out SpriteAnimation? animation))
        {
            throw new KeyNotFoundException(
                $"No animation named '{name}' has been registered.");
        }

        if (CurrentAnimation is not null)
        {
            CurrentAnimation.Stop();
        }

        CurrentAnimation = animation;
        CurrentAnimationName = name;

        CurrentAnimation.Restart();
    }

    /// <summary>
    /// Updates the currently selected animation.
    /// </summary>
    public void Update(float deltaTime)
    {
        CurrentAnimation?.Update(deltaTime);
    }

    /// <summary>
    /// Pauses the currently selected animation.
    /// </summary>
    public void Pause()
    {
        CurrentAnimation?.Pause();
    }

    /// <summary>
    /// Stops the currently selected animation and resets its frame.
    /// </summary>
    public void Stop()
    {
        CurrentAnimation?.Stop();
    }

    /// <summary>
    /// Returns whether an animation with the specified name exists.
    /// </summary>
    public bool ContainsAnimation(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return _animations.ContainsKey(name);
    }
}
