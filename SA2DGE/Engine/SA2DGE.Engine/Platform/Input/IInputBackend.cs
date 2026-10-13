using SA2DGEWindow = SA2DGE.Engine.Platform.Window.Window;

namespace SA2DGE.Engine.Platform.Input;

public interface IInputBackend : IDisposable
{
    void Initialize(SA2DGEWindow window);

    void Update(Input input);

    void Shutdown();
}