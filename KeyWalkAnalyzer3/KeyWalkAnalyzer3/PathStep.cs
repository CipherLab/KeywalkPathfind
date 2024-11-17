namespace KeyWalkAnalyzer3;

public class PathStep
{
    public char Key { get; set; }
    public string Direction { get; set; }
    public bool IsPress { get; set; }
    public Hand Hand { get; set; }
    public double Cost { get; set; }
    public Dictionary<string, object> Metadata { get; internal set; }

    public PathStep(char key, string direction, bool isPress = false)
    {
        Key = key;
        Direction = direction;
        IsPress = isPress;
        Metadata = new Dictionary<string, object>();
    }

    public void IncrementRedundantMoveCount()
    {
        if (!Metadata.ContainsKey("RedundantMoveCount"))
        {
            Metadata["RedundantMoveCount"] = 0;
        }
        Metadata["RedundantMoveCount"] = (int)Metadata["RedundantMoveCount"] + 1;
    }

    public int GetRedundantMoveCount()
    {
        return Metadata.ContainsKey("RedundantMoveCount")
            ? (int)Metadata["RedundantMoveCount"]
            : 0;
    }
    public string ToAsciiCharacter(string direction, bool isPress)
    {
        return (Direction.ToLower(), IsPress) switch
        {
            ("release", false) => "",
            ("release", true) => "",
            ("press", true) => "◘",
            ("same", true) => "◘",
            ("right", true) => "►",
            ("left", true) => "◄",
            ("up", true) => "▲",
            ("down", true) => "▼",
            ("right", false) => "→",
            ("left", false) => "←",
            ("up", false) => "↑",
            ("down", false) => "↓",
            _ => throw new Exception($"Invalid PathStep: {Direction} {IsPress}")
        };
    }

    public string SimplifyPathSteps(List<PathStep> pathSteps)
    {
        if (pathSteps == null || pathSteps.Count == 0)
            return string.Empty;

        var simplified = new System.Text.StringBuilder();
        var currentDirection = pathSteps[0].Direction;
        var currentIsPress = pathSteps[0].IsPress;
        int count = 1;

        for (int i = 1; i < pathSteps.Count; i++)
        {
            var step = pathSteps[i];

            // If direction or press state changes, or we've reached the end
            if (step.Direction != currentDirection || step.IsPress != currentIsPress)
            {
                // Convert the current sequence to an ASCII character
                simplified.Append(ToAsciiCharacter(currentDirection, currentIsPress));

                // Reset for the new sequence
                currentDirection = step.Direction;
                currentIsPress = step.IsPress;
                count = 1;
            }
            else
            {
                count++;
            }
        }

        // Add the last sequence
        simplified.Append(ToAsciiCharacter(currentDirection, currentIsPress));

        return simplified.ToString();
    }

    private static readonly HashSet<char> ValidCharacters = new HashSet<char>
        {
            '◘', '►', '◄', '▲', '▼', '→', '←', '↑', '↓'
        };

    /// <summary>
    /// Finds the smallest repeating pattern in a string containing only directional arrows and special characters.
    /// Valid characters are: ◘ ► ◄ ▲ ▼ → ← ↑ ↓
    /// </summary>
    /// <param name="input">The input string containing only valid directional characters</param>
    /// <returns>The smallest repeating pattern, or the entire string if no pattern exists</returns>
    /// <exception cref="ArgumentException">Thrown when input contains invalid characters</exception>
    public static string FindSmallestRepeatingPattern(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Validate input characters
        foreach (char c in input)
        {
            if (!ValidCharacters.Contains(c))
                throw new ArgumentException($"Invalid character found: {c}. Only directional arrows and special characters are allowed.");
        }

        int len = input.Length;

        // Try all possible pattern lengths from 1 to half the string length
        for (int patternLen = 1; patternLen <= len / 2; patternLen++)
        {
            // Only consider lengths that divide evenly into the string length
            if (len % patternLen != 0)
                continue;

            string pattern = input[..patternLen];
            bool isRepeating = true;

            // Check if this pattern repeats throughout the string
            for (int i = patternLen; i < len; i += patternLen)
            {
                if (!input.Substring(i, patternLen).Equals(pattern))
                {
                    isRepeating = false;
                    break;
                }
            }

            if (isRepeating)
                return pattern;
        }

        // If no repeating pattern is found, return the entire string
        return input;
    }
    public static string GetSmallestRepeatingPattern(string input)
    {
        for (int patternLength = 1; patternLength <= input.Length / 2; patternLength++)
        {
            string pattern = input.Substring(0, patternLength);
            int nextIndex = patternLength;

            while (nextIndex < input.Length)
            {
                if (input.Substring(nextIndex).StartsWith(pattern))
                {
                    nextIndex += patternLength;
                }
                else
                {
                    break;
                }
            }

            if (nextIndex >= input.Length - patternLength + 1)
            {
                return pattern;
            }
        }

        return input; // If no repeating pattern is found, return the original string.
    }



    public override string ToString()
    {
        return ToAsciiCharacter(this.Direction, this.IsPress);
    }


}