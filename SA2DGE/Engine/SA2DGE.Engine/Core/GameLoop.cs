using System.Diagnostics;
using SA2DGE.Engine.Platform.Window;


namespace SA2DGE.Engine.Core;

public sealed class GameLoop
{
    private const double FixedDeltaTime = 1.0 / 60.0;
    private const double MaximumFrameDelta = 0.25;

    private readonly Game _game;
    private readonly Stopwatch _clock;

    private bool _running;

    public bool IsRunning => _running;

    public GameLoop(Game game)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _clock = new Stopwatch();
    }

    public void Run()
    {
        if (_running)
            throw new InvalidOperationException("Game loop is already running.");

        _game.InitializeRuntime();

        _running = true;
        _clock.Restart();

        double accumulator = 0.0;

        try
        {
            while (_running && _game.Window.IsOpen)
            {
                _game.Window.ProcessEvents();

                while (_game.Window.TryGetEvent(out WindowEvent windowEvent))
                {
                    _game.OnWindowEvent(windowEvent);
                }

                if (!_game.Window.IsOpen)
                    break;

                _game.Input.BeginFrame();
                _game.UpdateInput();

                double frameDelta = _clock.Elapsed.TotalSeconds;
                _clock.Restart();

                if (frameDelta < 0.0)
                    frameDelta = 0.0;

                if (frameDelta > MaximumFrameDelta)
                    frameDelta = MaximumFrameDelta;

                accumulator += frameDelta;

                while (accumulator >= FixedDeltaTime)
                {
                    _game.Update((float)FixedDeltaTime);
                    accumulator -= FixedDeltaTime;
                }

                _game.Render();
            }
        }
        finally
        {
            _running = false;
            _clock.Stop();
            _game.ShutdownRuntime();
        }
    }

    public void Stop()
    {
        _running = false;
    }
}