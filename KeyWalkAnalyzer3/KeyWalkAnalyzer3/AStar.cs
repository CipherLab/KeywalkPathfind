namespace KeyboardPathAnalysis
{
    public class AStar
    {
        private readonly KeyboardLayout keyboard;
        private static readonly (int row, int col)[] DIRECTIONS = new[]
        {
            (-1, 0),  // up
            (1, 0),   // down
            (0, -1),  // left
            (0, 1)    // right
        };

        public AStar(KeyboardLayout keyboard)
        {
            this.keyboard = keyboard;
        }
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
            while (rowDiff != 0)
            {
                path.Add(new PathStep(rowDiff > 0 ? "down" : "up"));
                rowDiff += rowDiff > 0 ? -1 : 1;
            }

            // Add horizontal movements
            while (colDiff != 0)
            {
                path.Add(new PathStep(colDiff > 0 ? "right" : "left"));
                colDiff += colDiff > 0 ? -1 : 1;
            }

            // Add final press
            path.Add(new PathStep("", true));

            return path;
        }
        protected virtual double CalculateCost(char fromKey, char toKey)
        {
            var from = keyboard.GetKeyPosition(fromKey);
            var to = keyboard.GetKeyPosition(toKey);

            if (from == null || to == null)
                return double.MaxValue;

            return Math.Abs(from.Row - to.Row) + Math.Abs(from.Col - to.Col);
        }
    }
}