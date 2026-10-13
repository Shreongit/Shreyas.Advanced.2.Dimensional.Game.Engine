namespace SA2DGE.Engine.Graphics;

public sealed class RenderQueue
{
    private readonly List<RenderCommand> _commands = new();

    public int Count => _commands.Count;

    public bool IsEmpty => _commands.Count == 0;

    public void Submit(RenderCommand command)
    {
        _commands.Add(command);
    }

    public void SubmitRange(
        IEnumerable<RenderCommand> commands)
    {
        ArgumentNullException.ThrowIfNull(commands);

        _commands.AddRange(commands);
    }

    public void Sort()
    {
        _commands.Sort(
            static (left, right) =>
            {
                int layerComparison =
                    left.Layer.CompareTo(right.Layer);

                if (layerComparison != 0)
                {
                    return layerComparison;
                }

                return left.Type.CompareTo(right.Type);
            });
    }

    public void Clear()
    {
        _commands.Clear();
    }

    public IReadOnlyList<RenderCommand> Commands =>
        _commands;

    public void Execute(
        Action<RenderCommand> renderer)
    {
        ArgumentNullException.ThrowIfNull(renderer);

        foreach (RenderCommand command in _commands)
        {
            renderer(command);
        }
    }
}