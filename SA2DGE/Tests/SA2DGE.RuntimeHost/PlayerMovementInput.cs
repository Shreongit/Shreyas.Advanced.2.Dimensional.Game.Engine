using SA2DGE.Engine.Math;
using SA2DGE.Engine.Platform.Input;

internal interface IPlayerMovementInput
{
    Vector2 GetMovementDirection();
}

internal sealed class KeyboardPlayerMovementInput : IPlayerMovementInput
{
    private readonly Keyboard _keyboard;

    public KeyboardPlayerMovementInput(Keyboard keyboard)
    {
        _keyboard = keyboard
                    ?? throw new ArgumentNullException(nameof(keyboard));
    }

    public Vector2 GetMovementDirection()
    {
        float horizontal = 0.0f;
        float vertical = 0.0f;

        if (_keyboard.IsDown(InputKey.A) ||
            _keyboard.IsDown(InputKey.Left))
        {
            horizontal -= 1.0f;
        }

        if (_keyboard.IsDown(InputKey.D) ||
            _keyboard.IsDown(InputKey.Right))
        {
            horizontal += 1.0f;
        }

        if (_keyboard.IsDown(InputKey.W) ||
            _keyboard.IsDown(InputKey.Up))
        {
            vertical -= 1.0f;
        }

        if (_keyboard.IsDown(InputKey.S) ||
            _keyboard.IsDown(InputKey.Down))
        {
            vertical += 1.0f;
        }

        return new Vector2(horizontal, vertical);
    }
}


internal sealed class TestPlayerMovementInput : IPlayerMovementInput
{
    public Vector2 MovementDirection { get; set; } = Vector2.Zero;

    public Vector2 GetMovementDirection()
    {
        return MovementDirection;
    }
}
