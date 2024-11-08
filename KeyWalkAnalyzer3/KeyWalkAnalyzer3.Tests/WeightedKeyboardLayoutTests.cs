using Xunit;
using KeyWalkAnalyzer3;

public class WeightedKeyboardLayoutTests
{
    private readonly WeightedKeyboardLayout _layout;

    public WeightedKeyboardLayoutTests()
    {
        _layout = new WeightedKeyboardLayout();
    }

    [Theory]
    [InlineData('a', 's', 1.0)] // Adjacent keys
    [InlineData('q', 'p', 2.0)] // Far apart keys
    [InlineData('z', 'm', 2.0)] // Bottom row keys
    public void GetMovementCost_ShouldReturnExpectedCost(char fromKey, char toKey, double expectedCost)
    {
        // Act
        double cost = _layout.GetMovementCost(fromKey, toKey);

        // Assert
        Assert.True(cost > 0);
    }

    [Theory]
    [InlineData('a', 'a')] // Same key
    public void GetMovementCost_SameKey_ShouldReturnZero(char fromKey, char toKey)
    {
        // Act
        double cost = _layout.GetMovementCost(fromKey, toKey);

        // Assert
        Assert.Equal(0, cost);
    }

    [Theory]
    [InlineData('a', 'A')] // Shifted key
    [InlineData('1', '!')] // Shifted number
    public void GetMovementCost_WithShift_ShouldReturnHigherCost(char fromKey, char toKey)
    {
        // Act
        double normalCost = _layout.GetMovementCost(fromKey, char.ToLower(toKey));
        double shiftCost = _layout.GetMovementCost(fromKey, toKey);

        // Assert
        Assert.True(shiftCost >= normalCost);
    }

    [Fact]
    public void InitializeWeights_ShouldSetKeyWeights()
    {
        // Act
        var keyWeightsField = _layout.GetType().GetField("keyWeights", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(keyWeightsField);

        var keyWeights = keyWeightsField.GetValue(_layout) as Dictionary<char, KeyWeight>;
        Assert.NotNull(keyWeights);

        // Assert
        Assert.NotEmpty(keyWeights);
    }
}