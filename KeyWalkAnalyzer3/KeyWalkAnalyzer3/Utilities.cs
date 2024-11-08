using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeyWalkAnalyzer3;

public class Utilities
{

    public static bool IsShiftCharacter(char c)
    {
        // Characters that require shift on a standard US keyboard
        return "~!@#$%^&*()_+{}|:\"<>?".Contains(c);
    }
    public static Dictionary<char, EnhancedKeyPosition> InitializeKeyboardLayout()
    {
        var layout = new Dictionary<char, EnhancedKeyPosition>();

        // Define the keyboard layout (QWERTY)
        // Lowercase letters
        layout.Add('q', new EnhancedKeyPosition(0, 0, 'q', Hand.Left, FingerStrength.Pinky, 0.8, false, true));
        layout.Add('w', new EnhancedKeyPosition(0, 1, 'w', Hand.Left, FingerStrength.Ring, 0.7, false, true));
        layout.Add('e', new EnhancedKeyPosition(0, 2, 'e', Hand.Left, FingerStrength.Middle, 0.6, false, true));
        layout.Add('r', new EnhancedKeyPosition(0, 3, 'r', Hand.Left, FingerStrength.Index, 0.5, false, true));
        layout.Add('t', new EnhancedKeyPosition(0, 4, 't', Hand.Left, FingerStrength.Index, 0.4, false, true));
        layout.Add('y', new EnhancedKeyPosition(0, 5, 'y', Hand.Right, FingerStrength.Index, 0.4, false, true));
        layout.Add('u', new EnhancedKeyPosition(0, 6, 'u', Hand.Right, FingerStrength.Middle, 0.5, false, true));
        layout.Add('i', new EnhancedKeyPosition(0, 7, 'i', Hand.Right, FingerStrength.Ring, 0.6, false, true));
        layout.Add('o', new EnhancedKeyPosition(0, 8, 'o', Hand.Right, FingerStrength.Pinky, 0.7, false, true));
        layout.Add('p', new EnhancedKeyPosition(0, 9, 'p', Hand.Right, FingerStrength.Pinky, 0.8, false, true));

        layout.Add('a', new EnhancedKeyPosition(1, 0, 'a', Hand.Left, FingerStrength.Pinky, 0.4, true, false));
        layout.Add('s', new EnhancedKeyPosition(1, 1, 's', Hand.Left, FingerStrength.Ring, 0.3, true, false));
        layout.Add('d', new EnhancedKeyPosition(1, 2, 'd', Hand.Left, FingerStrength.Middle, 0.2, true, false));
        layout.Add('f', new EnhancedKeyPosition(1, 3, 'f', Hand.Left, FingerStrength.Index, 0.1, true, false));
        layout.Add('g', new EnhancedKeyPosition(1, 4, 'g', Hand.Right, FingerStrength.Index, 0.1, true, false));
        layout.Add('h', new EnhancedKeyPosition(1, 5, 'h', Hand.Right, FingerStrength.Middle, 0.2, true, false));
        layout.Add('j', new EnhancedKeyPosition(1, 6, 'j', Hand.Right, FingerStrength.Ring, 0.3, true, false));
        layout.Add('k', new EnhancedKeyPosition(1, 7, 'k', Hand.Right, FingerStrength.Pinky, 0.4, true, false));
        layout.Add('l', new EnhancedKeyPosition(1, 8, 'l', Hand.Right, FingerStrength.Pinky, 0.5, true, false));

        layout.Add('z', new EnhancedKeyPosition(2, 0, 'z', Hand.Left, FingerStrength.Pinky, 0.9, false, false));
        layout.Add('x', new EnhancedKeyPosition(2, 1, 'x', Hand.Left, FingerStrength.Ring, 0.8, false, false));
        layout.Add('c', new EnhancedKeyPosition(2, 2, 'c', Hand.Left, FingerStrength.Middle, 0.7, false, false));
        layout.Add('v', new EnhancedKeyPosition(2, 3, 'v', Hand.Left, FingerStrength.Index, 0.6, false, false));
        layout.Add('b', new EnhancedKeyPosition(2, 4, 'b', Hand.Right, FingerStrength.Index, 0.6, false, false));
        layout.Add('n', new EnhancedKeyPosition(2, 5, 'n', Hand.Right, FingerStrength.Middle, 0.7, false, false));
        layout.Add('m', new EnhancedKeyPosition(2, 6, 'm', Hand.Right, FingerStrength.Ring, 0.8, false, false));

        // Numbers
        layout.Add('1', new EnhancedKeyPosition(0, 0, '1', Hand.Left, FingerStrength.Pinky, 0.9, false, true));
        layout.Add('2', new EnhancedKeyPosition(0, 1, '2', Hand.Left, FingerStrength.Ring, 0.8, false, true));
        layout.Add('3', new EnhancedKeyPosition(0, 2, '3', Hand.Left, FingerStrength.Middle, 0.7, false, true));
        layout.Add('4', new EnhancedKeyPosition(0, 3, '4', Hand.Left, FingerStrength.Index, 0.6, false, true));
        layout.Add('5', new EnhancedKeyPosition(0, 4, '5', Hand.Right, FingerStrength.Index, 0.6, false, true));
        layout.Add('6', new EnhancedKeyPosition(0, 5, '6', Hand.Right, FingerStrength.Middle, 0.7, false, true));
        layout.Add('7', new EnhancedKeyPosition(0, 6, '7', Hand.Right, FingerStrength.Ring, 0.8, false, true));
        layout.Add('8', new EnhancedKeyPosition(0, 7, '8', Hand.Right, FingerStrength.Pinky, 0.9, false, true));
        layout.Add('9', new EnhancedKeyPosition(0, 8, '9', Hand.Right, FingerStrength.Pinky, 1.0, false, true));
        layout.Add('0', new EnhancedKeyPosition(0, 9, '0', Hand.Right, FingerStrength.Pinky, 1.0, false, true));

        // Symbols (with shift requirements)
        layout.Add('`', new EnhancedKeyPosition(0, 0, '`', Hand.Left, FingerStrength.Pinky, 1.0, false, false));
        layout.Add('-', new EnhancedKeyPosition(0, 12, '-', Hand.Right, FingerStrength.Pinky, 0.9, false, false));
        layout.Add('=', new EnhancedKeyPosition(0, 13, '=', Hand.Right, FingerStrength.Pinky, 0.9, false, true));
        layout.Add('[', new EnhancedKeyPosition(1, 11, '[', Hand.Right, FingerStrength.Pinky, 0.8, false, false));
        layout.Add(']', new EnhancedKeyPosition(1, 12, ']', Hand.Right, FingerStrength.Pinky, 0.8, false, false));
        layout.Add('\\', new EnhancedKeyPosition(1, 13, '\\', Hand.Right, FingerStrength.Pinky, 0.9, false, false));
        layout.Add(';', new EnhancedKeyPosition(1, 9, ';', Hand.Right, FingerStrength.Pinky, 0.7, false, false));
        layout.Add('\'', new EnhancedKeyPosition(1, 10, '\'', Hand.Right, FingerStrength.Pinky, 0.8, false, false));
        layout.Add(',', new EnhancedKeyPosition(2, 7, ',', Hand.Right, FingerStrength.Pinky, 0.9, false, false));
        layout.Add('.', new EnhancedKeyPosition(2, 8, '.', Hand.Right, FingerStrength.Pinky, 0.9, false, false));
        layout.Add('/', new EnhancedKeyPosition(2, 9, '/', Hand.Right, FingerStrength.Pinky, 1.0, false, false));

        // Shifted symbols
        layout.Add('~', new EnhancedKeyPosition(0, 0, '~', Hand.Left, FingerStrength.Pinky, 1.0, true, true));
        layout.Add('!', new EnhancedKeyPosition(0, 1, '!', Hand.Left, FingerStrength.Ring, 0.9, true, true));
        layout.Add('@', new EnhancedKeyPosition(0, 2, '@', Hand.Left, FingerStrength.Middle, 0.8, true, true));
        layout.Add('#', new EnhancedKeyPosition(0, 3, '#', Hand.Left, FingerStrength.Index, 0.7, true, true));
        layout.Add('$', new EnhancedKeyPosition(0, 4, '$', Hand.Right, FingerStrength.Index, 0.7, true, true));
        layout.Add('%', new EnhancedKeyPosition(0, 5, '%', Hand.Right, FingerStrength.Middle, 0.8, true, true));
        layout.Add('^', new EnhancedKeyPosition(0, 6, '^', Hand.Right, FingerStrength.Ring, 0.9, true, true));
        layout.Add('&', new EnhancedKeyPosition(0, 7, '&', Hand.Right, FingerStrength.Pinky, 1.0, true, true));
        layout.Add('*', new EnhancedKeyPosition(0, 8, '*', Hand.Right, FingerStrength.Pinky, 1.0, true, true));
        layout.Add('(', new EnhancedKeyPosition(0, 9, '(', Hand.Right, FingerStrength.Pinky, 1.0, true, true));
        layout.Add(')', new EnhancedKeyPosition(0, 10, ')', Hand.Right, FingerStrength.Pinky, 1.0, true, true));
        layout.Add('_', new EnhancedKeyPosition(0, 12, '_', Hand.Right, FingerStrength.Pinky, 1.0, true, true));
        layout.Add('+', new EnhancedKeyPosition(0, 13, '+', Hand.Right, FingerStrength.Pinky, 1.0, true, true));
        layout.Add('{', new EnhancedKeyPosition(1, 11, '{', Hand.Right, FingerStrength.Pinky, 0.9, true, true));
        layout.Add('}', new EnhancedKeyPosition(1, 12, '}', Hand.Right, FingerStrength.Pinky, 0.9, true, true));
        layout.Add('|', new EnhancedKeyPosition(1, 13, '|', Hand.Right, FingerStrength.Pinky, 1.0, true, true));
        layout.Add(':', new EnhancedKeyPosition(1, 9, ':', Hand.Right, FingerStrength.Pinky, 0.8, true, true));
        layout.Add('"', new EnhancedKeyPosition(1, 10, '"', Hand.Right, FingerStrength.Pinky, 0.9, true, true));
        layout.Add('<', new EnhancedKeyPosition(2, 7, '<', Hand.Right, FingerStrength.Pinky, 1.0, true, true));
        layout.Add('>', new EnhancedKeyPosition(2, 8, '>', Hand.Right, FingerStrength.Pinky, 1.0, true, true));
        layout.Add('?', new EnhancedKeyPosition(2, 9, '?', Hand.Right, FingerStrength.Pinky, 1.0, true, true));

        return layout;
    }
}