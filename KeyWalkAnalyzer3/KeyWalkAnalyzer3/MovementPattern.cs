namespace KeyWalkAnalyzer3;

public class MovementPattern
    {
        public char StartChar { get; }
        public List<Movement> Movements { get; }

        public MovementPattern(char startChar, List<Movement> movements)
        {
            StartChar = startChar;
            Movements = movements;
        }

        public string ToArrowString()
        {
            return string.Join("", Movements.Select(m => m.ToArrow()));
        }
    }
