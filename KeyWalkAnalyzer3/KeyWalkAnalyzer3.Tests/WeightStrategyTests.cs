using Xunit;

namespace KeyWalkAnalyzer3.Tests;

public class WeightStrategyTests
{
    private readonly StandardWeightStrategy _standardStrategy;
    private readonly GamingKeyboardStrategy _gamingStrategy;
    private readonly MobileKeyboardStrategy _mobileStrategy;
    private readonly KeyboardLayout _layout;

    public WeightStrategyTests()
    {
        _standardStrategy = new StandardWeightStrategy();
        _gamingStrategy = new GamingKeyboardStrategy();
        _mobileStrategy = new MobileKeyboardStrategy();
        _layout = new KeyboardLayout();
    }

    [Theory]
    [InlineData('a', 's')] // Adjacent keys
    [InlineData('q', 'p')] // Far apart keys
    [InlineData('z', 'm')] // Bottom row keys
    public void CalculateMovementCost_DifferentStrategies_ConsistentResults(char from, char to)
    {
        // Arrange
        var fromPos = _layout.GetKeyPosition(from);
        var toPos = _layout.GetKeyPosition(to);
        var shiftState = ShiftState.NoShift;

        // Act
        var standardCost = _standardStrategy.CalculateMovementCost(fromPos, toPos, shiftState);
        var gamingCost = _gamingStrategy.CalculateMovementCost(fromPos, toPos, shiftState);
        var mobileCost = _mobileStrategy.CalculateMovementCost(fromPos, toPos, shiftState);

        // Assert
        Assert.True(standardCost > 0);
        Assert.True(gamingCost > 0);
        Assert.True(mobileCost > 0);
    }

    [Theory]
    [InlineData('a')]
    [InlineData('A')] // Shifted key
    [InlineData('1')]
    [InlineData('!')] // Shifted number
    public void CalculateKeyPressCost_DifferentStrategies_ConsistentResults(char key)
    {
        // Arrange
        var keyPos = _layout.GetKeyPosition(key);
        var shiftState = char.IsUpper(key) || "!@#$%^&*()".Contains(key)
            ? ShiftState.LeftShift
            : ShiftState.NoShift;

        // Act
        var standardCost = _standardStrategy.CalculateKeyPressCost(keyPos, shiftState);
        var gamingCost = _gamingStrategy.CalculateKeyPressCost(keyPos, shiftState);
        var mobileCost = _mobileStrategy.CalculateKeyPressCost(keyPos, shiftState);

        // Assert
        Assert.True(standardCost > 0);
        Assert.True(gamingCost > 0);
        Assert.True(mobileCost > 0);
    }

    [Fact]
    public void CalculateMovementCost_SamePosition_ReturnsZero()
    {
        // Arrange
        var pos = _layout.GetKeyPosition('a');
        var shiftState = ShiftState.NoShift;

        // Act
        var standardCost = _standardStrategy.CalculateMovementCost(pos, pos, shiftState);
        var gamingCost = _gamingStrategy.CalculateMovementCost(pos, pos, shiftState);
        var mobileCost = _mobileStrategy.CalculateMovementCost(pos, pos, shiftState);

        // Assert
        Assert.Equal(0, standardCost);
        Assert.Equal(0, gamingCost);
        Assert.Equal(0, mobileCost);
    }

    [Theory]
    [InlineData('q', 'p')] // Far horizontal movement
    [InlineData('6', 'm')] // Far vertical movement
    [InlineData('5', 'm')] // Diagonal movement
    public void CalculateMovementCost_LongerDistance_HigherCost(char from, char to)
    {
        // Arrange
        var fromPos = _layout.GetKeyPosition(from);
        var toPos = _layout.GetKeyPosition(to);
        var middlePos = _layout.GetKeyPosition('h'); // Middle of keyboard
        var shiftState = ShiftState.NoShift;

        // Act & Assert for each strategy
        void TestStrategy(IKeyboardWeightStrategy strategy)
        {
            var shortMove = strategy.CalculateMovementCost(fromPos, middlePos, shiftState);
            var longMove = strategy.CalculateMovementCost(fromPos, toPos, shiftState);
            Assert.True(longMove >= shortMove);
        }

        TestStrategy(_standardStrategy);
        TestStrategy(_gamingStrategy);
        TestStrategy(_mobileStrategy);
    }

    [Theory]
    [InlineData('a', ShiftState.LeftShift)]
    [InlineData('a', ShiftState.RightShift)]
    public void CalculateKeyPressCost_WithShift_HigherCost(char key, ShiftState shiftState)
    {
        // Arrange
        var keyPos = _layout.GetKeyPosition(key);

        // Act & Assert for each strategy
        void TestStrategy(IKeyboardWeightStrategy strategy)
        {
            var normalCost = strategy.CalculateKeyPressCost(keyPos, ShiftState.NoShift);
            var shiftCost = strategy.CalculateKeyPressCost(keyPos, shiftState);
            Assert.True(shiftCost >= normalCost);
        }

        TestStrategy(_standardStrategy);
        TestStrategy(_gamingStrategy);
        TestStrategy(_mobileStrategy);
    }
}
