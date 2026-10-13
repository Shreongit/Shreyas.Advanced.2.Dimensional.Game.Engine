namespace SA2DGE.Engine.Core;

public sealed class EngineConfig
{
    public string ApplicationName { get; set; } = "SA2DGE Application";

    public int WindowWidth { get; set; } = 1280;

    public int WindowHeight { get; set; } = 720;

    public bool VSync { get; set; } = true;

    public bool Resizable { get; set; } = true;

    public bool Fullscreen { get; set; } = false;

    public int TargetFrameRate { get; set; } = 60;
}