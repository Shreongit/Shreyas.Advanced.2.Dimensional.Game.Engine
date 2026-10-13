using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.UI;

public abstract class UIElement
{
    private bool _visible = true;
    private bool _enabled = true;

    public string Name { get; set; }

    public Vector2 Position { get; set; }

    public Vector2 Size { get; set; }

    public Vector2 Pivot { get; set; }

    public bool Visible
    {
        get => _visible;
        set
        {
            if (_visible == value)
            {
                return;
            }

            _visible = value;
            OnVisibilityChanged(value);
        }
    }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled == value)
            {
                return;
            }

            _enabled = value;
            OnEnabledChanged(value);
        }
    }

    public UIElement? Parent { get; internal set; }

    public Rectangle Bounds =>
        new(
            Position.X - Size.X * Pivot.X,
            Position.Y - Size.Y * Pivot.Y,
            Size.X,
            Size.Y);

    public bool IsInteractable =>
        Visible &&
        Enabled;

    protected UIElement(
        string name = "UIElement")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        Position = Vector2.Zero;
        Size = Vector2.Zero;
        Pivot = Vector2.Zero;
    }

    public bool ContainsPoint(
        Vector2 point)
    {
        return Visible &&
               Bounds.Contains(point);
    }

    public void SetPosition(
        Vector2 position)
    {
        Position = position;
    }

    public void SetSize(
        Vector2 size)
    {
        if (size.X < 0.0f ||
            size.Y < 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(size),
                "UI element size cannot be negative.");
        }

        Size = size;
    }

    public void SetPivot(
        Vector2 pivot)
    {
        Pivot = pivot;
    }

    public void Show()
    {
        Visible = true;
    }

    public void Hide()
    {
        Visible = false;
    }

    public void Enable()
    {
        Enabled = true;
    }

    public void Disable()
    {
        Enabled = false;
    }

    public virtual void Update(
        float deltaTime)
    {
    }

    public virtual void Draw()
    {
    }

    protected virtual void OnVisibilityChanged(
        bool visible)
    {
    }

    protected virtual void OnEnabledChanged(
        bool enabled)
    {
    }

    public override string ToString()
    {
        return $"{GetType().Name}: {Name}";
    }
}