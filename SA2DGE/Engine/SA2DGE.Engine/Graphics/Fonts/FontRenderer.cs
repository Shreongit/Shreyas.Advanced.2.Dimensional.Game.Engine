using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics.Fonts;

public abstract class FontRenderer
{
    private bool _begun;

    public bool IsRendering =>
        _begun;

    public void Begin()
    {
        if (_begun)
        {
            throw new InvalidOperationException(
                "Font rendering has already begun.");
        }

        OnBegin();

        _begun = true;
    }

    public void Draw(Text text)
    {
        ArgumentNullException.ThrowIfNull(text);

        EnsureRendering();

        if (text.Font is null)
        {
            throw new InvalidOperationException(
                "Text must have a font assigned before it can be rendered.");
        }

        if (!text.Font.IsLoaded)
        {
            throw new InvalidOperationException(
                $"Font '{text.Font.Name}' must be loaded before rendering.");
        }

        OnDraw(text);
    }

    public void Draw(
        Font font,
        string content,
        Vector2 position,
        Color color,
        float size = 0.0f,
        int layer = 0)
    {
        ArgumentNullException.ThrowIfNull(font);
        ArgumentNullException.ThrowIfNull(content);

        EnsureRendering();

        if (!font.IsLoaded)
        {
            throw new InvalidOperationException(
                $"Font '{font.Name}' must be loaded before rendering.");
        }

        Text text = new(
            content,
            font)
        {
            Position = position,
            Color = color,
            Size = size > 0.0f
                ? size
                : font.Size,
            Layer = layer
        };

        OnDraw(text);
    }

    public void End()
    {
        EnsureRendering();

        try
        {
            OnEnd();
        }
        finally
        {
            _begun = false;
        }
    }

    protected virtual void OnBegin()
    {
    }

    protected abstract void OnDraw(Text text);

    protected virtual void OnEnd()
    {
    }

    private void EnsureRendering()
    {
        if (!_begun)
        {
            throw new InvalidOperationException(
                "Begin() must be called before rendering text.");
        }
    }
}