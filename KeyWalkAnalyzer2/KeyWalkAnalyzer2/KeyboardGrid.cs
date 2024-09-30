namespace KeyWalkAnalyzer2;

public class KeyboardGrid
{
    public char[,] Grid { get; private set; }
    public int Rows { get; private set; }
    public int Columns { get; private set; }

    public KeyboardGrid(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Grid = new char[rows, columns];
    }

    public void SetLayout(char[,] layout)
    {
        if (layout.GetLength(0) != Rows || layout.GetLength(1) != Columns)
        {
            throw new ArgumentException("Layout dimensions do not match grid dimensions.");
        }
        Grid = layout;
    }

    // Example QWERTY layout (adjust as needed)
    public void SetQwertyLayout()
    {
        char[,] qwertyLayout = {
            { '`', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '=' },
            { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '[', ']', '\\' },
            { 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ';', '\'', '\0','\0' },
            { 'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.', '/', '\0', '\0','\0' }
        };
        SetLayout(qwertyLayout);
    }

    // Example DVORAK layout (adjust as needed)
    public void SetDvorakLayout()
    {
        char[,] dvorakLayout = {
            { '`', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '[', ']' },
            { '\'', ',', '.', 'p', 'y', 'f', 'g', 'c', 'r', 'l', '/', '=', '\\' },
            { 'a', 'o', 'e', 'u', 'i', 'd', 'h', 't', 'n', 's', '-', '\0','\0' },
            { ';', 'q', 'j', 'k', 'x', 'b', 'm', 'w', 'v', 'z', '\0','\0','\0' }
        };
        SetLayout(dvorakLayout);
    }

    public List<PathElement> GeneratePathElements(List<Tuple<int, int>> path, char start, char end)
    {
        List<PathElement> pathElements = new List<PathElement>();

        int lastMovementIndex = path.Count - 2; // Since we are moving from current to next, indices up to path.Count -2

        for (int i = 0; i <= lastMovementIndex; i++)
        {
            Tuple<int, int> current = path[i];
            Tuple<int, int> next = path[i + 1];

            Direction direction;

            // Determine direction
            if (next.Item1 < current.Item1)
            {
                direction = Direction.Up;
            }
            else if (next.Item1 > current.Item1)
            {
                direction = Direction.Down;
            }
            else if (next.Item2 < current.Item2)
            {
                direction = Direction.Left;
            }
            else
            {
                direction = Direction.Right;
            }

            // For the last movement, set Select and ShiftDown appropriately
            bool isLastMovement = (i == lastMovementIndex);
            bool select = isLastMovement;
            bool shiftDown = isLastMovement ? end.IsShiftedSymbol() : false;

            pathElements.Add(new PathElement
            {
                Direction = direction,
                Steps = 1,
                Select = select,
                ShiftDown = shiftDown,
            });
        }

        return pathElements;
    }


    internal Tuple<int, int> FindCharPosition(char v)
    {
        foreach (int i in Enumerable.Range(0, Rows))
        {
            foreach (int j in Enumerable.Range(0, Columns))
            {
                if (Grid[i, j] == v)
                {
                    return Tuple.Create(i, j);
                }
            }
        }

        return null;
    }
}