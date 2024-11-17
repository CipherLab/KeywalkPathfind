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