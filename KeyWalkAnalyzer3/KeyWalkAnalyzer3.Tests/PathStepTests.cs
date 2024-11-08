using Xunit;
using KeyWalkAnalyzer3;
using System.Collections.Generic;

namespace KeyWalkAnalyzer3.Tests;
public class PathStepTests
{
    [Fact]
    public void Constructor_InitializesPropertiesCorrectly()
    {
        // Arrange
        char key = 'a';
        string direction = "up";
        bool isPress = true;

        // Act
        var step = new PathStep(key, direction, isPress);

        // Assert
        Assert.Equal(key, step.Key);
        Assert.Equal(direction, step.Direction);
        Assert.True(step.IsPress);
        Assert.NotNull(step.Metadata);
        Assert.Empty(step.Metadata);
    }

    [Fact]
    public void ToString_ReturnsCorrectStringRepresentation()
    {
        // Arrange
        var pressStep = new PathStep('a', "press", true);
        var moveStep = new PathStep('\0', "down", false);

        // Act
        string pressString = pressStep.ToString();
        string moveString = moveStep.ToString();

        // Assert
        Assert.Equal("press 'a'", pressString);
        Assert.Equal("down", moveString);
    }

    [Fact]
    public void ToString_HandlesNullKeyCorrectly()
    {
        // Arrange
        var step = new PathStep('\0', "left", false);

        // Act
        string stepString = step.ToString();

        // Assert
        Assert.Equal("left", stepString);
    }
}
