namespace SA2DGE.Engine.Platform.Window;

public enum WindowEventType
{
    None = 0,
    CloseRequested,
    Closed,
    Resized,
    FocusGained,
    FocusLost,
    Minimized,
    Restored,
    MouseWheel
}

public readonly struct WindowEvent
{
    public WindowEventType Type { get; }

    public int Width { get; }

    public int Height { get; }

    public int WheelDelta { get; }

    public WindowEvent(
        WindowEventType type,
        int width = 0,
        int height = 0,
        int wheelDelta = 0)
    {
        Type = type;
        Width = width;
        Height = height;
        WheelDelta = wheelDelta;
    }

    public static WindowEvent CloseRequested()
    {
        return new WindowEvent(
            WindowEventType.CloseRequested);
    }

    public static WindowEvent Closed()
    {
        return new WindowEvent(
            WindowEventType.Closed);
    }

    public static WindowEvent Resized(
        int width,
        int height)
    {
        return new WindowEvent(
            WindowEventType.Resized,
            width,
            height);
    }

    public static WindowEvent FocusGained()
    {
        return new WindowEvent(
            WindowEventType.FocusGained);
    }

    public static WindowEvent FocusLost()
    {
        return new WindowEvent(
            WindowEventType.FocusLost);
    }

    public static WindowEvent Minimized()
    {
        return new WindowEvent(
            WindowEventType.Minimized);
    }

    public static WindowEvent Restored(
        int width,
        int height)
    {
        return new WindowEvent(
            WindowEventType.Restored,
            width,
            height);
    }

    public static WindowEvent MouseWheel(
        int wheelDelta)
    {
        return new WindowEvent(
            WindowEventType.MouseWheel,
            wheelDelta: wheelDelta);
    }
}