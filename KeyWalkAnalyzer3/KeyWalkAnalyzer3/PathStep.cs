namespace KeyboardPathAnalysis
{
    public class PathStep
    {
        public char Key { get; set; } // Added Key property
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
            return (IsPress ? $"Press '{Key}'" : Direction).ToLower();
        }
    }
}