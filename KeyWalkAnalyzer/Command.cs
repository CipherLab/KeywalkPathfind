using Newtonsoft.Json;

public class Command
{
    public string Direction { get; set; }
    public bool Take { get; set; }

    public bool IsShift { get; set; }
    public int Count { get; set; }
    public Command(string direction, bool take, bool isShift, int count = 1)
    {
        Direction = direction;
        Take = take;
        IsShift = isShift;
        Count = count;
    }

    public override string ToString()
    {
        if (IsShift)
        {
            switch (this.Direction)
            {
                case "↑":
                    return "╧";

                case "↓":
                    return "╤";

                case "←":
                    return "╢";

                case "→":
                    return "╟";

                default:
                    return "◙";
            }
        }
        if (Take)
        {
            switch (this.Direction)
            {
                case "↑":
                    return "▲";

                case "↓":
                    return "▼";

                case "←":
                    return "◄";

                case "→":
                    return "►";

                default:
                    return "◘";
            }
        }

        return this.Direction;
    }
}