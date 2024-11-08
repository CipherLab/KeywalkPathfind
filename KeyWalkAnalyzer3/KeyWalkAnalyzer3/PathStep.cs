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

    public override string ToString()
    {
        if (Direction.StartsWith("shift"))
        {
            return $"{Direction} for '{Key}'";
        }
        return (IsPress ? $"Press '{Key}'" : Direction).ToLower();
    }
}
