namespace SA2DGE.Engine.Platform.Input;

public sealed class Mouse
{
    private readonly bool[] _currentState;
    private readonly bool[] _previousState;

    public float X { get; private set; }
    public float Y { get; private set; }
    
    public float WheelDelta { get; private set; }

    public float DeltaX { get; private set; }
    public float DeltaY { get; private set; }

    internal Mouse()
    {
        int buttonCount = Enum.GetValues<MouseButton>().Length;

        _currentState = new bool[buttonCount];
        _previousState = new bool[buttonCount];
    }
    
    internal void AddWheelDelta(float delta)
    {
        WheelDelta += delta;
    }

    public bool IsDown(MouseButton button)
    {
        return IsValidButton(button) &&
               _currentState[(int)button];
    }

    public bool IsPressed(MouseButton button)
    {
        return IsValidButton(button) &&
               _currentState[(int)button] &&
               !_previousState[(int)button];
    }

    public bool IsReleased(MouseButton button)
    {
        return IsValidButton(button) &&
               !_currentState[(int)button] &&
               _previousState[(int)button];
    }

    internal void BeginFrame()
    {
        Array.Copy(
            _currentState,
            _previousState,
            _currentState.Length);

        DeltaX = 0.0f;
        DeltaY = 0.0f;
        WheelDelta = 0.0f;
    }

    internal void SetPosition(float x, float y)
    {
        DeltaX = x - X;
        DeltaY = y - Y;

        X = x;
        Y = y;
    }

    internal void SetButtonState(
        MouseButton button,
        bool isDown)
    {
        if (!IsValidButton(button))
            return;

        _currentState[(int)button] = isDown;
    }

    internal void Clear()
    {
        Array.Clear(_currentState);
        Array.Clear(_previousState);

        X = 0.0f;
        Y = 0.0f;
        DeltaX = 0.0f;
        DeltaY = 0.0f;
        WheelDelta = 0.0f;
    }

    private bool IsValidButton(MouseButton button)
    {
        int index = (int)button;

        return index >= 0 &&
               index < _currentState.Length;
    }
}