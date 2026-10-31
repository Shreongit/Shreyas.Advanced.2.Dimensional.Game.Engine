
using SA2DGE.Engine.Graphics;
using SA2DGE.Engine.Math;
using Xunit;

public sealed class EnvironmentManagerTests
{
    private static EnvironmentObject CreateObject(
        float x = 200.0f,
        float y = 200.0f,
        float width = 80.0f,
        float height = 60.0f)
    {
        return new EnvironmentObject(
            position: new Vector2(x, y),
            size: new Vector2(width, height),
            color: new Color(160, 90, 45));
    }

    [Fact]
    public void Add_IncreasesCount()
    {
        var manager = new EnvironmentManager();

        manager.Add(CreateObject());

        Assert.Equal(1, manager.Count);
    }

    [Fact]
    public void Add_NullObject_Throws()
    {
        var manager = new EnvironmentManager();

        Assert.Throws<ArgumentNullException>(
            () => manager.Add(null!));
    }

    [Fact]
    public void Remove_ExistingObject_ReturnsTrue()
    {
        var manager = new EnvironmentManager();
        var environmentObject = CreateObject();

        manager.Add(environmentObject);

        bool removed = manager.Remove(environmentObject);

        Assert.True(removed);
        Assert.Equal(0, manager.Count);
    }

    [Fact]
    public void Remove_UnknownObject_ReturnsFalse()
    {
        var manager = new EnvironmentManager();

        bool removed = manager.Remove(CreateObject());

        Assert.False(removed);
        Assert.Equal(0, manager.Count);
    }

    [Fact]
    public void Clear_RemovesAllObjects()
    {
        var manager = new EnvironmentManager();

        manager.Add(CreateObject(100.0f, 100.0f));
        manager.Add(CreateObject(300.0f, 300.0f));

        manager.Clear();

        Assert.Equal(0, manager.Count);
        Assert.Empty(manager.Objects);
        Assert.Empty(manager.CollisionBounds);
    }

    [Fact]
    public void CollisionBounds_MatchEnvironmentObjects()
    {
        var manager = new EnvironmentManager();
        var environmentObject = CreateObject(
            x: 200.0f,
            y: 150.0f,
            width: 80.0f,
            height: 60.0f);

        manager.Add(environmentObject);

        Rectangle bounds = Assert.Single(manager.CollisionBounds);

        Assert.Equal(160.0f, bounds.X);
        Assert.Equal(120.0f, bounds.Y);
        Assert.Equal(80.0f, bounds.Width);
        Assert.Equal(60.0f, bounds.Height);
    }

    [Fact]
    public void GenerateRandomized_WithSameSeed_ProducesSameLayout()
    {
        var firstManager = new EnvironmentManager();
        var secondManager = new EnvironmentManager();

        var playerPosition = new Vector2(640.0f, 360.0f);

        firstManager.GenerateRandomized(
            playerPosition, 1280.0f, 720.0f, seed: 123);

        secondManager.GenerateRandomized(
            playerPosition, 1280.0f, 720.0f, seed: 123);

        Assert.Equal(firstManager.Count, secondManager.Count);

        for (int i = 0; i < firstManager.Count; i++)
        {
            Assert.Equal(
                firstManager.Objects[i].Position.X,
                secondManager.Objects[i].Position.X);

            Assert.Equal(
                firstManager.Objects[i].Position.Y,
                secondManager.Objects[i].Position.Y);

            Assert.Equal(
                firstManager.Objects[i].Size.X,
                secondManager.Objects[i].Size.X);

            Assert.Equal(
                firstManager.Objects[i].Size.Y,
                secondManager.Objects[i].Size.Y);
        }
    }

    [Fact]
    public void GenerateRandomized_CalledAgain_ReplacesPreviousLayout()
    {
        var manager = new EnvironmentManager();

        manager.Add(CreateObject());

        manager.GenerateRandomized(
            new Vector2(640.0f, 360.0f),
            1280.0f,
            720.0f,
            seed: 42);

        Assert.Equal(8, manager.Count);
    }

    [Fact]
    public void GenerateRandomized_InvalidWindowWidth_Throws()
    {
        var manager = new EnvironmentManager();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            manager.GenerateRandomized(
                new Vector2(640.0f, 360.0f),
                0.0f,
                720.0f));
    }

    [Fact]
    public void GenerateRandomized_InvalidWindowHeight_Throws()
    {
        var manager = new EnvironmentManager();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            manager.GenerateRandomized(
                new Vector2(640.0f, 360.0f),
                1280.0f,
                float.NaN));
    }
    
    
