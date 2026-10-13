using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.UI;

public class UIContainer : UIElement
{
    private readonly List<UIElement> _children = new();

    public IReadOnlyList<UIElement> Children =>
        _children;

    public int ChildCount =>
        _children.Count;

    public UIContainer(
        string name = "UIContainer")
        : base(name)
    {
    }

    public void AddChild(
        UIElement child)
    {
        ArgumentNullException.ThrowIfNull(child);

        if (ReferenceEquals(
                child,
                this))
        {
            throw new InvalidOperationException(
                "A UI element cannot be its own child.");
        }

        if (_children.Contains(child))
        {
            return;
        }

        if (child.Parent is UIContainer previousParent)
        {
            previousParent.RemoveChild(child);
        }

        child.Parent = this;
        _children.Add(child);
    }

    public bool RemoveChild(
        UIElement child)
    {
        ArgumentNullException.ThrowIfNull(child);

        if (!_children.Remove(child))
        {
            return false;
        }

        if (ReferenceEquals(
                child.Parent,
                this))
        {
            child.Parent = null;
        }

        return true;
    }

    public UIElement GetChild(
        int index)
    {
        if ((uint)index >=
            (uint)_children.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index));
        }

        return _children[index];
    }

    public UIElement? FindChild(
        string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        foreach (UIElement child in _children)
        {
            if (string.Equals(
                    child.Name,
                    name,
                    StringComparison.OrdinalIgnoreCase))
            {
                return child;
            }

            if (child is UIContainer container)
            {
                UIElement? nested =
                    container.FindChild(name);

                if (nested is not null)
                {
                    return nested;
                }
            }
        }

        return null;
    }

    public UIElement? GetElementAt(
        Vector2 point)
    {
        if (!Visible)
        {
            return null;
        }

        for (int i = _children.Count - 1;
             i >= 0;
             i--)
        {
            UIElement child =
                _children[i];

            if (!child.IsInteractable)
            {
                continue;
            }

            if (child is UIContainer container)
            {
                UIElement? nested =
                    container.GetElementAt(point);

                if (nested is not null)
                {
                    return nested;
                }
            }

            if (child.ContainsPoint(point))
            {
                return child;
            }
        }

        return ContainsPoint(point)
            ? this
            : null;
    }

    public override void Update(
        float deltaTime)
    {
        if (!Visible ||
            !Enabled)
        {
            return;
        }

        base.Update(deltaTime);

        foreach (UIElement child in _children)
        {
            child.Update(deltaTime);
        }
    }

    public override void Draw()
    {
        if (!Visible)
        {
            return;
        }

        base.Draw();

        foreach (UIElement child in _children)
        {
            child.Draw();
        }
    }

    public void Clear()
    {
        foreach (UIElement child in _children)
        {
            if (ReferenceEquals(
                    child.Parent,
                    this))
            {
                child.Parent = null;
            }
        }

        _children.Clear();
    }
}