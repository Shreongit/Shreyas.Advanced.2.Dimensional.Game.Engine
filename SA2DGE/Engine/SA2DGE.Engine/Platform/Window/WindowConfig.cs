namespace SA2DGE.Engine.Platform.Window;

public sealed class WindowConfig
{
    public string Title { get; set; } = "SA2DGE";

    public int Width { get; set; } = 1280;

    public int Height { get; set; } = 720;

    public bool VSync { get; set; } = true;

    public bool Resizable { get; set; } = true;

    public bool Fullscreen { get; set; } = false;

    public bool Borderless { get; set; } = false;

    public bool Visible { get; set; } = true;
}