private static PlayerController CreateCollisionTestPlayer(
    TestPlayerMovementInput input,
    Vector2 initialPosition)
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
        initialPosition: initialPosition,
        moveSpeed: 180.0f,
        windowWidth: 1280.0f,
        windowHeight: 720.0f,
        spriteWidth: 80.0f,
        spriteHeight: 112.0f);
}

[Fact]
public void SynchronizeCollisionBounds_ObstacleBlocksPlayer()
{
    var manager = new EnvironmentManager();
    var input = new TestPlayerMovementInput
    {
        MovementDirection = new Vector2(1.0f, 0.0f)
    };

    var player = CreateCollisionTestPlayer(
        input, new Vector2(100.0f, 200.0f));

    manager.Add(CreateObject(
        x: 200.0f, y: 200.0f, width: 80.0f, height: 80.0f));

    manager.SynchronizeCollisionBounds(player);
    player.Update(1.0f);

    Assert.Equal(120.0f, player.Position.X);
}

[Fact]
public void SynchronizeCollisionBounds_AfterClear_RemovesOldCollision()
{
    var manager = new EnvironmentManager();
    var input = new TestPlayerMovementInput
    {
        MovementDirection = new Vector2(1.0f, 0.0f)
    };

    var player = CreateCollisionTestPlayer(
        input, new Vector2(100.0f, 200.0f));

    manager.Add(CreateObject(
        x: 200.0f, y: 200.0f, width: 80.0f, height: 80.0f));

    manager.SynchronizeCollisionBounds(player);
    manager.Clear();
    manager.SynchronizeCollisionBounds(player);

    player.Update(1.0f);

    Assert.Equal(280.0f, player.Position.X);
}

[Fact]
public void SynchronizeCollisionBounds_AfterRemoval_UpdatesPlayerCollision()
{
    var manager = new EnvironmentManager();
    var input = new TestPlayerMovementInput
    {
        MovementDirection = new Vector2(1.0f, 0.0f)
    };

    var player = CreateCollisionTestPlayer(
        input, new Vector2(100.0f, 200.0f));

    var obstacle = CreateObject(
        x: 200.0f, y: 200.0f, width: 80.0f, height: 80.0f);

    manager.Add(obstacle);
    manager.SynchronizeCollisionBounds(player);

    Assert.True(manager.Remove(obstacle));

    manager.SynchronizeCollisionBounds(player);
    player.Update(1.0f);

    Assert.Equal(280.0f, player.Position.X);
}


[Fact]
public void HighSpeedMovement_DoesNotPassThroughObstacle()
{
    var manager = new EnvironmentManager();
    var input = new TestPlayerMovementInput
    {
        MovementDirection = new Vector2(1.0f, 0.0f)
    };

    var player = CreateCollisionTestPlayer(
        input, new Vector2(100.0f, 200.0f));

    manager.Add(CreateObject(
        x: 200.0f, y: 200.0f,
        width: 80.0f, height: 80.0f));

    manager.SynchronizeCollisionBounds(player);

    // A five-second update requests 900 pixels of movement.
    player.Update(5.0f);

    // The player's right edge must stop at the obstacle's left edge.
    Assert.Equal(120.0f, player.Position.X);
}

[Fact]
public void DiagonalMovement_StopsAtObstacleCorner()
{
    var manager = new EnvironmentManager();
    var input = new TestPlayerMovementInput
    {
        MovementDirection = new Vector2(1.0f, 1.0f)
    };

    var player = CreateCollisionTestPlayer(
        input, new Vector2(100.0f, 100.0f));

    manager.Add(CreateObject(
        x: 200.0f, y: 200.0f,
        width: 80.0f, height: 80.0f));

    manager.SynchronizeCollisionBounds(player);
    player.Update(1.0f);

    // Horizontal movement occurs first; vertical movement then
    // stops the player at the obstacle's top edge.
    Assert.InRange(player.Position.X, 220.0f, 240.0f);
    Assert.Equal(104.0f, player.Position.Y);
}

