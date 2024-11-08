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

        // If start and end are different keys, add release step for start key
        if (startKey != endKey)
        {
            path.Add(new PathStep(startKey, "release", isPress: false) { Cost = 0.5 });
        }

        // Calculate relative position
        int rowDiff = end.Row - start.Row;
        int colDiff = end.Col - start.Col;

        // Consolidate vertical movements
        if (rowDiff != 0)
        {
            string direction = rowDiff > 0 ? "down" : "up";
            int steps = Math.Abs(rowDiff);
            path.Add(new PathStep(startKey, direction, isPress: false)
            {
                Cost = steps * 1.0,
                Metadata = new Dictionary<string, object> { { "steps", steps } }
            });
        }

        // Consolidate horizontal movements
        if (colDiff != 0)
        {
            string direction = colDiff > 0 ? "right" : "left";
            int steps = Math.Abs(colDiff);
            path.Add(new PathStep('\0', direction, isPress: false)
            {
                Cost = steps * 1.0,
                Metadata = new Dictionary<string, object> { { "steps", steps } }
            });
        }

        // Add final press for end key
        if (startKey == endKey)
        {
            // For same key, add press and release
            path.Add(new PathStep(startKey, "press", isPress: true) { Cost = 0.5 });
            path.Add(new PathStep(startKey, "release", isPress: false) { Cost = 0.5 });
        }
        else
        {
            // For different keys, add press of end key
            path.Add(new PathStep(endKey, "press", isPress: true) { Cost = 1.0 });
        }

        return path;
    }

    public virtual double CalculateCost(char fromKey, char toKey)
    {
        var from = keyboard.GetKeyPosition(fromKey);
        var to = keyboard.GetKeyPosition(toKey);

        if (from == null || to == null)
            return double.MaxValue;

        // Pure Manhattan distance
        return Math.Abs(from.Row - to.Row) + Math.Abs(from.Col - to.Col);
    }
}
