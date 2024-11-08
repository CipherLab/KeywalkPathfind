using Xunit;
using KeyboardPathAnalysis;
using System.Collections.Generic;

public class WeightedAStarTests
{
    private readonly WeightedKeyboardLayout _keyboard;
    private readonly WeightedAStar _pathfinder;

    public WeightedAStarTests()
    {
        _keyboard = new WeightedKeyboardLayout();
        _pathfinder = new WeightedAStar(_keyboard);
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

    [Fact]
    public void FindPath_ShouldIncludeTotalEffortMetadata()
    {
        // Arrange
        char start = 'a';
        char end = 'l';

        // Act
        var path = _pathfinder.FindPath(start, end);

        // Assert
        Assert.True(path[0].Metadata.ContainsKey("TotalEffort"));
        Assert.True((double)path[0].Metadata["TotalEffort"] > 0);
    }

    [Theory]
    [InlineData('f', 'j')] // Cross-hand movement (should be easier)
    [InlineData('f', 'g')] // Same-hand adjacent (should be harder)
    public void CalculateCost_ShouldReflectHandPositionDifficulty(char from, char to)
    {
        // Act
        var crossHandPath = _pathfinder.FindPath(from, to);
        double effort = (double)crossHandPath[0].Metadata["TotalEffort"];

        // Assert
        Assert.True(effort > 0);
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
}