[Fact]
public void MovementAlongObstacleEdge_RemainsUnblocked()
{
    var manager = new EnvironmentManager();
    var input = new TestPlayerMovementInput
    {
        MovementDirection = new Vector2(1.0f, 0.0f)
    };

    // The player's bottom edge exactly touches the obstacle's top.
    var player = CreateCollisionTestPlayer(
        input, new Vector2(100.0f, 104.0f));

    manager.Add(CreateObject(
        x: 200.0f, y: 200.0f,
        width: 80.0f, height: 80.0f));

    manager.SynchronizeCollisionBounds(player);
    player.Update(0.5f);

    Assert.Equal(190.0f, player.Position.X);
    Assert.Equal(104.0f, player.Position.Y);
}

[Fact]
public void HighSpeedMovement_Leftward_DoesNotPassThroughObstacle()
{
    var manager = new EnvironmentManager();
    var input = new TestPlayerMovementInput
    {
        MovementDirection = new Vector2(-1.0f, 0.0f)
    };

    var player = CreateCollisionTestPlayer(
        input, new Vector2(400.0f, 200.0f));

    manager.Add(CreateObject(
        x: 300.0f, y: 200.0f,
        width: 80.0f, height: 80.0f));

    manager.SynchronizeCollisionBounds(player);
    player.Update(5.0f);

    // The player's left edge must stop at the obstacle's right edge.
    Assert.Equal(380.0f, player.Position.X);
}


[Fact]
public void Boundary_LeftMovement_StopsAtLeftEdge()
{
    var input = new TestPlayerMovementInput
    {
        MovementDirection = new Vector2(-1.0f, 0.0f)
    };

    var player = CreateCollisionTestPlayer(
        input, new Vector2(100.0f, 200.0f));

    player.Update(5.0f);

    Assert.Equal(40.0f, player.Position.X);
}

[Fact]
public void Boundary_RightMovement_StopsAtRightEdge()
{
    var input = new TestPlayerMovementInput
    {
        MovementDirection = new Vector2(1.0f, 0.0f)
    };

    var player = CreateCollisionTestPlayer(
        input, new Vector2(1100.0f, 200.0f));

    player.Update(5.0f);

    Assert.Equal(1240.0f, player.Position.X);
}

[Fact]
public void Boundary_TopMovement_StopsAtTopEdge()
{
    var input = new TestPlayerMovementInput
    {
        MovementDirection = new Vector2(0.0f, -1.0f)
    };

    var player = CreateCollisionTestPlayer(
        input, new Vector2(300.0f, 200.0f));

    player.Update(5.0f);

    Assert.Equal(56.0f, player.Position.Y);
}

[Fact]
public void Boundary_BottomMovement_StopsAtBottomEdge()
{
    var input = new TestPlayerMovementInput
    {
        MovementDirection = new Vector2(0.0f, 1.0f)
    };

    var player = CreateCollisionTestPlayer(
        input, new Vector2(300.0f, 500.0f));

    player.Update(5.0f);

    Assert.Equal(664.0f, player.Position.Y);
}

[Fact]
public void ResizeWindow_LargerWindow_UpdatesMovementLimits()
{
    var input = new TestPlayerMovementInput
    {
        MovementDirection = new Vector2(1.0f, 0.0f)
    };

    var player = CreateCollisionTestPlayer(
        input, new Vector2(1200.0f, 300.0f));

    player.ResizeWindow(1600.0f, 900.0f);
    player.Update(1.0f);

    Assert.Equal(1380.0f, player.Position.X);
}

[Fact]
public void ResizeWindow_SmallerWindow_ClampsPlayerPosition()
{
    var input = new TestPlayerMovementInput
    {
        MovementDirection = Vector2.Zero
    };

    var player = CreateCollisionTestPlayer(
        input, new Vector2(1200.0f, 600.0f));

    player.ResizeWindow(800.0f, 600.0f);

    Assert.Equal(760.0f, player.Position.X);
    Assert.Equal(544.0f, player.Position.Y);
}

[Fact]
public void ResizeWindow_InvalidDimensions_PreservesPreviousBounds()
{
    var input = new TestPlayerMovementInput
    {
        MovementDirection = new Vector2(1.0f, 0.0f)
    };

    var player = CreateCollisionTestPlayer(
        input, new Vector2(1200.0f, 300.0f));

    player.ResizeWindow(1600.0f, 900.0f);

    // Invalid dimensions must not replace the valid 1600x900 window.
    player.ResizeWindow(20.0f, 20.0f);
    player.Update(1.0f);

    Assert.Equal(1380.0f, player.Position.X);
    Assert.Equal(300.0f, player.Position.Y);
}



    
}

