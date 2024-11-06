namespace KeyboardPathAnalysis
{
    public class PathStep
    {
        public string Direction { get; set; }
        public bool IsPress { get; set; }
        public Hand Hand { get; set; }

        public double Cost { get; set; }
        public Dictionary<string, object> Metadata { get; internal set; }

        public PathStep(string direction, bool isPress = false)
        {
            Direction = direction;
            IsPress = isPress;
        }

        public override string ToString()
        {
            return IsPress ? "press" : Direction;
        }
    }
}