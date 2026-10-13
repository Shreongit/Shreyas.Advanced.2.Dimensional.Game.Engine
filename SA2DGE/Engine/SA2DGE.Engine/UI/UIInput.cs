using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.UI;

public sealed class UIInput : UIElement
{
    private string _text = string.Empty;

    public string Text
    {
        get => _text;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (_text == value)
            {
                return;
            }

            _text = value;
            TextChanged?.Invoke(this);
        }
    }

    public string Placeholder { get; set; }

    public int MaxLength { get; set; }

    public bool IsFocused { get; private set; }

    public int CaretPosition { get; private set; }

    public Color TextColor { get; set; }

    public Color PlaceholderColor { get; set; }

    public event Action<UIInput>? TextChanged;

    public event Action<UIInput>? Submitted;

    public event Action<UIInput>? FocusGained;

    public event Action<UIInput>? FocusLost;

    public UIInput(
        string placeholder = "")
        : base("UIInput")
    {
        ArgumentNullException.ThrowIfNull(placeholder);

        Placeholder = placeholder;
        MaxLength = 256;
        TextColor = Color.White;
        PlaceholderColor = new Color(
            0.6f,
            0.6f,
            0.6f);
    }

    public void Focus()
    {
        if (!IsInteractable ||
            IsFocused)
        {
            return;
        }

        IsFocused = true;
        CaretPosition = _text.Length;

        FocusGained?.Invoke(this);
    }

    public void Unfocus()
    {
        if (!IsFocused)
        {
            return;
        }

        IsFocused = false;

        FocusLost?.Invoke(this);
    }

    public void SetText(
        string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (text.Length > MaxLength)
        {
            text = text[..MaxLength];
        }

        _text = text;
        CaretPosition =
            System.Math.Clamp(
                CaretPosition,
                0,
                _text.Length);

        TextChanged?.Invoke(this);
    }

    public void InsertText(
        string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (!IsInteractable ||
            !IsFocused ||
            string.IsNullOrEmpty(text))
        {
            return;
        }

        int available =
            MaxLength - _text.Length;

        if (available <= 0)
        {
            return;
        }

        if (text.Length > available)
        {
            text = text[..available];
        }

        _text =
            _text.Insert(
                CaretPosition,
                text);

        CaretPosition +=
            text.Length;

        TextChanged?.Invoke(this);
    }

    public void Backspace()
    {
        if (!IsFocused ||
            CaretPosition <= 0 ||
            _text.Length == 0)
        {
            return;
        }

        _text =
            _text.Remove(
                CaretPosition - 1,
                1);

        CaretPosition--;

        TextChanged?.Invoke(this);
    }

    public void Delete()
    {
        if (!IsFocused ||
            CaretPosition >= _text.Length)
        {
            return;
        }

        _text =
            _text.Remove(
                CaretPosition,
                1);

        TextChanged?.Invoke(this);
    }

    public void MoveCaretLeft()
    {
        if (!IsFocused)
        {
            return;
        }

        CaretPosition =
            System.Math.Max(
                0,
                CaretPosition - 1);
    }

    public void MoveCaretRight()
    {
        if (!IsFocused)
        {
            return;
        }

        CaretPosition =
            System.Math.Min(
                _text.Length,
                CaretPosition + 1);
    }

    public void MoveCaretToStart()
    {
        if (IsFocused)
        {
            CaretPosition = 0;
        }
    }

    public void MoveCaretToEnd()
    {
        if (IsFocused)
        {
            CaretPosition = _text.Length;
        }
    }

    public void Submit()
    {
        if (!IsInteractable ||
            !IsFocused)
        {
            return;
        }

        Submitted?.Invoke(this);
    }

    public bool HasText =>
        _text.Length > 0;

    public string DisplayText =>
        HasText
            ? _text
            : Placeholder;

    public Color DisplayColor =>
        HasText
            ? TextColor
            : PlaceholderColor;

    protected override void OnEnabledChanged(
        bool enabled)
    {
        if (!enabled)
        {
            Unfocus();
        }
    }

    protected override void OnVisibilityChanged(
        bool visible)
    {
        if (!visible)
        {
            Unfocus();
        }
    }
}