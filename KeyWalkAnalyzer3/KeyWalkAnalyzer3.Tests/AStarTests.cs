using Xunit;
using System.Collections.Generic;
using KeyWalkAnalyzer3;
using System;

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
    [InlineData('a', 's')]
    [InlineData('f', 'j')]
    [InlineData('q', 'p')]
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
    [InlineData('a', 's')]
    [InlineData('f', 'j')]
    [InlineData('q', 'p')]
    public void FindPath_StepCountShouldMatchManhattanDistance(char start, char end)
    {
        // Arrange
        var startPos = _keyboard.GetKeyPosition(start);
        var endPos = _keyboard.GetKeyPosition(end);

        // Sanity check for valid keys
        Assert.NotNull(startPos);
        Assert.NotNull(endPos);

        // Calculate expected steps
        int expectedVerticalSteps = Math.Abs(startPos.Row - endPos.Row);
        int expectedHorizontalSteps = Math.Abs(startPos.Col - endPos.Col);
        int expectedTotalSteps = expectedVerticalSteps + expectedHorizontalSteps + 1; // +1 for final press

        // Act
        var path = _pathfinder.FindPath(start, end);

        // Assert
        Assert.Equal(expectedTotalSteps, path.Count);
    }

    [Fact]
    public void FindPath_SameKey_ShouldReturnMinimalPath()
    {
        // Arrange
        char key = 'a';

        // Act
        var path = _pathfinder.FindPath(key, key);

        // Assert
        Assert.NotNull(path);
        Assert.Equal(2, path.Count); // Press and release only
        Assert.Equal(key, path[0].Key);
        Assert.Equal(key, path[1].Key);
        Assert.True(path[0].IsPress, "First step should be a press action.");
        Assert.False(path[1].IsPress, "Second step should be a release action.");
    }

    [Fact]
    public void FindPath_InvalidKeys_ShouldReturnEmptyPath()
    {
        // Act
        var path1 = _pathfinder.FindPath(' ', 'a'); // Invalid start key
        var path2 = _pathfinder.FindPath('a', ' '); // Invalid end key

        // Assert
        Assert.Empty(path1);
        Assert.Empty(path2);
    }

    [Theory]
    [InlineData('a', 's')]
    [InlineData('f', 'j')]
    [InlineData('q', 'p')]
    public void CalculateCost_ShouldMatchManhattanDistance(char start, char end)
    {
        // Arrange
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

    [Fact]
    public void CalculateCost_InvalidKeys_ShouldReturnMaxValue()
    {
        // Act & Assert
        Assert.Equal(double.MaxValue, _pathfinder.CalculateCost(' ', 'a'));
        Assert.Equal(double.MaxValue, _pathfinder.CalculateCost('a', ' '));
    }
}
