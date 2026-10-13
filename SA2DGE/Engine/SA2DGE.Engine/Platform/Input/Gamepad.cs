namespace SA2DGE.Engine.Platform.Input;

public static class Gamepad
{
    private static readonly HashSet<GamepadButton> ConnectedButtons = new();

    public static bool IsConnected { get; internal set; }

    public static float LeftStickX { get; internal set; }

    public static float LeftStickY { get; internal set; }

    public static float RightStickX { get; internal set; }

    public static float RightStickY { get; internal set; }

    public static float LeftTrigger { get; internal set; }

    public static float RightTrigger { get; internal set; }

    public static bool IsDown(GamepadButton button)
    {
        return ConnectedButtons.Contains(button);
    }

    internal static void SetButtonState(
        GamepadButton button,
        bool pressed)
    {
        if (pressed)
        {
            ConnectedButtons.Add(button);
        }
        else
        {
            ConnectedButtons.Remove(button);
        }
    }

    internal static void Clear()
    {
        ConnectedButtons.Clear();

        IsConnected = false;

        LeftStickX = 0.0f;
        LeftStickY = 0.0f;
        RightStickX = 0.0f;
        RightStickY = 0.0f;
        LeftTrigger = 0.0f;
        RightTrigger = 0.0f;
    }
}

public enum GamepadButton
{
    A,
    B,
    X,
    Y,

    LeftShoulder,
    RightShoulder,

    LeftStick,
    RightStick,

    Start,
    Back,

    DPadUp,
    DPadDown,
    DPadLeft,
    DPadRight
}