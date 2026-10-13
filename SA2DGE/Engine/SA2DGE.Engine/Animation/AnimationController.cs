namespace SA2DGE.Engine.Animation;

public sealed class AnimationController
{
    private readonly Dictionary<string, AnimationState> _states =
        new(StringComparer.OrdinalIgnoreCase);

    private AnimationState? _currentState;

    public AnimationState? CurrentState =>
        _currentState;

    public string? CurrentStateName =>
        _currentState?.Name;

    public int StateCount =>
        _states.Count;

    public IReadOnlyDictionary<string, AnimationState> States =>
        _states;

    public bool IsPlaying =>
        _currentState?.IsPlaying ?? false;

    public int CurrentFrame =>
        _currentState?.CurrentFrame ?? -1;

    public void AddState(
        AnimationState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        if (_states.ContainsKey(state.Name))
        {
            throw new InvalidOperationException(
                $"An animation state named '{state.Name}' already exists.");
        }

        _states.Add(
            state.Name,
            state);

        if (_currentState is null)
        {
            _currentState = state;
        }
    }

    public AnimationState AddState(
        string name,
        AnimationClip clip)
    {
        AnimationState state =
            new(name, clip);

        AddState(state);

        return state;
    }

    public bool RemoveState(
        string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!_states.TryGetValue(
                name,
                out AnimationState? state))
        {
            return false;
        }

        if (ReferenceEquals(
                state,
                _currentState))
        {
            _currentState = null;
        }

        return _states.Remove(name);
    }

    public AnimationState GetState(
        string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!_states.TryGetValue(
                name,
                out AnimationState? state))
        {
            throw new KeyNotFoundException(
                $"Animation state '{name}' was not found.");
        }

        return state;
    }

    public bool TryGetState(
        string name,
        out AnimationState? state)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            state = null;
            return false;
        }

        return _states.TryGetValue(
            name,
            out state);
    }

    public bool Play(
        string name,
        bool restart = false)
    {
        AnimationState state =
            GetState(name);

        if (!ReferenceEquals(
                state,
                _currentState))
        {
            _currentState?.Stop();
            _currentState = state;
        }
        else if (restart)
        {
            state.Reset();
        }

        state.Play();

        return true;
    }

    public void Pause()
    {
        _currentState?.Pause();
    }

    public void Stop()
    {
        _currentState?.Stop();
    }

    public void Reset()
    {
        _currentState?.Reset();
    }

    public void Update(
        float deltaTime)
    {
        _currentState?.Update(deltaTime);
    }

    public AnimationFrame GetCurrentFrame()
    {
        if (_currentState is null)
        {
            throw new InvalidOperationException(
                "The animation controller has no current state.");
        }

        return _currentState.GetCurrentFrame();
    }

    public void Clear()
    {
        foreach (AnimationState state in _states.Values)
        {
            state.Stop();
        }

        _states.Clear();
        _currentState = null;
    }
}