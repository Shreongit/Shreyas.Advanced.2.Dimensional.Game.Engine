using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics;

/// <summary>
/// Calculates UV regions for frames in a uniformly divided sprite sheet.
/// </summary>
public sealed class SpriteSheet
{
    public int Columns { get; }
    public int Rows { get; }

    public int FrameCount => Columns * Rows;

    public Vector2 FrameUVSize { get; }

    public SpriteSheet(int columns, int rows)
    {
        if (columns <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(columns),
                "Columns must be greater than zero.");
        }

        if (rows <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rows),
                "Rows must be greater than zero.");
        }

        Columns = columns;
        Rows = rows;

        FrameUVSize = new Vector2(
            1.0f / columns,
            1.0f / rows);
    }

    /// <summary>
    /// Gets the UV offset for a zero-based frame index.
    /// Frames are ordered left-to-right, then top-to-bottom.
    /// </summary>
    public Vector2 GetFrameUVOffset(int frameIndex)
    {
        if (frameIndex < 0 || frameIndex >= FrameCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(frameIndex),
                $"Frame index must be between 0 and {FrameCount - 1}.");
        }

        int column = frameIndex % Columns;
        int row = frameIndex / Columns;

        return new Vector2(
            column * FrameUVSize.X,
            row * FrameUVSize.Y);
    }
}