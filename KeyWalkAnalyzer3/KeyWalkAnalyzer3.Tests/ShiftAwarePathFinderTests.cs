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
    [InlineData("a")]
    [InlineData("A")]
    public void FindPath_SingleCharacter_ReturnsCorrectSteps(string sequence)
    {
        // Arrange
        SetupMockStrategy(1.0, 2.0); // Example costs

        // Act
        var path = _pathFinder.FindPath(sequence);

        // Assert
        Assert.Equal(char.IsUpper(sequence[0]) ? 4 : 3, path.Count);

        if (char.IsUpper(sequence[0]))
        {
            Assert.Equal($"shift_down for '{sequence[0]}'", path[0].ToString());
        }

        Assert.Equal("move", path[char.IsUpper(sequence[0]) ? 1 : 0].Direction);
        Assert.Equal("press", path[char.IsUpper(sequence[0]) ? 2 : 1].Direction);
        Assert.Equal("release", path[char.IsUpper(sequence[0]) ? 3 : 2].Direction);
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
    public void FindPath_WithShift_AppliesShiftCorrectly()
    {
        // Arrange
        SetupMockStrategy(1.0, 2.0);
        string sequence = "As";

        // Act
        var path = _pathFinder.FindPath(sequence);

        // Assert
        Assert.Equal(7, path.Count);
        Assert.Equal($"shift_down for 'A'", path[0].ToString());
        Assert.Equal("shift_up", path[6].Direction);
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
        Assert.Equal(sequence.Length * 4, path.Count); // 4 steps per character
        Assert.Equal(sequence.Length, path.Count(s => s.Direction == "shift_down"));
    }
}