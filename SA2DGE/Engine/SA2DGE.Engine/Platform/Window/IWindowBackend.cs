namespace SA2DGE.Engine.Platform.Window;

public interface IWindowBackend : IDisposable
{
    bool IsOpen { get; }

    int Width { get; }

    int Height { get; }

    string Title { get; }

    internal nint NativeHandle { get; }

    void Open();

    void Close();

    void ProcessEvents();

    bool TryGetEvent(out WindowEvent windowEvent);

    void Resize(int width, int height);

    void SetTitle(string title);

    void SetVSync(bool enabled);

    void SetResizable(bool enabled);

    void SetFullscreen(bool enabled);

    void SetBorderless(bool enabled);
}