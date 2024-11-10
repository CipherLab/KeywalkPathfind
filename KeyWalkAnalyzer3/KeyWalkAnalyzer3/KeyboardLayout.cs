using System;
using System.Collections.Generic;
using System.Linq;

namespace KeyWalkAnalyzer3;

public class KeyboardLayout
{
    private Dictionary<char, KeyPosition> keyPositions = new Dictionary<char, KeyPosition>();
    private Dictionary<char, char> shiftVariants = new Dictionary<char, char>();
    private static readonly string[] QWERTY_LAYOUT = new[]
    {
        "`1234567890-=▒",
        "▒qwertyuiop[]\\",
        "▒asdfghjkl;'▒▒",
        "▒zxcvbnm,./▒▒▒"
    };

    public KeyboardLayout()
    {
        InitializeQwertyLayout();
        InitializeShiftVariants();
    }

    private void InitializeQwertyLayout()
    {
        keyPositions = new Dictionary<char, KeyPosition>();

        for (int row = 0; row < QWERTY_LAYOUT.Length; row++)
        {
            for (int col = 0; col < QWERTY_LAYOUT[row].Length; col++)
            {
                char key = QWERTY_LAYOUT[row][col];
                if (key != '▒')
                {
                    keyPositions[key] = new KeyPosition(row, col, key);
                }
            }
        }
    }

    private void InitializeShiftVariants()
    {
        shiftVariants = new Dictionary<char, char>
        {
            {'!', '1'},
            {'@', '2'},
            {'#', '3'},
            {'$', '4'},
            {'%', '5'},
            {'^', '6'},
            {'&', '7'},
            {'*', '8'},
            {'(', '9'},
            {')', '0'}
        };
    }

    public KeyPosition? GetKeyPosition(char key)
    {
        key = char.ToLower(key);
        if (shiftVariants.ContainsKey(key))
        {
            key = shiftVariants[key];
        }
        return keyPositions.ContainsKey(key) ? keyPositions[key] : null;
    }

    public List<char> GetHorizontalNeighbors(char key)
    {
        key = char.ToLower(key);
        var position = GetKeyPosition(key);
        if (position == null) return new List<char>();

        var row = QWERTY_LAYOUT[position.Row];
        int currentIndex = row.IndexOf(key);

        var neighbors = new List<char>();

        // Left neighbor
        for (int i = currentIndex - 1; i >= 0; i--)
        {
            if (row[i] != '▒')
            {
                neighbors.Add(row[i]);
                break;
            }
        }

        // Right neighbor
        for (int i = currentIndex + 1; i < row.Length; i++)
        {
            if (row[i] != '▒')
            {
                neighbors.Add(row[i]);
                break;
            }
        }

        return neighbors;
    }

    public char GetVerticalNeighbor(char key, int direction)
    {
        key = char.ToLower(key);
        var position = GetKeyPosition(key);
        if (position == null) throw new ArgumentException($"Invalid key: {key}");

        int newRow = position.Row + direction;

        // Ensure we stay within the layout bounds
        if (newRow < 0 || newRow >= QWERTY_LAYOUT.Length)
            throw new ArgumentException($"Cannot move vertically from {key}");

        // Try to find a key in the same column of the adjacent row
        string adjacentRow = QWERTY_LAYOUT[newRow];

        // If the current column is out of bounds in the adjacent row, 
        // use the closest available column
        int column = Math.Min(position.Col, adjacentRow.Length - 1);

        // Skip ▒ blocks
        while (column >= 0 && adjacentRow[column] == '▒')
        {
            column--;
        }

        return adjacentRow[column];
    }

    public char GetNextCharInRow(char currentChar, bool goingRight)
    {
        currentChar = char.ToLower(currentChar);
        var neighbors = GetHorizontalNeighbors(currentChar);

        if (!neighbors.Any())
            return currentChar;

        if (goingRight)
        {
            // If no right neighbor, wrap to the first available neighbor
            return neighbors.Count > 1 ? neighbors[1] : neighbors[0];
        }
        else
        {
            // If no left neighbor, wrap to the first available neighbor
            return neighbors[0];
        }
    }

    public char GetNextCharInColumn(char currentChar, bool goingDown)
    {
        currentChar = char.ToLower(currentChar);
        var pos = GetKeyPosition(currentChar);
        if (pos == null) return currentChar;

        // Create a column by extracting characters at the same column index across rows
        var column = new List<char>();
        for (int row = 0; row < QWERTY_LAYOUT.Length; row++)
        {
            if (pos.Col < QWERTY_LAYOUT[row].Length)
            {
                char candidate = QWERTY_LAYOUT[row][pos.Col];
                if (candidate != '▒')
                    column.Add(candidate);
            }
        }

        int currentIndex = column.IndexOf(currentChar);
        if (currentIndex == -1) return currentChar;

        if (goingDown)
        {
            // Move down, wrapping around if needed
            return currentIndex < column.Count - 1 ? column[currentIndex + 1] : column[0];
        }
        else
        {
            // Move up, wrapping around if needed
            return currentIndex > 0 ? column[currentIndex - 1] : column[column.Count - 1];
        }
    }
}
