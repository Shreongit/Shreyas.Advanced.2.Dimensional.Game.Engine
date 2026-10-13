namespace SA2DGE.Engine.Platform.Input;

public sealed class Keyboard
{
    private readonly bool[] _currentState;
    private readonly bool[] _previousState;

    internal Keyboard()
    {
        int keyCount = Enum.GetValues<InputKey>().Length;

        _currentState = new bool[keyCount];
        _previousState = new bool[keyCount];
    }

    public bool IsDown(InputKey key)
    {
        return IsValidKey(key) &&
               _currentState[(int)key];
    }

    public bool IsPressed(InputKey key)
    {
        return IsValidKey(key) &&
               _currentState[(int)key] &&
               !_previousState[(int)key];
    }

    public bool IsReleased(InputKey key)
    {
        return IsValidKey(key) &&
               !_currentState[(int)key] &&
               _previousState[(int)key];
    }

    internal void BeginFrame()
    {
        Array.Copy(
            _currentState,
            _previousState,
            _currentState.Length);
    }

    internal void SetKeyState(
        InputKey key,
        bool isDown)
    {
        if (!IsValidKey(key))
            return;

        _currentState[(int)key] = isDown;
    }

    internal void Clear()
    {
        Array.Clear(_currentState);
        Array.Clear(_previousState);
    }

    private bool IsValidKey(InputKey key)
    {
        int index = (int)key;

        return index >= 0 &&
               index < _currentState.Length &&
               key != InputKey.Unknown;
    }
}