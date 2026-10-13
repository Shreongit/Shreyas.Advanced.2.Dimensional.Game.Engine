namespace SA2DGE.Engine.Platform.Input;

public static class InputManager
{
    private static readonly HashSet<KeyCode> CurrentKeys = new();
    private static readonly HashSet<KeyCode> PreviousKeys = new();

    private static readonly HashSet<MouseButton> CurrentMouseButtons = new();
    private static readonly HashSet<MouseButton> PreviousMouseButtons = new();

    public static float MouseX { get; internal set; }

    public static float MouseY { get; internal set; }

    public static float MouseDeltaX { get; internal set; }

    public static float MouseDeltaY { get; internal set; }

    public static void Update()
    {
        PreviousKeys.Clear();
        PreviousKeys.UnionWith(CurrentKeys);

        PreviousMouseButtons.Clear();
        PreviousMouseButtons.UnionWith(CurrentMouseButtons);

        MouseDeltaX = 0.0f;
        MouseDeltaY = 0.0f;
    }

    public static bool IsKeyDown(KeyCode key)
    {
        return CurrentKeys.Contains(key);
    }

    public static bool IsKeyPressed(KeyCode key)
    {
        return CurrentKeys.Contains(key) &&
               !PreviousKeys.Contains(key);
    }

    public static bool IsKeyReleased(KeyCode key)
    {
        return !CurrentKeys.Contains(key) &&
               PreviousKeys.Contains(key);
    }

    public static bool IsMouseButtonDown(MouseButton button)
    {
        return CurrentMouseButtons.Contains(button);
    }

    public static bool IsMouseButtonPressed(MouseButton button)
    {
        return CurrentMouseButtons.Contains(button) &&
               !PreviousMouseButtons.Contains(button);
    }

    public static bool IsMouseButtonReleased(MouseButton button)
    {
        return !CurrentMouseButtons.Contains(button) &&
               PreviousMouseButtons.Contains(button);
    }

    internal static void SetKeyState(KeyCode key, bool pressed)
    {
        if (pressed)
        {
            CurrentKeys.Add(key);
        }
        else
        {
            CurrentKeys.Remove(key);
        }
    }

    internal static void SetMouseButtonState(
        MouseButton button,
        bool pressed)
    {
        if (pressed)
        {
            CurrentMouseButtons.Add(button);
        }
        else
        {
            CurrentMouseButtons.Remove(button);
        }
    }

    internal static void SetMousePosition(float x, float y)
    {
        MouseDeltaX = x - MouseX;
        MouseDeltaY = y - MouseY;

        MouseX = x;
        MouseY = y;
    }

    internal static void Clear()
    {
        CurrentKeys.Clear();
        PreviousKeys.Clear();

        CurrentMouseButtons.Clear();
        PreviousMouseButtons.Clear();

        MouseX = 0.0f;
        MouseY = 0.0f;
        MouseDeltaX = 0.0f;
        MouseDeltaY = 0.0f;
    }
}

public enum KeyCode
{
    Unknown,

    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J,
    K,
    L,
    M,
    N,
    O,
    P,
    Q,
    R,
    S,
    T,
    U,
    V,
    W,
    X,
    Y,
    Z,

    Num0,
    Num1,
    Num2,
    Num3,
    Num4,
    Num5,
    Num6,
    Num7,
    Num8,
    Num9,

    Escape,
    Enter,
    Tab,
    Backspace,
    Space,

    Left,
    Right,
    Up,
    Down,

    LeftShift,
    RightShift,
    LeftControl,
    RightControl,
    LeftAlt,
    RightAlt,

    F1,
    F2,
    F3,
    F4,
    F5,
    F6,
    F7,
    F8,
    F9,
    F10,
    F11,
    F12
}

public enum MouseButton
{
    Left,
    Right,
    Middle,
    Button4,
    Button5
}