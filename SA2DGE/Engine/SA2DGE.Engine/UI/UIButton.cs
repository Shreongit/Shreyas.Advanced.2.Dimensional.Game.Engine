using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.UI;

public sealed class UIButton : UIElement
{
    private bool _pressed;

    public string Text { get; set; }

    public Color NormalColor { get; set; }

    public Color HoverColor { get; set; }

    public Color PressedColor { get; set; }

    public Color DisabledColor { get; set; }

    public bool IsHovered { get; private set; }

    public bool IsPressed =>
        _pressed;

    public event Action<UIButton>? Clicked;

    public event Action<UIButton>? Pressed;

    public event Action<UIButton>? Released;

    public UIButton(
        string text = "")
        : base("UIButton")
    {
        ArgumentNullException.ThrowIfNull(text);

        Text = text;

        NormalColor = Color.White;
        HoverColor = new Color(
            0.85f,
            0.85f,
            0.85f);

        PressedColor = new Color(
            0.65f,
            0.65f,
            0.65f);

        DisabledColor = new Color(
            0.5f,
            0.5f,
            0.5f);
    }

    public void SetHovered(
        bool hovered)
    {
        if (!IsInteractable)
        {
            IsHovered = false;
            return;
        }

        IsHovered = hovered;
    }

    public void SetPressed(
        bool pressed)
    {
        if (!IsInteractable)
        {
            return;
        }

        if (_pressed == pressed)
        {
            return;
        }

        _pressed = pressed;

        if (pressed)
        {
            Pressed?.Invoke(this);
        }
        else
        {
            Released?.Invoke(this);

            if (IsHovered)
            {
                Clicked?.Invoke(this);
            }
        }
    }

    public void Click()
    {
        if (!IsInteractable)
        {
            return;
        }

        Clicked?.Invoke(this);
    }

    public Color GetCurrentColor()
    {
        if (!Enabled)
        {
            return DisabledColor;
        }

        if (_pressed)
        {
            return PressedColor;
        }

        if (IsHovered)
        {
            return HoverColor;
        }

        return NormalColor;
    }

    protected override void OnEnabledChanged(
        bool enabled)
    {
        if (!enabled)
        {
            _pressed = false;
            IsHovered = false;
        }
    }

    protected override void OnVisibilityChanged(
        bool visible)
    {
        if (!visible)
        {
            _pressed = false;
            IsHovered = false;
        }
    }
}