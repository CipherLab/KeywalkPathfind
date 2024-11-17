namespace KeyWalkAnalyzer3;

public class Movement
    {
        public int RowDiff { get; set; }
        public int ColDiff { get; set; }
        public bool IsDirect { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Movement other)
            {
                return RowDiff == other.RowDiff &&
                       ColDiff == other.ColDiff &&
                       IsDirect == other.IsDirect;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowDiff, ColDiff, IsDirect);
        }

        public string ToArrow()
        {
            if (RowDiff > 0) return "▼";
            if (RowDiff < 0) return "▲";
            if (ColDiff > 0) return "►";
            if (ColDiff < 0) return "◄";
            return "";
        }
    }
