namespace SA2DGE.Engine.Platform;

public static class Platform
{
    public static string Name =>
        OperatingSystem.IsWindows()
            ? "Windows"
            : OperatingSystem.IsLinux()
                ? "Linux"
                : OperatingSystem.IsMacOS()
                    ? "macOS"
                    : "Unknown";

    public static bool IsWindows =>
        OperatingSystem.IsWindows();

    public static bool IsLinux =>
        OperatingSystem.IsLinux();

    public static bool IsMacOS =>
        OperatingSystem.IsMacOS();
}