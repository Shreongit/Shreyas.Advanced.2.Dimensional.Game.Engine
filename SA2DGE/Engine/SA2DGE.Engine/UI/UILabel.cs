using SA2DGE.Engine.Graphics.Fonts;
using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.UI;

public sealed class UILabel : UIElement
{
    private string _text;

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
            UpdateSize();
        }
    }

    public Font? Font { get; set; }

    public float FontSize { get; set; }

    public Color TextColor { get; set; }

    public TextAlignment Alignment { get; set; }

    public UILabel(
        string text = "",
        Font? font = null)
        : base("UILabel")
    {
        ArgumentNullException.ThrowIfNull(text);

        _text = text;
        Font = font;
        FontSize = font?.Size ?? 16.0f;
        TextColor = Color.White;
        Alignment = TextAlignment.Left;

        UpdateSize();
    }

    public Vector2 Measure()
    {
        if (Font is null ||
            string.IsNullOrEmpty(_text))
        {
            return Vector2.Zero;
        }

        return new Vector2(
            Font.MeasureWidth(_text),
            Font.MeasureHeight(_text));
    }

    public void SetText(
        string text)
    {
        Text = text;
    }

    public void SetFont(
        Font font)
    {
        ArgumentNullException.ThrowIfNull(font);

        Font = font;

        if (FontSize <= 0.0f)
        {
            FontSize = font.Size;
        }

        UpdateSize();
    }

    public void SetColor(
        Color color)
    {
        TextColor = color;
    }

    private void UpdateSize()
    {
        Vector2 measuredSize =
            Measure();

        if (measuredSize != Vector2.Zero)
        {
            Size = measuredSize;
        }
    }
}

public enum TextAlignment
{
    Left,
    Center,
    Right
}