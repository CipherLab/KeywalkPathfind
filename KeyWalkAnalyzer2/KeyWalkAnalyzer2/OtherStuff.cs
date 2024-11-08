using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KeyWalkAnalyzer2;
public struct PathElement
{
    public Direction Direction;
    public int Steps;
    public bool Select;
    public bool ShiftDown;
    public override bool Equals(object obj)
    {
        if (obj is PathElement other)
        {
            return Direction == other.Direction &&
                   Select == other.Select &&
                   ShiftDown == other.ShiftDown;
        }
        return false;
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}


public static class CharExtensions

{
    public static bool IsShiftedSymbol(this char c)

    {

        switch (c)
        {
            case '!':
            case '@':
            case '#':
            case '$':
            case '%':
            case '^':
            case '&':
            case '*':
            case '(':
            case ')':
            case '_':
            case '+':
            case '{':
            case '}':
            case '|':
            case ':':
            case '"':
            case '<':
            case '>':
            case '?':
            case '~':
                return true;
            default:
                {
                    var retval = char.IsUpper(c);
                    return retval;
                }
        }

    }
    public static char ConvertToShift(this char c)

    {

        switch (c)
        {
            case '1': return '!';
            case '2': return '@';
            case '3': return '#';
            case '4': return '$';
            case '5': return '%';
            case '6': return '^';
            case '7': return '&';
            case '8': return '*';
            case '9': return '(';
            case '0': return ')';
            case '-': return '_';
            case '=': return '+';
            case '[': return '{';
            case ']': return '}';
            case '\\': return '|';
            case ';': return ':';
            case '\'': return '"';
            case ',': return '<';
            case '.': return '>';
            case '/': return '?';
            default: return char.ToLower(c);
        }

    }
    public static char ConvertToNonShift(this char c)

    {

        switch (c)
        {
            case '!': return '1';
            case '@': return '2';
            case '#': return '3';
            case '$': return '4';
            case '%': return '5';
            case '^': return '6';
            case '&': return '7';
            case '*': return '8';
            case '(': return '9';
            case ')': return '0';
            case '_': return '-';
            case '+': return '=';
            case '{': return '[';
            case '}': return ']';
            case '|': return '\\';
            case ':': return ';';
            case '"': return '\'';
            case '<': return ',';
            case '>': return '.';
            case '?': return '/';
            default: return char.ToUpper(c);
        }

    }

}

public class Helpers
{

    public static List<PathElement> ReducePath(List<PathElement> path)
    {
        KMP kMP = new KMP();
        var fullPathString = PathToString(path);
        var smallestRepeating = kMP.GetSmallestRepeatingPattern(fullPathString);

        Console.WriteLine($"Smallest Repeating Pattern: {smallestRepeating}");
        var retval = StringToPath(smallestRepeating);
        var updatedRetval = new List<PathElement>();
        foreach (PathElement element in retval)
        {
            var str = PathToString(element);
            var updatedElement = element;
            updatedElement.Steps = 1;
            //get a count of how many this element is repeated in the original repeating pattern
            updatedRetval.Add(updatedElement);
        }
        retval = updatedRetval;
        return retval;
    }
    public static int CountSubstringOccurrences(string text, string substring)
    {
        int count = 0;
        int index = 0;

        while ((index = text.IndexOf(substring, index)) != -1)
        {
            count++;
            index += substring.Length;
        }

        return count;
    }
    public static string ExecuteReducedPath(
        char startingChar, List<PathElement> reducedPath, KeyboardGrid keyboard)
    {
        StringBuilder outputString = new StringBuilder();
        outputString.Append(startingChar); // Add the starting character
        if (startingChar.IsShiftedSymbol())
            startingChar = startingChar.ConvertToNonShift();

        Tuple<int, int> currentPosition = keyboard.FindCharPosition(startingChar);
        bool shiftIsDown = startingChar.IsShiftedSymbol(); // Track Shift key state

        foreach (PathElement element in reducedPath)
        {
            for (int i = 0; i < element.Steps; i++)
            {
                // Move based on direction
                switch (element.Direction)
                {
                    case Direction.Up:
                        currentPosition = Tuple.Create(currentPosition.Item1 - 1, currentPosition.Item2);
                        break;
                    case Direction.Down:
                        currentPosition = Tuple.Create(currentPosition.Item1 + 1, currentPosition.Item2);
                        break;
                    case Direction.Left:
                        currentPosition = Tuple.Create(currentPosition.Item1, currentPosition.Item2 - 1);
                        break;
                    case Direction.Right:
                        currentPosition = Tuple.Create(currentPosition.Item1, currentPosition.Item2 + 1);
                        break;
                }

                // Handle Shift key down
                if (element.ShiftDown && !shiftIsDown)
                {
                    shiftIsDown = true;
                }

                // Select character if needed
                if (element.Select)
                {
                    char selectedChar = keyboard.Grid[currentPosition.Item1, currentPosition.Item2];
                    if (shiftIsDown)
                    {
                        // Convert to shifted character if Shift is down
                        // (assuming your keyboard grid handles shifted characters)
                        // You might need a separate method to get the shifted character
                        // based on your keyboard grid implementation.
                        // For now, I'll just assume you have a way to do this.
                        selectedChar = selectedChar.ConvertToShift();
                    }
                    outputString.Append(selectedChar);

                    // Handle Shift key up (if needed)
                    // Only release Shift if the next character is not shifted
                    if (shiftIsDown && !element.ShiftDown) // Assuming ShiftDown in the element indicates whether the NEXT character needs Shift
                    {
                        shiftIsDown = false;
                    }
                }
            }
        }

        return outputString.ToString();
    }
    // Convert PathElement list to a string representation
    public static string PathToString(List<PathElement> path)
    {
        StringBuilder sb = new StringBuilder();
        foreach (PathElement element in path)
        {
            sb.Append(PathToString(element));
        }
        return sb.ToString();
    }
    public static string PathToString(PathElement element)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append((int)element.Direction); // Encode direction as integer
        sb.Append(element.Select ? 'D' : 'd'); // Encode Select as 1 or 0
        sb.Append(element.ShiftDown ? 'S' : 's'); // Encode ShiftDown as 1 or 0
        return sb.ToString();
    }

    // Convert string representation back to PathElement list
    public static List<PathElement> StringToPath(string pathString)
    {
        List<PathElement> path = new List<PathElement>();
        for (int i = 0; i < pathString.Length; i += 3)
        {
            Direction direction = (Direction)int.Parse(pathString[i].ToString());
            bool select = pathString[i + 1] == 'D';
            bool shiftDown = pathString[i + 2] == 'S';
            path.Add(new PathElement { Direction = direction, Steps = 1, Select = select, ShiftDown = shiftDown });
        }
        return path;
    }

}