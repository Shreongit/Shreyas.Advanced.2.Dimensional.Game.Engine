
using System;
using System.Collections.Generic;
using System.Linq;
using SA2DGE.Engine.Graphics;
using SA2DGE.Engine.Math;

/// <summary>
/// Manages environment objects, randomized generation,
/// rendering, and collision-bound synchronization.
/// </summary>
internal sealed class EnvironmentManager
{
    private readonly List<EnvironmentObject> _objects = new();

    public int Count => _objects.Count;

    public IReadOnlyList<EnvironmentObject> Objects => _objects;

    public IEnumerable<Rectangle> CollisionBounds =>
        _objects.Select(item => item.CollisionBounds);

    public void Clear()
    {
        _objects.Clear();
    }

    public void Add(EnvironmentObject environmentObject)
    {
        ArgumentNullException.ThrowIfNull(environmentObject);
        _objects.Add(environmentObject);
    }

    public bool Remove(EnvironmentObject environmentObject)
    {
        ArgumentNullException.ThrowIfNull(environmentObject);
        return _objects.Remove(environmentObject);
    }

    public void SynchronizeCollisionBounds(PlayerController playerController)
    {
        ArgumentNullException.ThrowIfNull(playerController);

        playerController.SetCollisionBounds(CollisionBounds);
    }

    public void GenerateRandomized(
        Vector2 playerPosition,
        float windowWidth,
        float windowHeight,
        int seed = 42)
    {
        if (!float.IsFinite(windowWidth) || windowWidth <= 0.0f)
            throw new ArgumentOutOfRangeException(nameof(windowWidth));

        if (!float.IsFinite(windowHeight) || windowHeight <= 0.0f)
            throw new ArgumentOutOfRangeException(nameof(windowHeight));

        const int obstacleCount = 8;
        const int maxAttemptsPerObstacle = 200;
        const float edgeMargin = 20.0f;
        const float playerClearance = 40.0f;

        _objects.Clear();

        Random random = new(seed);

        Rectangle playerBounds = new Rectangle(
            playerPosition.X - 40.0f,
            playerPosition.Y - 56.0f,
            80.0f,
            112.0f).Inflate(playerClearance, playerClearance);

        Color[] obstacleColors =
        {
            new Color(160, 90, 45),
            new Color(110, 110, 120),
            new Color(70, 140, 80),
            new Color(170, 120, 55)
        };

        for (int i = 0; i < obstacleCount; i++)
        {
            bool placed = false;

            for (int attempt = 0;
                 attempt < maxAttemptsPerObstacle;
                 attempt++)
            {
                float width = random.Next(60, 141);
                float height = random.Next(45, 111);

                float x = random.Next(
                    (int)(edgeMargin + width / 2.0f),
                    (int)(windowWidth - edgeMargin - width / 2.0f));

                float y = random.Next(
                    (int)(edgeMargin + height / 2.0f),
                    (int)(windowHeight - edgeMargin - height / 2.0f));

                EnvironmentObject candidate = new(
                    position: new Vector2(x, y),
                    size: new Vector2(width, height),
                    color: obstacleColors[
                        random.Next(obstacleColors.Length)],
                    layer: 1);

                Rectangle candidateBounds = candidate.CollisionBounds;

                if (candidateBounds.Intersects(playerBounds))
                    continue;

                bool overlapsObstacle = _objects.Any(
                    existing => candidateBounds.Intersects(
                        existing.CollisionBounds));

                if (overlapsObstacle)
                    continue;

                _objects.Add(candidate);
                placed = true;
                break;
            }

            if (!placed)
            {
                Console.WriteLine(
                    $"Could not place obstacle {i + 1} after " +
                    $"{maxAttemptsPerObstacle} attempts.");
            }
        }

        Console.WriteLine(
            $"Generated {_objects.Count} environment obstacles.");
    }

    public void Draw(Renderer2D renderer)
    {
        ArgumentNullException.ThrowIfNull(renderer);

        foreach (EnvironmentObject environmentObject in _objects)
        {
            environmentObject.Draw(renderer);
        }
    }
}
