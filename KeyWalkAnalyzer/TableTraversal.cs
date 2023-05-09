// See https://aka.ms/new-console-template for more information
public class TableTraversal
{
    private char[,] _table { get; }
    private Helpers _helper { get; }

    public TableTraversal(char[,] table)
    {
        _table = table;
        _helper = new Helpers();
    }

    public string BuildString(char startChar, Command[] pattern, int length)
    {
        Position currentPosition = FindStartPosition(startChar);
        Position startPosition = currentPosition;
        if (currentPosition.Col == -1)
        {
            var tempchar = _helper.GetOppositeShiftCharacter(startChar);
            currentPosition = FindStartPosition(tempchar);
            if (currentPosition.Col == -1)
            {
                throw new ArgumentException("Invalid starting character.");
            }
        }
        //pattern = pattern.Take(pattern.Length - 1).ToArray();
        var result = startChar.ToString(); // Include the starting character in the output
        var patternIndex = 0;
        bool edgeLimit = false;
        while (result.Length <= length && !edgeLimit)
        {
            var currentCommand = pattern[patternIndex];
            switch (currentCommand.Direction)
            {
                case "↑":
                    // currentPosition.Row = (currentPosition.Row - 1 + _table.GetLength(0)) % _table.GetLength(0);
                    if (currentPosition.Row - 1 < 0)
                    {
                        currentPosition.Row = _table.GetLength(0) - 1;
                        edgeLimit = true;
                    }
                    else
                    {
                        currentPosition.Row = currentPosition.Row - 1;
                    }

                    break;

                case "↓":
                    //currentPosition.Row = (currentPosition.Row + 1) % _table.GetLength(0);
                    if (currentPosition.Row + 1 >= _table.GetLength(0))
                    {
                        currentPosition.Row = 0;
                        edgeLimit = true;
                    }
                    else
                    {
                        currentPosition.Row = currentPosition.Row + 1;
                    }

                    break;

                case "←":
                    if (currentPosition.Col == 0)
                    {
                        // currentPosition.Col = _table.GetLength(1) - 1;
                        currentPosition.Col = _table.GetLength(1) - 1;
                        edgeLimit = true;
                    }
                    else
                    {
                        currentPosition.Col -= 1;
                    }
                    break;

                case "→":
                    if (currentPosition.Col == _table.GetLength(1) - 1)
                    {
                        currentPosition.Col = 0;
                        edgeLimit = true;
                    }
                    else
                    {
                        currentPosition.Col += 1;
                    }
                    break;

                case "◘":

                    break;

                default:
                    throw new ArgumentException("Invalid pattern command.");
            }

            if (currentCommand.Take)
            {
                var tempval = _table[currentPosition.Row, currentPosition.Col];
                if (currentCommand.IsShift)
                    tempval = _helper.GetOppositeShiftCharacter(tempval);
                result += tempval;
            }

            patternIndex = (patternIndex + 1) % pattern.Length;
        }
        result = result.Replace(" ", "");
        if (result.Length > length)
            result = result.Substring(0, length);
        if (result.Length < length)
            return string.Empty;
        return result;
    }

    private Position FindStartPosition(char startChar)
    {
        for (int i = 0; i < _table.GetLength(0); i++)
        {
            for (int j = 0; j < _table.GetLength(1); j++)
            {
                if (_table[i, j] == startChar)
                {
                    return new Position(i, j);
                }
            }
        }

        return new Position(-1, -1);
    }

    public struct Position
    {
        public int Row;
        public int Col;

        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
}