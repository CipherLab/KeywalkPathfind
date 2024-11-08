namespace KeyWalkAnalyzer3;

public abstract class BaseKeyboardWeightStrategy : IKeyboardWeightStrategy
{
    // Configurable parameters that can be set by derived classes
    protected virtual double SameFingersMovementPenalty { get; } = 1.5;
    protected virtual double CrossHandMovementBonus { get; } = 0.8;
    protected virtual double ShiftHandReachPenaltyFactor { get; } = 1.5;
    protected virtual bool ApplyAwkwardHandPenalty { get; } = true;

    public double CalculateMovementCost(KeyPosition from, KeyPosition to, ShiftState shiftState)
    {
        var fromEnhanced = from as EnhancedKeyPosition ?? CreateDefaultEnhancedKeyPosition(from);
        var toEnhanced = to as EnhancedKeyPosition ?? CreateDefaultEnhancedKeyPosition(to);

        double cost = CalculateBaseCost(fromEnhanced, toEnhanced);

        if (shiftState != ShiftState.NoShift)
        {
            cost = ApplyShiftPenalties(cost, fromEnhanced, toEnhanced, shiftState);
        }

        return cost;
    }

    public abstract double CalculateKeyPressCost(KeyPosition key, ShiftState shiftState);

    protected virtual double CalculateBaseCost(EnhancedKeyPosition from, EnhancedKeyPosition to)
    {
        double distance = Math.Sqrt(
            Math.Pow(from.Row - to.Row, 2) +
            Math.Pow(from.Col - to.Col, 2)
        );

        // Same finger penalty
        if (from.Finger == to.Finger && from.PreferredHand == to.PreferredHand)
        {
            distance *= SameFingersMovementPenalty;
        }

        // Cross-hand movement bonus
        if (from.PreferredHand != to.PreferredHand)
        {
            distance *= CrossHandMovementBonus;
        }

        return distance;
    }

    protected virtual double ApplyShiftPenalties(double baseCost,
        EnhancedKeyPosition from,
        EnhancedKeyPosition to,
        ShiftState shiftState)
    {
        double cost = baseCost;
        Hand shiftHand = shiftState == ShiftState.LeftShift ? Hand.Left : Hand.Right;

        // Penalty if the shift hand needs to reach far
        if (to.PreferredHand == shiftHand)
        {
            cost *= 1 + to.ReachDifficulty * ShiftHandReachPenaltyFactor;
        }

        // Extra penalty for awkward combinations
        if (ApplyAwkwardHandPenalty &&
            from.PreferredHand == shiftHand &&
            to.PreferredHand == shiftHand)
        {
            cost *= 2.0;
        }

        return cost;
    }

    protected abstract EnhancedKeyPosition CreateDefaultEnhancedKeyPosition(KeyPosition key);
}
