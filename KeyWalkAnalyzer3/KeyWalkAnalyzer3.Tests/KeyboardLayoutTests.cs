using Xunit;

namespace KeyWalkAnalyzer3.Tests;

public class KeyboardLayoutTests
{
    private readonly KeyboardLayout _layout;

    public KeyboardLayoutTests()
    {
        _layout = new KeyboardLayout();
    }

    [Theory]
    [InlineData('q')]
    [InlineData('w')]
    [InlineData('e')]
    [InlineData('r')]
    [InlineData('t')]
    [InlineData('1')]
    [InlineData('@')]
    [InlineData('#')]
    public void GetKeyPosition_ValidKeys_ReturnsPosition(char key)
    {
        // Act
        var position = _layout.GetKeyPosition(key);

        // Assert
        Assert.NotNull(position);
        Assert.True(position.Row >= 0);
        Assert.True(position.Col >= 0);
    }

    [Theory]
    [InlineData('±')]
    [InlineData('§')]
    [InlineData('€')]
    public void GetKeyPosition_InvalidKeys_ReturnsNull(char key)
    {
        // Act
        var position = _layout.GetKeyPosition(key);

        // Assert
        Assert.Null(position);
    }

    [Theory]
    [InlineData('a', 'q')] // Vertical adjacent
    [InlineData('q', 'w')] // Horizontal adjacent
    [InlineData('z', 'x')] // Bottom row adjacent
    public void GetKeyPosition_AdjacentKeys_ReturnsAdjacentPositions(char key1, char key2)
    {
        // Act
        var pos1 = _layout.GetKeyPosition(key1);
        var pos2 = _layout.GetKeyPosition(key2);

        // Assert
        Assert.NotNull(pos1);
        Assert.NotNull(pos2);

        // Check if positions are adjacent (either horizontally or vertically)
        bool isAdjacent = (Math.Abs(pos1.Row - pos2.Row) <= 1 && pos1.Col == pos2.Col) ||
                         (Math.Abs(pos1.Col - pos2.Col) <= 1 && pos1.Row == pos2.Row);

        Assert.True(isAdjacent);
    }

    [Theory]
    [InlineData('A', 'a')] // Uppercase to lowercase
    [InlineData('!', '1')] // Shift symbol to number
    [InlineData('@', '2')] // Shift symbol to number
    public void GetKeyPosition_ShiftVariants_ShareSamePosition(char shiftKey, char baseKey)
    {
        // Act
        var shiftPos = _layout.GetKeyPosition(shiftKey);
        var basePos = _layout.GetKeyPosition(baseKey);

        // Assert
        Assert.NotNull(shiftPos);
        Assert.NotNull(basePos);
        Assert.Equal(shiftPos.Row, basePos.Row);
        Assert.Equal(shiftPos.Col, basePos.Col);
    }

    [Fact]
    public void GetKeyPosition_CaseInsensitive_ReturnsSamePosition()
    {
        // Arrange
        var letters = "abcdefghijklmnopqrstuvwxyz";

        foreach (char c in letters)
        {
            // Act
            var lowerPos = _layout.GetKeyPosition(c);
            var upperPos = _layout.GetKeyPosition(char.ToUpper(c));

            // Assert
            Assert.NotNull(lowerPos);
            Assert.NotNull(upperPos);
            Assert.Equal(lowerPos.Row, upperPos.Row);
            Assert.Equal(lowerPos.Col, upperPos.Col);
        }
    }
}
