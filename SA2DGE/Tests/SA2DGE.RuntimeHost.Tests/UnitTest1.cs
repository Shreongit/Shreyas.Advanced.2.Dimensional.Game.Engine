
using SA2DGE.Engine.Graphics;
using SA2DGE.Engine.Math;
using Xunit;

public sealed class PlayerControllerTests
{
    
    
    private static PlayerController CreateController(
        TestPlayerMovementInput input)
    {
        var spriteSheet = new SpriteSheet(columns: 4, rows: 4);

        var idleAnimation = new SpriteAnimation(
            spriteSheet: spriteSheet,
            frameDuration: 0.1f,
            loop: true,
            startFrame: 0,
            frameCount: 4);

        var walkAnimation = new SpriteAnimation(
            spriteSheet: spriteSheet,
            frameDuration: 0.1f,
            loop: true,
            startFrame: 4,
            frameCount: 4);

        var animations = new SpriteAnimationController();
        animations.AddAnimation("Idle", idleAnimation);
        animations.AddAnimation("Walk", walkAnimation);
        animations.Play("Idle");

        return new PlayerController(
            animations: animations,
            movementInput: input,
            initialPosition: new Vector2(400.0f, 300.0f),
            moveSpeed: 180.0f,
            windowWidth: 1280.0f,
            windowHeight: 720.0f,
            spriteWidth: 80.0f,
            spriteHeight: 112.0f);
    }


    [Fact]
    public void NoInput_DoesNotChangePosition()
    {
        var input = new TestPlayerMovementInput();
        var controller = CreateController(input);
        Vector2 initialPosition = controller.Position;

        controller.Update(1.0f);

        Assert.Equal(initialPosition.X, controller.Position.X);
        Assert.Equal(initialPosition.Y, controller.Position.Y);
    }

    [Fact]
    public void RightInput_MovesPlayerRight()
    {
        var input = new TestPlayerMovementInput
        {
            MovementDirection = new Vector2(1.0f, 0.0f)
        };

        var controller = CreateController(input);
        float initialX = controller.Position.X;

        controller.Update(1.0f);

        Assert.Equal(initialX + 180.0f, controller.Position.X, 3);
    }

    [Fact]
    public void DiagonalInput_IsNormalized()
    {
        var input = new TestPlayerMovementInput
        {
            MovementDirection = new Vector2(1.0f, 1.0f)
        };

        var controller = CreateController(input);
        Vector2 initialPosition = controller.Position;

        controller.Update(1.0f);

        float deltaX = controller.Position.X - initialPosition.X;
        float deltaY = controller.Position.Y - initialPosition.Y;

        Assert.Equal(deltaX, deltaY, 3);
        Assert.Equal(180.0f, MathF.Sqrt(
            deltaX * deltaX + deltaY * deltaY), 3);
    }
    
    
    [Fact]
    public void LeftInput_MovesPlayerLeftAndFacesLeft()
    {
        var input = new TestPlayerMovementInput
        {
            MovementDirection = new Vector2(-1.0f, 0.0f)
        };

        var controller = CreateController(input);
        float initialX = controller.Position.X;

        controller.Update(1.0f);

        Assert.Equal(initialX - 180.0f, controller.Position.X, 3);
        Assert.True(controller.IsFacingLeft);
    }

    [Fact]
    public void RightInput_FacesRight()
    {
        var input = new TestPlayerMovementInput
        {
            MovementDirection = new Vector2(1.0f, 0.0f)
        };

        var controller = CreateController(input);

        // Establish left-facing state first.
        input.MovementDirection = new Vector2(-1.0f, 0.0f);
        controller.Update(0.1f);

        input.MovementDirection = new Vector2(1.0f, 0.0f);
        controller.Update(0.1f);

        Assert.False(controller.IsFacingLeft);
    }

