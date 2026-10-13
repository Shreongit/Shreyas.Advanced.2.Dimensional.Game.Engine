namespace SA2DGE.Engine.Graphics.Fonts;

public abstract class Font : IDisposable
{
    private bool _disposed;

    public string Name { get; }

    public float Size { get; }

    public bool IsLoaded { get; private set; }

    protected Font(
        string name,
        float size)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (size <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(size));
        }

        Name = name;
        Size = size;
    }

    public void Load()
    {
        ThrowIfDisposed();

        if (IsLoaded)
        {
            return;
        }

        OnLoad();

        IsLoaded = true;
    }

    public void Unload()
    {
        if (_disposed || !IsLoaded)
        {
            return;
        }

        OnUnload();

        IsLoaded = false;
    }

    public float MeasureWidth(
        string text)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(text);

        return OnMeasureWidth(text);
    }

    public float MeasureHeight(
        string text)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(text);

        return OnMeasureHeight(text);
    }

    public FontMetrics GetMetrics()
    {
        ThrowIfDisposed();

        return OnGetMetrics();
    }

    protected virtual void OnLoad()
    {
    }

    protected virtual void OnUnload()
    {
    }

    protected abstract float OnMeasureWidth(
        string text);

    protected abstract float OnMeasureHeight(
        string text);

    protected abstract FontMetrics OnGetMetrics();

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Unload();

        OnDispose();

        _disposed = true;
    }

    protected virtual void OnDispose()
    {
    }

    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}

public readonly struct FontMetrics
{
    public float Ascent { get; }

    public float Descent { get; }

    public float LineHeight { get; }

    public float LineGap { get; }

    public FontMetrics(
        float ascent,
        float descent,
        float lineHeight,
        float lineGap = 0.0f)
    {
        Ascent = ascent;
        Descent = descent;
        LineHeight = lineHeight;
        LineGap = lineGap;
    }
}