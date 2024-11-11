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


    public override string ToString()
    {
        return ToAsciiCharacter(this.Direction, this.IsPress);
    }


}