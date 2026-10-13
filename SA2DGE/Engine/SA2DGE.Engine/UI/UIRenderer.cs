using SA2DGE.Engine.Graphics.Fonts;
using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.UI;

public abstract class UIRenderer
{
    private bool _rendering;

    public bool IsRendering =>
        _rendering;

    public void Begin()
    {
        if (_rendering)
        {
            throw new InvalidOperationException(
                "UI rendering has already begun.");
        }

        OnBegin();
        _rendering = true;
    }

    public void Draw(
        UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        EnsureRendering();

        if (!element.Visible)
        {
            return;
        }

        RenderElement(element);
    }

    public void Draw(
        UIContainer container)
    {
        ArgumentNullException.ThrowIfNull(container);
        EnsureRendering();

        if (!container.Visible)
        {
            return;
        }

        RenderElement(container);

        foreach (UIElement child in container.Children)
        {
            Draw(child);
        }
    }

    public void DrawLabel(
        UILabel label)
    {
        ArgumentNullException.ThrowIfNull(label);
        EnsureRendering();

        if (!label.Visible)
        {
            return;
        }

        RenderLabel(
            label,
            label.Text,
            label.TextColor);
    }

    public void DrawButton(
        UIButton button)
    {
        ArgumentNullException.ThrowIfNull(button);
        EnsureRendering();

        if (!button.Visible)
        {
            return;
        }

        RenderButton(
            button,
            button.GetCurrentColor());
    }

    public void DrawPanel(
        UIPanel panel)
    {
        ArgumentNullException.ThrowIfNull(panel);
        EnsureRendering();

        if (!panel.Visible)
        {
            return;
        }

        RenderPanel(
            panel,
            panel.BackgroundColor,
            panel.BorderColor,
            panel.BorderThickness,
            panel.HasBorder);

        foreach (UIElement child in panel.Children)
        {
            Draw(child);
        }
    }

    public void DrawInput(
        UIInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        EnsureRendering();

        if (!input.Visible)
        {
            return;
        }

        RenderInput(
            input,
            input.DisplayText,
            input.DisplayColor,
            input.IsFocused,
            input.CaretPosition);
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
            _rendering = false;
        }
    }

    protected virtual void OnBegin()
    {
    }

    protected virtual void OnEnd()
    {
    }

    protected abstract void RenderElement(
        UIElement element);

    protected abstract void RenderLabel(
        UILabel label,
        string text,
        Color color);

    protected abstract void RenderButton(
        UIButton button,
        Color color);

    protected abstract void RenderPanel(
        UIPanel panel,
        Color backgroundColor,
        Color borderColor,
        float borderThickness,
        bool hasBorder);

    protected abstract void RenderInput(
        UIInput input,
        string text,
        Color color,
        bool focused,
        int caretPosition);

    private void EnsureRendering()
    {
        if (!_rendering)
        {
            throw new InvalidOperationException(
                "Begin() must be called before rendering UI.");
        }
    }
}