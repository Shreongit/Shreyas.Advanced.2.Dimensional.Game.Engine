using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.UI;

public sealed class UIPanel : UIContainer
{
    public Color BackgroundColor { get; set; }

    public Color BorderColor { get; set; }

    public float BorderThickness { get; set; }

    public bool HasBorder { get; set; }

    public UIPanel(
        string name = "UIPanel")
        : base(name)
    {
        BackgroundColor = new Color(
            0.15f,
            0.15f,
            0.15f,
            1.0f);

        BorderColor = Color.White;
        BorderThickness = 1.0f;
        HasBorder = false;
    }

    public void SetBackgroundColor(
        Color color)
    {
        BackgroundColor = color;
    }

    public void SetBorder(
        Color color,
        float thickness = 1.0f)
    {
        if (thickness < 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(thickness));
        }

        BorderColor = color;
        BorderThickness = thickness;
        HasBorder = thickness > 0.0f;
    }

    public void RemoveBorder()
    {
        HasBorder = false;
        BorderThickness = 0.0f;
    }

    public override void Draw()
    {
        if (!Visible)
        {
            return;
        }

        base.Draw();
    }
}