
using SA2DGE.Engine.Graphics;
using SA2DGE.Engine.Math;
using SA2DGE.Engine.Platform.Input;

internal sealed class PlayerController
{
    private readonly SpriteAnimationController _animations;
    private readonly IPlayerMovementInput _movementInput;

    private readonly float _windowWidth;
    private readonly float _windowHeight;
    private readonly float _spriteWidth;
    private readonly float _spriteHeight;

    private Rectangle[] _collisionBounds = Array.Empty<Rectangle>();
    private float _moveSpeed;
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
        ArgumentNullException.ThrowIfNull(movementInput);

        if (!float.IsFinite(moveSpeed) || moveSpeed < 0.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(moveSpeed));
        }

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

        if (windowWidth < spriteWidth ||
            windowHeight < spriteHeight)
        {
            throw new ArgumentException(
                "The sprite must fit inside the game window.");
        }

        if (!float.IsFinite(initialPosition.X) ||
            !float.IsFinite(initialPosition.Y))
        {
            throw new ArgumentOutOfRangeException(nameof(initialPosition));
        }

        _animations = animations;
        _movementInput = movementInput;
        _moveSpeed = moveSpeed;

        _windowWidth = windowWidth;
        _windowHeight = windowHeight;
        _spriteWidth = spriteWidth;
        _spriteHeight = spriteHeight;

        Position = initialPosition;
        IsFacingLeft = false;

        ClampToWindow();
    }

    /// <summary>
    /// Sets the static rectangular collision obstacles used by this player.
    /// The supplied rectangles are copied so later collection changes
    /// do not silently change the controller's collision configuration.
    /// </summary>
    public void SetCollisionBounds(
        IEnumerable<Rectangle> collisionBounds)
    {
        ArgumentNullException.ThrowIfNull(collisionBounds);

        Rectangle[] bounds = collisionBounds.ToArray();

        foreach (Rectangle bound in bounds)
        {
            if (!float.IsFinite(bound.X) ||
                !float.IsFinite(bound.Y) ||
                !float.IsFinite(bound.Width) ||
                !float.IsFinite(bound.Height))
            {
                throw new ArgumentException(
                    "Collision bounds must contain finite values.",
                    nameof(collisionBounds));
            }
        }

        _collisionBounds = bounds;
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
            float length = MathF.Sqrt(
                moveX * moveX + moveY * moveY);

            if (length > 0.0f && float.IsFinite(length))
            {
                moveX /= length;
                moveY /= length;

                float deltaX = moveX * _moveSpeed * deltaTime;
                float deltaY = moveY * _moveSpeed * deltaTime;

                // Resolve each axis independently to allow wall sliding.
                MoveHorizontally(deltaX);
                MoveVertically(deltaY);

                if (moveX < 0.0f)
                {
                    IsFacingLeft = true;
                }
                else if (moveX > 0.0f)
                {
                    IsFacingLeft = false;
                }
            }
        }

        ClampToWindow();

        if (moving != _isMoving)
        {
            _isMoving = moving;
            _animations.Play(moving ? "Walk" : "Idle");
        }

        _animations.Update(deltaTime);
    }

    private void MoveHorizontally(float amount)
    {
        if (amount == 0.0f)
        {
            return;
        }

        float halfWidth = _spriteWidth / 2.0f;
        float halfHeight = _spriteHeight / 2.0f;

        float currentLeft = Position.X - halfWidth;
        float currentRight = Position.X + halfWidth;
        float currentTop = Position.Y - halfHeight;
        float currentBottom = Position.Y + halfHeight;

        float targetX = Position.X + amount;

        foreach (Rectangle obstacle in _collisionBounds)
        {
            bool overlapsVertically =
                currentBottom > obstacle.Top &&
                currentTop < obstacle.Bottom;

            if (!overlapsVertically)
            {
                continue;
            }

            if (amount > 0.0f &&
                currentRight <= obstacle.Left &&
                targetX + halfWidth > obstacle.Left)
            {
                targetX = MathF.Min(
                    targetX,
                    obstacle.Left - halfWidth);
            }
            else if (amount < 0.0f &&
                     currentLeft >= obstacle.Right &&
                     targetX - halfWidth < obstacle.Right)
            {
                targetX = MathF.Max(
                    targetX,
                    obstacle.Right + halfWidth);
            }
        }

        Position = new Vector2(targetX, Position.Y);
    }

    private void MoveVertically(float amount)
    {
        if (amount == 0.0f)
        {
            return;
        }

        float halfWidth = _spriteWidth / 2.0f;
        float halfHeight = _spriteHeight / 2.0f;

        float currentLeft = Position.X - halfWidth;
        float currentRight = Position.X + halfWidth;
        float currentTop = Position.Y - halfHeight;
        float currentBottom = Position.Y + halfHeight;

        float targetY = Position.Y + amount;

        foreach (Rectangle obstacle in _collisionBounds)
        {
            bool overlapsHorizontally =
                currentRight > obstacle.Left &&
                currentLeft < obstacle.Right;

            if (!overlapsHorizontally)
            {
                continue;
            }

            if (amount > 0.0f &&
                currentBottom <= obstacle.Top &&
                targetY + halfHeight > obstacle.Top)
            {
                targetY = MathF.Min(
                    targetY,
                    obstacle.Top - halfHeight);
            }
            else if (amount < 0.0f &&
                     currentTop >= obstacle.Bottom &&
                     targetY - halfHeight < obstacle.Bottom)
            {
                targetY = MathF.Max(
                    targetY,
                    obstacle.Bottom + halfHeight);
            }
        }

        Position = new Vector2(Position.X, targetY);
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
