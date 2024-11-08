namespace KeyWalkAnalyzer3;

public class StandardWeightStrategy : BaseKeyboardWeightStrategy
{
    private readonly Dictionary<char, double> languageFrequencies;

    public StandardWeightStrategy()
    {
        languageFrequencies = new Dictionary<char, double>
        {
            {'e', 0.111607}, {'a', 0.084966}, {'r', 0.075809},
            // ... other frequencies ...
        };
    }

    public override double CalculateKeyPressCost(KeyPosition key, ShiftState shiftState)
    {
        var enhancedKey = key as EnhancedKeyPosition ?? CreateDefaultEnhancedKeyPosition(key);
        double cost = 1.0;

        // Base difficulty from finger strength and reach
        cost *= (5 - (int)enhancedKey.Finger) / 4.0;
        cost *= 1 + enhancedKey.ReachDifficulty;

        // Home row advantage
        if (enhancedKey.IsHomeRow) cost *= 0.8;

        // Shift penalties
        if (shiftState != ShiftState.NoShift)
        {
            bool isShiftHandConstrained =
                shiftState == ShiftState.LeftShift && enhancedKey.PreferredHand == Hand.Left ||
                shiftState == ShiftState.RightShift && enhancedKey.PreferredHand == Hand.Right;

            if (isShiftHandConstrained)
            {
                cost *= 1.5; // Significant penalty for using shifted hand
            }
        }

        return cost;
    }

    protected override EnhancedKeyPosition CreateDefaultEnhancedKeyPosition(KeyPosition key)
    {
        // Create a default EnhancedKeyPosition with minimal assumptions
        return new EnhancedKeyPosition(
            key.Row,
            key.Col,
            key.Key,
            Hand.Right,  // Default to right hand
            FingerStrength.Middle,  // Default to middle finger
            0.5,  // Moderate reach difficulty
            false  // Not home row by default
        );
    }

    // Optional: Override base method to customize standard keyboard behavior
    protected override double CalculateBaseCost(EnhancedKeyPosition from, EnhancedKeyPosition to)
    {
        // Can add standard keyboard-specific base cost calculations if needed
        return base.CalculateBaseCost(from, to);
    }
}
