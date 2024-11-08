namespace KeyWalkAnalyzer3;

public class EnhancedKeyPosition : KeyPosition
{
    public Hand PreferredHand { get; set; }
    public FingerStrength Finger { get; set; }
    public bool RequiresShift { get; set; }
    public double ReachDifficulty { get; set; }
    public bool IsHomeRow { get; set; }

    public EnhancedKeyPosition(
        int row,
        int col,
        char key,
        Hand preferredHand,
        FingerStrength finger,
        double reachDifficulty,
        bool isHomeRow,
        bool v) : base(row, col, key)
    {
        PreferredHand = preferredHand;
        Finger = finger;
        ReachDifficulty = reachDifficulty;
        IsHomeRow = isHomeRow;
        RequiresShift = char.IsUpper(key) || Utilities.IsShiftCharacter(key);
    }

}