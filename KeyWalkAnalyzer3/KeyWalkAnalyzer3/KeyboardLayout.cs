using KeyWalkAnalyzer3;

public class KeyboardLayout
{
    private Dictionary<char, KeyPosition> keyPositions = new Dictionary<char, KeyPosition>();
    private Dictionary<char, char> shiftVariants = new Dictionary<char, char>();
    private static readonly string[] QWERTY_LAYOUT = new[]
    {
        "1234567890-=",
        "qwertyuiop[]",
        "asdfghjkl;'",
        "zxcvbnm,./"
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
                keyPositions[key] = new KeyPosition(row, col, key);
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
}
