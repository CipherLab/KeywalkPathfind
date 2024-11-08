using Xunit;
using System.Collections.Generic;
using KeyWalkAnalyzer3;
using System;
using System.Linq;

public class AStarTests
{
    private readonly KeyboardLayout _keyboard;
    private readonly AStar _pathfinder;

    public AStarTests()
    {
        _keyboard = new KeyboardLayout();
        _pathfinder = new AStar(_keyboard);
    }

    [Theory]
    [InlineData('a', 's')] // Adjacent keys
    [InlineData('f', 'j')] // Cross-hand movement
    [InlineData('q', 'p')] // Far keys
    public void FindPath_ShouldReturnValidPath(char start, char end)
    {
        // Act
        var path = _pathfinder.FindPath(start, end);

        // Assert
        Assert.NotNull(path);
        Assert.NotEmpty(path);
        Assert.Equal(start, path[0].Key);
        Assert.Equal(end, path[^1].Key);
    }

    [Theory]
    [InlineData('a', 'a', 2)]  // Press + Release
    [InlineData('a', 's', 3)]  // Release + Move + Press
    [InlineData('q', 'p', 12)] // Release + 10 moves + Press
    public void FindPath_ShouldReturnCorrectNumberOfSteps(char start, char end, int expectedSteps)
    {
        // Act
        var path = _pathfinder.FindPath(start, end);

        // Assert
        Assert.Equal(expectedSteps, path.Count);

        // Additional validation
        if (start != end)
        {
            Assert.Equal("release", path[0].Direction);
            Assert.Equal("press", path[^1].Direction);
        }
    }

    [Fact]
    public void FindPath_VerticalMovement_Detected()
    {
        // Arrange
        var verticalTestCases = new[] {
            ('1', 'q'),   // Top row to second row
            ('0', 'z'),   // Top row to bottom row
            ('a', '1'),   // Second row to top row
            ('m', '0')    // Bottom row to top row
        };

        // Act & Assert
        foreach (var (start, end) in verticalTestCases)
        {
            var path = _pathfinder.FindPath(start, end);
            var movementSteps = path.Where(p => !p.IsPress && p.Direction != "release").ToList();

            Assert.True(movementSteps.Any(p => p.Direction.Contains("up") || p.Direction.Contains("down")),
                $"Path from {start} to {end} should include vertical movements");
        }
    }

    [Fact]
    public void FindPath_HorizontalMovement_Detected()
    {
        // Arrange
        var horizontalTestCases = new[] {
            ('q', 'w'),   // Adjacent keys on same row
            ('1', '2'),   // Top row horizontal movement
            ('z', 'x'),   // Bottom row horizontal movement
            ('a', 's')    // Second row horizontal movement
        };

        // Act & Assert
        foreach (var (start, end) in horizontalTestCases)
        {
            var path = _pathfinder.FindPath(start, end);
            var movementSteps = path.Where(p => !p.IsPress && p.Direction != "release").ToList();

            Assert.True(movementSteps.Any(p => p.Direction.Contains("left") || p.Direction.Contains("right")),
                $"Path from {start} to {end} should include horizontal movements");
        }
    }



    [Theory]
    [InlineData('a', 'z', 11)]  // Far keys with significant movement
    [InlineData('1', '0', 9)]   // Top row keys
    [InlineData('p', 'm', 7)]   // Keys from different rows
    public void FindPath_ComplexMovement_VerifyCostAndSteps(char start, char end, int maxExpectedSteps)
    {
        // Act
        var path = _pathfinder.FindPath(start, end);

        // Assert
        Assert.NotEmpty(path);
        Assert.True(path.Count <= maxExpectedSteps,
            $"Path from {start} to {end} should have {maxExpectedSteps} or fewer steps (got {path.Count - 1})");

        // Verify cost calculation
        double totalCost = path.Sum(p => p.Cost);
        double directCost = _pathfinder.CalculateCost(start, end);
        Assert.True(totalCost > 0, "Path cost should be positive");
        Assert.True(totalCost >= directCost, "Path cost should be at least the direct cost");
    }

    [Fact]
    public void FindPath_BoundaryKeys_GeneratesValidPath()
    {
        // Arrange
        char[] boundaryKeys = new[] { '1', 'q', 'p', 'm', '0', 'z' };

        // Act & Assert
        foreach (char start in boundaryKeys)
        {
            foreach (char end in boundaryKeys)
            {
                if (start == end) continue;

                var path = _pathfinder.FindPath(start, end);

                Assert.NotNull(path);
                Assert.NotEmpty(path);
                Assert.Equal(start, path[0].Key);
                Assert.Equal(end, path[^1].Key);
            }
        }
    }

    [Fact]
    public void CalculateCost_ConsistentWithPathGeneration()
    {
        // Arrange
        char[] testKeys = new[] { 'a', 's', 'q', 'p', '1', '0', 'z', 'm' };

        // Act & Assert
        foreach (char start in testKeys)
        {
            foreach (char end in testKeys)
            {
                if (start == end) continue;

                var path = _pathfinder.FindPath(start, end);
                var directCost = _pathfinder.CalculateCost(start, end);

                // Verify path steps match or exceed direct cost
                double pathCost = path.Sum(p => p.Cost);

                Assert.True(pathCost >= directCost,
                    $"Path cost from {start} to {end} should be at least the direct cost");
            }
        }
    }

    [Fact]
    public void CalculateCost_MatchesManhattanDistance()
    {
        // Arrange
        char[] testKeys = new[] { 'a', 's', 'q', 'p', '1', '0', 'z', 'm' };

        // Act & Assert
        foreach (char start in testKeys)
        {
            foreach (char end in testKeys)
            {
                if (start == end) continue;

                var startPos = _keyboard.GetKeyPosition(start);
                var endPos = _keyboard.GetKeyPosition(end);

                // Sanity check for valid keys
                Assert.NotNull(startPos);
                Assert.NotNull(endPos);

                // Calculate expected Manhattan distance
                double expectedCost = Math.Abs(startPos.Row - endPos.Row) + Math.Abs(startPos.Col - endPos.Col);

                // Act
                var calculatedCost = _pathfinder.CalculateCost(start, end);

                // Assert
                Assert.Equal(expectedCost, calculatedCost);
            }
        }
    }

    [Fact]
    public void CalculateCost_InvalidKeys_ShouldReturnMaxValue()
    {
        // Act & Assert
        Assert.Equal(double.MaxValue, _pathfinder.CalculateCost(' ', 'a'));
        Assert.Equal(double.MaxValue, _pathfinder.CalculateCost('a', ' '));
    }
}
