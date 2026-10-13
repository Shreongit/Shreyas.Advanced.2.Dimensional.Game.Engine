using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Audio;

public sealed class AudioListener
{
    public Vector2 Position { get; set; }

    public Vector2 Forward { get; set; }

    public Vector2 Up { get; set; }

    public float Volume { get; set; }

    public AudioListener()
    {
        Position = Vector2.Zero;
        Forward = Vector2.UnitY;
        Up = new Vector2(-1.0f, 0.0f);
        Volume = 1.0f;
    }

    public void SetPosition(
        Vector2 position)
    {
        Position = position;
    }

    public void SetOrientation(
        Vector2 forward,
        Vector2 up)
    {
        if (forward.LengthSquared <= MathUtils.Epsilon)
        {
            throw new ArgumentException(
                "Forward direction must not be zero.",
                nameof(forward));
        }

        if (up.LengthSquared <= MathUtils.Epsilon)
        {
            throw new ArgumentException(
                "Up direction must not be zero.",
                nameof(up));
        }

        Forward = forward.Normalized();
        Up = up.Normalized();
    }

    public void SetVolume(
        float volume)
    {
        Volume = MathUtils.Clamp(
            volume,
            0.0f,
            1.0f);
    }

    public float GetDistance(
        Vector2 sourcePosition)
    {
        return Vector2.Distance(
            Position,
            sourcePosition);
    }

    public Vector2 GetDirectionTo(
        Vector2 sourcePosition)
    {
        return (
            sourcePosition -
            Position).Normalized();
    }
}