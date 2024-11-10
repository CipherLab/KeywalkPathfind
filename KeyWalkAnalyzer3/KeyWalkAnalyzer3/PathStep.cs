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

    public string ToAsciiCharacter()
    {
        return (Direction, IsPress) switch
        {
            ("same", true) => "◘",
            ("right", true) => "►",
            ("left", true) => "◄",
            ("up", true) => "▲",
            ("down", true) => "▼",
            ("right", false) => "→",
            ("left", false) => "←",
            ("up", false) => "↑",
            ("down", false) => "↓",
            _ => " "
        };
    }

    public static string SimplifyPathSteps(List<PathStep> pathSteps)
    {
        if (pathSteps == null || pathSteps.Count == 0)
            return string.Empty;

        var asciiPath = string.Join("", pathSteps.Select(ps => ps.ToAsciiCharacter()));

        // Simple path simplification logic
        var simplified = new System.Text.StringBuilder();
        char? lastChar = null;
        int count = 0;

        foreach (char c in asciiPath)
        {
            if (lastChar == null)
            {
                lastChar = c;
                count = 1;
            }
            else if (c == lastChar)
            {
                count++;
            }
            else
            {
                simplified.Append(lastChar);
                if (count > 1)
                {
                    simplified.Append(count);
                }
                lastChar = c;
                count = 1;
            }
        }

        // Add the last character
        if (lastChar != null)
        {
            simplified.Append(lastChar);
            if (count > 1)
            {
                simplified.Append(count);
            }
        }

        return simplified.ToString();
    }

    public override string ToString()
    {
        if (Direction.StartsWith("shift"))
        {
            return $"{Direction} for '{Key}'";
        }
        return (IsPress ? $"Press '{Key}'" : Direction).ToLower();
    }
}
