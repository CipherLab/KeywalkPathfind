namespace KeyWalkAnalyzer3;

public class GamingKeyboardStrategy : BaseKeyboardWeightStrategy
{
    private readonly HashSet<char> gamingCriticalKeys = new HashSet<char>
    {
        'w', 'a', 's', 'd',
        '1', '2', '3', '4', '5',
        'q', 'e', 'r', 'f', 'g'
    };

    // Override base parameters for gaming-specific behavior
    protected override double SameFingersMovementPenalty => 1.2;
    protected override double CrossHandMovementBonus => 0.7;
    protected override double ShiftHandReachPenaltyFactor => 1.0;

    public override double CalculateKeyPressCost(KeyPosition key, ShiftState shiftState)
    {
        var enhancedKey = key as EnhancedKeyPosition ?? CreateDefaultEnhancedKeyPosition(key);
        double cost = 1.0;

        // Gaming keyboards have more responsive key mechanics
        cost *= (4 - (int)enhancedKey.Finger) / 3.0;  // More finger-friendly
        cost *= 1 + enhancedKey.ReachDifficulty * 0.7;  // Reduced reach penalty

        // Significant advantage for gaming-critical keys
        if (gamingCriticalKeys.Contains(char.ToLower(enhancedKey.Key)))
        {
            cost *= 0.6;  // Lower cost for gaming-critical keys
        }

        // Home row and gaming key cluster advantages
        if (enhancedKey.IsHomeRow || IsGamingKeyCluster(enhancedKey))
        {
            cost *= 0.7;
        }

        // Shift penalties with gaming keyboard considerations
        if (shiftState != ShiftState.NoShift)
        {
            bool isShiftHandConstrained =
                shiftState == ShiftState.LeftShift && enhancedKey.PreferredHand == Hand.Left ||
                shiftState == ShiftState.RightShift && enhancedKey.PreferredHand == Hand.Right;

            if (isShiftHandConstrained)
            {
                cost *= 1.2;  // Slightly reduced shift penalty
            }
        }

        return cost;
    }

    private bool IsGamingKeyCluster(EnhancedKeyPosition key)
    {
        // Define gaming key clusters based on typical gaming keyboard layouts
        return key.Row == 1 && key.Col >= 1 && key.Col <= 5 ||  // WASD area
               key.Row == 0 && key.Col >= 1 && key.Col <= 5;    // Number row near WASD
    }

    protected override EnhancedKeyPosition CreateDefaultEnhancedKeyPosition(KeyPosition key)
    {
        // Create a default EnhancedKeyPosition with gaming keyboard assumptions
        return new EnhancedKeyPosition(
            key.Row,
            key.Col,
            key.Key,
            Hand.Right,  // Default to right hand
            FingerStrength.Middle,  // Default to middle finger
            0.4,  // Reduced reach difficulty for gaming keyboards
            false  // Not home row by default
        );
    }

    // Optional: Override base method to customize gaming keyboard behavior
    protected override double CalculateBaseCost(EnhancedKeyPosition from, EnhancedKeyPosition to)
    {
        // Can add gaming-specific base cost calculations if needed
        return base.CalculateBaseCost(from, to);
    }
}
