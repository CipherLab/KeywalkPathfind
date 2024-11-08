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
}