    [Fact]
    public void MovementAtWindowEdge_StaysWithinBounds()
    {
        var input = new TestPlayerMovementInput
        {
            MovementDirection = new Vector2(-1.0f, 0.0f)
        };

        var controller = CreateController(input);

        // Move left long enough to reach the boundary.
        input.MovementDirection = new Vector2(-1.0f, 0.0f);
        for (int i = 0; i < 10; i++)
        {
            controller.Update(1.0f);
        }

        Assert.True(controller.Position.X >= 40.0f);
        Assert.True(controller.Position.X <= 1240.0f);
        Assert.True(controller.Position.Y >= 56.0f);
        Assert.True(controller.Position.Y <= 664.0f);
    }
    
    
    [Theory]
    [InlineData(-0.1f)]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void Update_InvalidDeltaTime_Throws(float deltaTime)
    {
        var input = new TestPlayerMovementInput();
        var controller = CreateController(input);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => controller.Update(deltaTime));
    }

    [Fact]
    public void Update_ZeroDeltaTime_DoesNotMovePlayer()
    {
        var input = new TestPlayerMovementInput
        {
            MovementDirection = new Vector2(1.0f, 0.0f)
        };

        var controller = CreateController(input);
        Vector2 initialPosition = controller.Position;

        controller.Update(0.0f);

        Assert.Equal(initialPosition.X, controller.Position.X);
        Assert.Equal(initialPosition.Y, controller.Position.Y);
    }
    
    
    [Fact]
    public void ZeroMoveSpeed_DoesNotChangePosition()
    {
        var input = new TestPlayerMovementInput
        {
            MovementDirection = new Vector2(1.0f, 0.0f)
        };

        var controller = CreateController(input);
        controller.MoveSpeed = 0.0f;

        Vector2 initialPosition = controller.Position;
        controller.Update(1.0f);

        Assert.Equal(initialPosition.X, controller.Position.X);
        Assert.Equal(initialPosition.Y, controller.Position.Y);
    }

    [Fact]
    public void MaximumMoveSpeed_MovesPlayerAtExpectedSpeed()
    {
        var input = new TestPlayerMovementInput
        {
            MovementDirection = new Vector2(1.0f, 0.0f)
        };

        var controller = CreateController(input);
        controller.MoveSpeed = 600.0f;

        float initialX = controller.Position.X;
        controller.Update(0.5f);

        Assert.Equal(initialX + 300.0f, controller.Position.X, 3);
    }

    [Theory]
    [InlineData(-1.0f)]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void InvalidMoveSpeed_Throws(float speed)
    {
        var input = new TestPlayerMovementInput();
        var controller = CreateController(input);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => controller.MoveSpeed = speed);
    }
    
    [Theory]
    [InlineData(-1.0f, 0.0f)]
    [InlineData(1.0f, 0.0f)]
    [InlineData(0.0f, -1.0f)]
    [InlineData(0.0f, 1.0f)]
    public void SustainedMovement_StaysWithinWindow(
        float directionX,
        float directionY)
    {
        var input = new TestPlayerMovementInput
        {
            MovementDirection = new Vector2(directionX, directionY)
        };

        var controller = CreateController(input);
        controller.MoveSpeed = 600.0f;

        for (int i = 0; i < 10; i++)
        {
            controller.Update(1.0f);
        }

        Assert.InRange(controller.Position.X, 40.0f, 1240.0f);
        Assert.InRange(controller.Position.Y, 56.0f, 664.0f);
    }
    
    
    [Fact]
    public void OpposingHorizontalInput_CancelsOut()
    {
        var input = new TestPlayerMovementInput
        {
            MovementDirection = new Vector2(0.0f, 0.0f)
        };

        var controller = CreateController(input);
        Vector2 initialPosition = controller.Position;

        controller.Update(1.0f);

        Assert.Equal(initialPosition.X, controller.Position.X);
        Assert.Equal(initialPosition.Y, controller.Position.Y);
    }

    [Fact]
    public void OpposingVerticalInput_CancelsOut()
    {
        var input = new TestPlayerMovementInput
        {
            MovementDirection = new Vector2(0.0f, 0.0f)
        };

        var controller = CreateController(input);
        Vector2 initialPosition = controller.Position;

        controller.Update(1.0f);

        Assert.Equal(initialPosition.X, controller.Position.X);
        Assert.Equal(initialPosition.Y, controller.Position.Y);
    }




}
