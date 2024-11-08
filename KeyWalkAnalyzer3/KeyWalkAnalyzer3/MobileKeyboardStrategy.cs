namespace KeyWalkAnalyzer3;

public class MobileKeyboardStrategy : BaseKeyboardWeightStrategy
{
    private readonly HashSet<char> frequentMobileKeys = new HashSet<char>
    {
        'e', 'a', 'r', 'i', 'o', 'n',
        ' ', // Space bar is critical in mobile typing
        '.' // Punctuation is frequent
    };

    // Override base parameters for mobile-specific behavior
    protected override double SameFingersMovementPenalty => 1.7;
    protected override double CrossHandMovementBonus => 0.9;
    protected override double ShiftHandReachPenaltyFactor => 1.2;

    public override double CalculateKeyPressCost(KeyPosition key, ShiftState shiftState)
    {
        var enhancedKey = key as EnhancedKeyPosition ?? CreateDefaultEnhancedKeyPosition(key);
        double cost = 1.0;

        // Mobile keyboard thumb-based typing dynamics
        cost *= (4 - (int)enhancedKey.Finger) / 3.5;  // Thumb-centric weighting
        cost *= 1 + enhancedKey.ReachDifficulty * 1.2;  // Higher reach difficulty

        // Frequent mobile typing keys get slight advantage
        if (frequentMobileKeys.Contains(char.ToLower(enhancedKey.Key)))
        {
            cost *= 0.8;  // Slight cost reduction for common keys
        }

        // Thumb-friendly zones
        if (IsThumbFriendlyZone(enhancedKey))
        {
            cost *= 0.7;  // Lower cost in thumb-friendly areas
        }

        // Shift penalties with mobile keyboard considerations
        if (shiftState != ShiftState.NoShift)
        {
            bool isShiftHandConstrained =
                shiftState == ShiftState.LeftShift && enhancedKey.PreferredHand == Hand.Left ||
                shiftState == ShiftState.RightShift && enhancedKey.PreferredHand == Hand.Right;

            if (isShiftHandConstrained)
            {
                cost *= 1.3;  // Moderate shift penalty
            }
        }

        return cost;
    }

    private bool IsThumbFriendlyZone(EnhancedKeyPosition key)
    {
        // Define thumb-friendly zones typical in mobile keyboards
        // Assumes a layout where bottom rows are more thumb-accessible
        return key.Row >= 2 && key.Row <= 3 &&  // Bottom rows
               key.Col >= 2 && key.Col <= 8;    // Central key area
    }

    protected override EnhancedKeyPosition CreateDefaultEnhancedKeyPosition(KeyPosition key)
    {
        // Create a default EnhancedKeyPosition with mobile keyboard assumptions
        return new EnhancedKeyPosition(
            key.Row,
            key.Col,
            key.Key,
            Hand.Right,  // Default to right hand (thumb-typing assumption)
            FingerStrength.Thumb,  // Thumb as primary typing finger
            0.6,  // Higher reach difficulty for mobile keyboards
            false  // Not home row by default
        );
    }

    // Optional: Override base method to customize mobile keyboard behavior
    protected override double CalculateBaseCost(EnhancedKeyPosition from, EnhancedKeyPosition to)
    {
        // Can add mobile-specific base cost calculations if needed
        return base.CalculateBaseCost(from, to);
    }
}
