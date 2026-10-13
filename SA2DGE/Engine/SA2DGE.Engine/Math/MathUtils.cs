namespace SA2DGE.Engine.Math;

public static class MathUtils
{
    public const float Epsilon = 0.00001f;

    public const float Pi = MathF.PI;

    public const float TwoPi = MathF.PI * 2.0f;

    public const float HalfPi = MathF.PI * 0.5f;

    public static float DegreesToRadians(float degrees)
    {
        return degrees * (Pi / 180.0f);
    }

    public static float RadiansToDegrees(float radians)
    {
        return radians * (180.0f / Pi);
    }

    public static float Clamp(
        float value,
        float min,
        float max)
    {
        return MathF.Max(min, MathF.Min(value, max));
    }

    public static int Clamp(
        int value,
        int min,
        int max)
    {
        return System.Math.Max(min, System.Math.Min(value, max));
    }

    public static float Lerp(
        float a,
        float b,
        float amount)
    {
        return a + ((b - a) * amount);
    }

    public static float InverseLerp(
        float a,
        float b,
        float value)
    {
        if (MathF.Abs(b - a) <= Epsilon)
        {
            return 0.0f;
        }

        return (value - a) / (b - a);
    }

    public static float Remap(
        float value,
        float inputMin,
        float inputMax,
        float outputMin,
        float outputMax)
    {
        float normalized = InverseLerp(
            inputMin,
            inputMax,
            value);

        return Lerp(
            outputMin,
            outputMax,
            normalized);
    }

    public static float MoveTowards(
        float current,
        float target,
        float maxDelta)
    {
        if (MathF.Abs(target - current) <= maxDelta)
        {
            return target;
        }

        return current +
               MathF.Sign(target - current) *
               maxDelta;
    }

    public static float Repeat(
        float value,
        float length)
    {
        if (length <= 0.0f)
        {
            return 0.0f;
        }

        return value -
               MathF.Floor(value / length) * length;
    }

    public static float PingPong(
        float value,
        float length)
    {
        if (length <= 0.0f)
        {
            return 0.0f;
        }

        float repeated = Repeat(value, length * 2.0f);

        return length -
               MathF.Abs(repeated - length);
    }

    public static float WrapAngle(float radians)
    {
        radians = Repeat(
            radians + Pi,
            TwoPi);

        return radians - Pi;
    }

    public static bool Approximately(
        float a,
        float b,
        float tolerance = Epsilon)
    {
        return MathF.Abs(a - b) <= tolerance;
    }

    public static int Sign(float value)
    {
        if (value > 0.0f)
        {
            return 1;
        }

        if (value < 0.0f)
        {
            return -1;
        }

        return 0;
    }

    public static float Min(
        float a,
        float b)
    {
        return MathF.Min(a, b);
    }

    public static float Max(
        float a,
        float b)
    {
        return MathF.Max(a, b);
    }

    public static float Abs(float value)
    {
        return MathF.Abs(value);
    }

    public static float Sqrt(float value)
    {
        return MathF.Sqrt(value);
    }

    public static float Distance(
        float x1,
        float y1,
        float x2,
        float y2)
    {
        float dx = x2 - x1;
        float dy = y2 - y1;

        return MathF.Sqrt(
            dx * dx +
            dy * dy);
    }

    public static float AngleBetween(
        Vector2 from,
        Vector2 to)
    {
        float cross =
            from.X * to.Y -
            from.Y * to.X;

        float dot =
            Vector2.Dot(from, to);

        return MathF.Atan2(cross, dot);
    }

    public static Vector2 DirectionFromAngle(
        float radians)
    {
        return new Vector2(
            MathF.Cos(radians),
            MathF.Sin(radians));
    }
}