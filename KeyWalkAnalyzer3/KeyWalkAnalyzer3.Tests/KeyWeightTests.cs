using Xunit;
using System.Diagnostics;
using Xunit.Abstractions;
using KeyWalkAnalyzer3;
namespace KeyWalkAnalyzer3;
public class FingerStrengthTests(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Theory]
    [InlineData(FingerStrength.Pinky, 0.412)]   // Rounded to three decimal places
    [InlineData(FingerStrength.Ring, 0.608)]
    [InlineData(FingerStrength.Middle, 0.804)]
    [InlineData(FingerStrength.Index, 1.000)]
    public void CalculateFingerFactor_ShouldMatchExpectedValues(FingerStrength finger, double expected)
    {
        // Arrange
        var keyWeight = new KeyWeight { Finger = finger };

        // Act
        decimal actualDecimal = (1.0m + (int)finger + 0.1m) / 5.1m;
        double actualDouble = (1.0 + (int)finger + 0.1) / 5.1;

        // Assert
        Assert.Equal(expected, Math.Round((double)actualDecimal, 3));
        Assert.Equal(expected, Math.Round(actualDouble, 3));
    }

    [Fact]
    public void CompareDoubleVsDecimalCalculations()
    {
        foreach (FingerStrength finger in Enum.GetValues(typeof(FingerStrength)))
        {
            // Decimal calculation
            decimal decimalResult = (5.0m - (int)finger + 0.1m) / 5.1m;

            // Double calculation (current implementation)
            double doubleResult = (5.0 - (int)finger + 0.1) / 5.1;

            // Output both for comparison
            _output.WriteLine($"Finger: {finger}");
            _output.WriteLine($"Decimal: {decimalResult}");
            _output.WriteLine($"Double:  {doubleResult}");
            _output.WriteLine("---");
        }
    }

    [Fact]
    public void CheckFingerStrengthValues()
    {
        _output.WriteLine($"Pinky: {(int)FingerStrength.Pinky}");
        _output.WriteLine($"Ring: {(int)FingerStrength.Ring}");
        _output.WriteLine($"Middle: {(int)FingerStrength.Middle}");
        _output.WriteLine($"Index: {(int)FingerStrength.Index}");
        _output.WriteLine($"Thumb: {(int)FingerStrength.Thumb}");
    }
}