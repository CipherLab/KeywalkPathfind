using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyWalkAnalyzer2;
public struct PathElement
{
    public Direction Direction;
    public int Steps;
    public bool Select;
    public bool ShiftDown;
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
                return true;
            default:
                return char.IsUpper(c);
        }

    }
    public static char ConvertToNonShift(this char c)

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
    public static char ConvertToShift(this char c)

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