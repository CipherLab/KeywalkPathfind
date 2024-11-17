using Xunit;
using Moq;
using System.Collections.Generic;
using KeyWalkAnalyzer3;

namespace KeyWalkAnalyzer3.Tests;
public class ShiftAwarePathFinderTests
{
    private readonly Mock<IKeyboardWeightStrategy> _mockStrategy;
    private readonly ShiftAwarePathFinder _pathFinder;

    public ShiftAwarePathFinderTests()
    {
        _mockStrategy = new Mock<IKeyboardWeightStrategy>();
        _pathFinder = new ShiftAwarePathFinder(_mockStrategy.Object);
    }

    // Helper method to set up mock behavior for movement and key press costs
    private void SetupMockStrategy(double movementCost, double pressCost)
    {
        _mockStrategy.Setup(s => s.CalculateMovementCost(It.IsAny<KeyPosition>(), It.IsAny<KeyPosition>(), It.IsAny<ShiftState>()))
                     .Returns(movementCost);
        _mockStrategy.Setup(s => s.CalculateKeyPressCost(It.IsAny<KeyPosition>(), It.IsAny<ShiftState>()))
                     .Returns(pressCost);
    }

    [Fact]
    public void FindPath_EmptySequence_ReturnsEmptyList()
    {
        // Act
        var path = _pathFinder.FindPath("");

        // Assert
        Assert.Empty(path);
    }
    [Theory]
    [InlineData("a", 3, false)]
    [InlineData("A", 5, true)]
    public void FindPath_SingleCharacter_ReturnsCorrectSteps(string sequence, int expectedStepCount, bool expectShiftSteps)
    {
        // Arrange
        SetupMockStrategy(1.0, 2.0); // Example costs

        // Act
        var path = _pathFinder.FindPath(sequence);

        // Assert
        Assert.Equal(expectedStepCount, path.Count);
        Assert.Equal(sequence[0], path[expectShiftSteps ? 2 : 1].Key);

        if (expectShiftSteps)
        {
            Assert.Equal("shift_down", path[0].Direction);
            Assert.Equal("shift_up", path[4].Direction);
        }
        else
        {
            Assert.DoesNotContain(path, step => step.Direction.Contains("shift"));
        }

        // Verify standard steps
        Assert.Equal("move", path[expectShiftSteps ? 1 : 0].Direction);
        Assert.Equal("press", path[expectShiftSteps ? 2 : 1].Direction);
        Assert.Equal("release", path[expectShiftSteps ? 3 : 2].Direction);
    }

    [Fact]
    public void FindPath_AdjacentKeys_NoShift()
    {
        // Arrange
        SetupMockStrategy(1.0, 2.0);
        string sequence = "ab";

        // Act
        var path = _pathFinder.FindPath(sequence);

        // Assert
        Assert.Equal(6, path.Count); // move, press, release for each character
        Assert.Equal('a', path[0].Key);
        Assert.Equal('b', path[3].Key);
    }

    [Fact]
    public void FindPath_MultipleShifts_OptimizesShiftUsage()
    {
        // Arrange
        SetupMockStrategy(1.0, 2.0);
        string sequence = "aBc";

        // Act
        var path = _pathFinder.FindPath(sequence);

        // Assert
        // Should only have one shift_down and one shift_up
        Assert.Single(path, s => s.Direction == "shift_down");
        Assert.Single(path, s => s.Direction == "shift_up");
    }

    [Fact]
    public void FindPath_ShiftCharacters_RecognizedCorrectly()
    {
        // Arrange
        SetupMockStrategy(1.0, 2.0);
        string sequence = "!@#$%^&*()";

        // Act
        var path = _pathFinder.FindPath(sequence);

        // Assert
        Assert.Equal((sequence.Length * 3) + 2, path.Count);

        // Verify shift steps
        var shiftDownSteps = path.Count(s => s.Direction == "shift_down");
        var shiftUpSteps = path.Count(s => s.Direction == "shift_up");

        Assert.Equal(1, shiftDownSteps);
        Assert.Equal(1, shiftUpSteps);

        // Verify step sequence for a few characters
        Assert.Equal("shift_down", path[0].Direction);
        Assert.Equal("move", path[1].Direction);
        Assert.Equal("press", path[2].Direction);
        Assert.Equal("release", path[3].Direction);
        Assert.Equal("shift_up", path[path.Count - 1].Direction);
    }
}