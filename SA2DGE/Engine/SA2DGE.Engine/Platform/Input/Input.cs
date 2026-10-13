namespace SA2DGE.Engine.Platform.Input;

public sealed class Input
{
    public Keyboard Keyboard { get; }
    public Mouse Mouse { get; }

    internal Input()
    {
        Keyboard = new Keyboard();
        Mouse = new Mouse();
    }

    internal void BeginFrame()
    {
        Keyboard.BeginFrame();
        Mouse.BeginFrame();
    }

    internal void SetKeyState(
        InputKey key,
        bool isDown)
    {
        Keyboard.SetKeyState(
            key,
            isDown);
    }

    internal void SetMousePosition(
        float x,
        float y)
    {
        Mouse.SetPosition(x, y);
    }

    internal void SetMouseButtonState(
        MouseButton button,
        bool isDown)
    {
        Mouse.SetButtonState(
            button,
            isDown);
    }

    internal void Clear()
    {
        Keyboard.Clear();
        Mouse.Clear();
    }
    
    internal void AddMouseWheelDelta(float delta)
    {
        Mouse.AddWheelDelta(delta);
    }
}