namespace SA2DGE.Engine.Core;

public static class Engine
{
    private static Application? _application;

    public static bool IsRunning =>
        _application?.IsRunning == true;

    public static Game? CurrentGame =>
        _application?.Game;

    public static Application? CurrentApplication =>
        _application;

    public static void Run(
        Game game)
    {
        ArgumentNullException.ThrowIfNull(game);

        if (_application is not null)
        {
            throw new InvalidOperationException(
                "SA2DGE is already running.");
        }

        Application application =
            new(game);

        _application = application;

        try
        {
            application.Run();
        }
        finally
        {
            application.Dispose();
            _application = null;
        }
    }

    public static void Stop()
    {
        _application?.Stop();
    }
}