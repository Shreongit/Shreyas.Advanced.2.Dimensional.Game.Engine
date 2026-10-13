namespace SA2DGE.Engine.Core;

public sealed class Application : IDisposable
{
    private readonly GameLoop _gameLoop;

    private bool _running;
    private bool _disposed;

    public Game Game { get; }

    public bool IsRunning => _running;

    public Application(Game game)
    {
        ArgumentNullException.ThrowIfNull(game);

        Game = game;
        _gameLoop = new GameLoop(game);
    }

    public void Run()
    {
        ThrowIfDisposed();

        if (_running)
            throw new InvalidOperationException(
                "Application is already running.");

        _running = true;

        try
        {
            _gameLoop.Run();
        }
        finally
        {
            _running = false;
        }
    }

    public void Stop()
    {
        if (_disposed)
            return;

        _gameLoop.Stop();
        _running = false;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            Stop();
        }
        finally
        {
            Game.Dispose();
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}