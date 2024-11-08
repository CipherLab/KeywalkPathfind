namespace KeyWalkAnalyzer3;
public class AStar(KeyboardLayout keyboard)
{
    private readonly KeyboardLayout keyboard = keyboard;
    private static readonly (int row, int col)[] DIRECTIONS = new[]
    {
            (-1, 0),  // up
            (1, 0),   // down
            (0, -1),  // left
            (0, 1)    // right
        };

    public virtual List<PathStep> FindPath(char startKey, char endKey)
    {
        var start = keyboard.GetKeyPosition(startKey);
        var end = keyboard.GetKeyPosition(endKey);

        if (start == null || end == null)
            return new List<PathStep>();

        var path = new List<PathStep>();

        // Calculate relative position
        int rowDiff = end.Row - start.Row;
        int colDiff = end.Col - start.Col;

        // Add vertical movements
        bool isFirstStep = true;
        while (rowDiff != 0)
        {
            char directionKey = isFirstStep ? startKey : '\0'; // Set startKey for the first step
            string direction = rowDiff > 0 ? "down" : "up";
            path.Add(new PathStep(directionKey, direction, isPress: false));
            rowDiff += rowDiff > 0 ? -1 : 1;
            isFirstStep = false;
        }

        // Add horizontal movements
        while (colDiff != 0)
        {
            char directionKey = isFirstStep ? startKey : '\0'; // Set startKey for the first step
            string direction = colDiff > 0 ? "right" : "left";
            path.Add(new PathStep(directionKey, direction, isPress: false));
            colDiff += colDiff > 0 ? -1 : 1;
            isFirstStep = false;
        }

        // Add final press and release if start and end keys are the same
        if (startKey == endKey)
        {
            path.Add(new PathStep(startKey, "press", isPress: true));
            path.Add(new PathStep(startKey, "release", isPress: false));
        }
        else
        {
            // Add final press
            path.Add(new PathStep(endKey, "press", isPress: true));
        }

        return path;
    }
    public virtual double CalculateCost(char fromKey, char toKey)
    {
        var from = keyboard.GetKeyPosition(fromKey);
        var to = keyboard.GetKeyPosition(toKey);

        if (from == null || to == null)
            return double.MaxValue;

        return Math.Abs(from.Row - to.Row) + Math.Abs(from.Col - to.Col);
    }
}