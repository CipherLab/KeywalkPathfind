namespace KeyWalkAnalyzer3;

public class WeightedAStar(WeightedKeyboardLayout keyboard) : AStar(keyboard)
{
    private readonly WeightedKeyboardLayout weightedKeyboard = keyboard;

    public override double CalculateCost(char fromKey, char toKey)
    {
        return weightedKeyboard.GetMovementCost(fromKey, toKey);
    }

    public override List<PathStep> FindPath(char startKey, char endKey)
    {
        var path = base.FindPath(startKey, endKey);

        // Annotate path with effort metrics
        double totalEffort = 0;
        foreach (var step in path)
        {
            if (!step.IsPress)
            {
                totalEffort += weightedKeyboard.GetMovementCost(startKey, endKey);
            }
        }

        // Add effort metadata to path
        path[0].Metadata["TotalEffort"] = totalEffort;

        return path;
    }
}