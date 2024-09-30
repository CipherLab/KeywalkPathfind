using System.Text;
using KeyWalkAnalyzer2;

public class PathFinder
{
    public PathFinder(char[,] table)
    {
        _table = table;
    }

    private char[,] _table { get; }

    public List<Tuple<int, int>> AStarPathFinding(char start, char end)
    {
        // Get the start and end positions
        var startCharConverted = start.ConvertToNonShift();
        var endCharConverted = end.ConvertToNonShift();


        Tuple<int, int> startPos = FindCharPosition(start);
        Tuple<int, int> endPos = FindCharPosition(end);

        // If the start and end positions are the same, return a list with two duplicate positions
        // Check if endPos is null and return an empty list if it is
        if (endPos == null)
        {
            return new List<Tuple<int, int>>();
        }
        if (startPos == null)
        {
            return new List<Tuple<int, int>>();
        }

        if (startPos.Equals(endPos))
        {
            return new List<Tuple<int, int>> { startPos, endPos };
        }

        // Create the open and closed lists
        HashSet<Tuple<int, int>> openList = new HashSet<Tuple<int, int>>();
        HashSet<Tuple<int, int>> closedList = new HashSet<Tuple<int, int>>();

        // Initialize the gScore and fScore dictionaries
        Dictionary<Tuple<int, int>, double> gScore = new Dictionary<Tuple<int, int>, double>();
        Dictionary<Tuple<int, int>, double> fScore = new Dictionary<Tuple<int, int>, double>();

        // Set the initial gScore and fScore for the start position
        gScore[startPos] = 0;
        fScore[startPos] = HeuristicCostEstimate(startPos, endPos);

        // Add the start position to the open list
        openList.Add(startPos);

        // The cameFrom dictionary will store the best previous position for each position
        Dictionary<Tuple<int, int>, Tuple<int, int>> cameFrom = new Dictionary<Tuple<int, int>, Tuple<int, int>>();

        while (openList.Count > 0)
        {
            // Get the position with the lowest fScore
            Tuple<int, int> current = openList.OrderBy(p => fScore[p]).First();

            // If we reached the end position, reconstruct the path
            if (current.Equals(endPos))
            {
                return ReconstructPath(cameFrom, current);
            }

            openList.Remove(current);
            closedList.Add(current);

            // Iterate through each neighbor of the current position
            foreach (Tuple<int, int> neighbor in GetNeighbors(current))
            {
                if (closedList.Contains(neighbor)) continue;

                double tentativeGScore = gScore[current] + 1;

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    // This is not a better path, skip it
                    continue;
                }
                // This path is the best so far, record it
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, endPos);
            }
        }

        // If we get here, there is no path
        return null;
    }

    public List<Tuple<int, int>> ReconstructPath(Dictionary<Tuple<int, int>, Tuple<int, int>> cameFrom, Tuple<int, int> current)
    {
        List<Tuple<int, int>> path = new List<Tuple<int, int>>();
        path.Add(current);

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }

        // Reverse the path to get the correct order
        path.Reverse();

        return path;
    }

    public Tuple<int, int> FindCharPosition(char c)
    {
        for (int i = 0; i < _table.GetLength(0); i++)
        {
            for (int j = 0; j < _table.GetLength(1); j++)
            {
                if (_table[i, j] == c)
                {
                    return Tuple.Create(i, j);
                }
            }
        }

        return null;
    }

    public List<Tuple<int, int>> GetNeighbors(Tuple<int, int> position)
    {
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        List<Tuple<int, int>> neighbors = new List<Tuple<int, int>>();

        for (int i = 0; i < 4; i++)
        {
            int newRow = position.Item1 + dx[i];
            int newCol = position.Item2 + dy[i];

            if (newRow >= 0 && newRow < _table.GetLength(0) && newCol >= 0 && newCol < _table.GetLength(1))
            {
                neighbors.Add(Tuple.Create(newRow, newCol));
            }
        }

        return neighbors;
    }

    public double HeuristicCostEstimate(Tuple<int, int> start, Tuple<int, int> end)
    {
        return Math.Abs(start.Item1 - end.Item1) + Math.Abs(start.Item2 - end.Item2);
    }


}
