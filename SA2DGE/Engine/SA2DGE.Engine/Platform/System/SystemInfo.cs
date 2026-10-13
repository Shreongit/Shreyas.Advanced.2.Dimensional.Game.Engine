namespace SA2DGE.Engine.Platform.System;

public static class SystemInfo
{
    public static string OperatingSystem =>
        global::System.Environment.OSVersion.ToString();

    public static string OperatingSystemName =>
        global::System.Runtime.InteropServices.RuntimeInformation.OSDescription;

    public static string Architecture =>
        global::System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString();

    public static string ProcessArchitecture =>
        global::System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();

    public static int ProcessorCount =>
        global::System.Environment.ProcessorCount;

    public static string RuntimeVersion =>
        global::System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;

    public static string MachineName =>
        global::System.Environment.MachineName;

    public static bool Is64BitProcess =>
        global::System.Environment.Is64BitProcess;

    public static bool Is64BitOperatingSystem =>
        global::System.Environment.Is64BitOperatingSystem;
}