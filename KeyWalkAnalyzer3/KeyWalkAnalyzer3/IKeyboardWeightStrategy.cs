namespace KeyWalkAnalyzer3;

// Strategy pattern for different weighting schemes
public interface IKeyboardWeightStrategy
{
    double CalculateMovementCost(KeyPosition from, KeyPosition to, ShiftState shiftState);
    double CalculateKeyPressCost(KeyPosition key, ShiftState shiftState);
}