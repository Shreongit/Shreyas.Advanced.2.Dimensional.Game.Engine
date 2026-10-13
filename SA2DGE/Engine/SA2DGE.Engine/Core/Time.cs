using System.Diagnostics;

namespace SA2DGE.Engine.Core;

public static class Time
{
    private static readonly Stopwatch Stopwatch = new();

    private static long _lastTimestamp;

    public static float DeltaTime { get; private set; }

    public static float UnscaledDeltaTime { get; private set; }

    public static double TotalTime { get; private set; }

    public static float TimeScale { get; set; } = 1.0f;

    public static float FramesPerSecond { get; private set; }

    public static void Initialize()
    {
        Stopwatch.Restart();
        _lastTimestamp = Stopwatch.ElapsedTicks;

        DeltaTime = 0.0f;
        UnscaledDeltaTime = 0.0f;
        TotalTime = 0.0;
        FramesPerSecond = 0.0f;
    }

    public static void Update()
    {
        long currentTimestamp = Stopwatch.ElapsedTicks;

        long elapsedTicks = currentTimestamp - _lastTimestamp;
        _lastTimestamp = currentTimestamp;

        UnscaledDeltaTime =
            (float)elapsedTicks / Stopwatch.Frequency;

        DeltaTime = UnscaledDeltaTime * TimeScale;

        TotalTime += UnscaledDeltaTime;

        FramesPerSecond = UnscaledDeltaTime > 0.0f
            ? 1.0f / UnscaledDeltaTime
            : 0.0f;
    }
}