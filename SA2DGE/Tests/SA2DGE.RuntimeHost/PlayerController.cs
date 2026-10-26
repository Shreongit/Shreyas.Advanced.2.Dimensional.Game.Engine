
using SA2DGE.Engine.Graphics;
using SA2DGE.Engine.Math;
using SA2DGE.Engine.Platform.Input;

internal sealed class PlayerController
{
    private readonly SpriteAnimationController _animations;
    private readonly IPlayerMovementInput _movementInput;
    private float _moveSpeed;
    private readonly float _windowWidth;
    private readonly float _windowHeight;
    private readonly float _spriteWidth;
    private readonly float _spriteHeight;

    private bool _isMoving;

    public Vector2 Position { get; private set; }

    public bool IsFacingLeft { get; private set; }
    
    
    
    public float MoveSpeed
    {
        get => _moveSpeed;
        set
        {
            if (!float.IsFinite(value) || value < 0.0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "Movement speed must be finite and non-negative.");
            }

            _moveSpeed = value;
        }
    }


    public SpriteAnimation? CurrentAnimation =>
        _animations.CurrentAnimation;

    public int CurrentFrame =>
        CurrentAnimation?.CurrentFrame ?? 0;

    public PlayerController(
        SpriteAnimationController animations,
        IPlayerMovementInput movementInput,
        Vector2 initialPosition,
        float moveSpeed,
        float windowWidth,
        float windowHeight,
        float spriteWidth,
        float spriteHeight)
    {
        
        
        ArgumentNullException.ThrowIfNull(animations);

        if (!float.IsFinite(moveSpeed) || moveSpeed < 0.0f)
            throw new ArgumentOutOfRangeException(nameof(moveSpeed));

        if (!float.IsFinite(windowWidth) ||
            !float.IsFinite(windowHeight) ||
            !float.IsFinite(spriteWidth) ||
            !float.IsFinite(spriteHeight) ||
            windowWidth <= 0.0f ||
            windowHeight <= 0.0f ||
            spriteWidth <= 0.0f ||
            spriteHeight <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(windowWidth),
                "Window and sprite dimensions must be finite and positive.");
        }

        if (windowWidth < spriteWidth || windowHeight < spriteHeight)
            throw new ArgumentException(
                "The sprite must fit inside the game window.");

        if (!float.IsFinite(initialPosition.X) ||
            !float.IsFinite(initialPosition.Y))
        {
            throw new ArgumentOutOfRangeException(nameof(initialPosition));
        }


        _animations = animations;
        _movementInput = movementInput
                         ?? throw new ArgumentNullException(nameof(movementInput));
        _moveSpeed = moveSpeed;
        _windowWidth = windowWidth;
        _windowHeight = windowHeight;
        _spriteWidth = spriteWidth;
        _spriteHeight = spriteHeight;

        Position = initialPosition;
        IsFacingLeft = false;

        ClampToWindow();
    }

    public void Update(float deltaTime)
    {
        
        if (!float.IsFinite(deltaTime) || deltaTime < 0.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(deltaTime),
                "Delta time must be finite and non-negative.");
        }
        
        Vector2 movement = _movementInput.GetMovementDirection();

        float moveX = movement.X;
        float moveY = movement.Y;

        bool moving = moveX != 0.0f || moveY != 0.0f;

        if (moving)
        {
            float length = MathF.Sqrt(moveX * moveX + moveY * moveY);

            moveX /= length;
            moveY /= length;

            Position = new Vector2(
                Position.X + moveX * _moveSpeed * deltaTime,
                Position.Y + moveY * _moveSpeed * deltaTime);

            if (moveX < 0.0f)
                IsFacingLeft = true;
            else if (moveX > 0.0f)
                IsFacingLeft = false;
        }

        ClampToWindow();

        if (moving != _isMoving)
        {
            _isMoving = moving;
            _animations.Play(moving ? "Walk" : "Idle");
        }

        _animations.Update(deltaTime);
    }

    private void ClampToWindow()
    {
        Position = new Vector2(
            Math.Clamp(
                Position.X,
                _spriteWidth / 2.0f,
                _windowWidth - _spriteWidth / 2.0f),
            Math.Clamp(
                Position.Y,
                _spriteHeight / 2.0f,
                _windowHeight - _spriteHeight / 2.0f));
    }
}
