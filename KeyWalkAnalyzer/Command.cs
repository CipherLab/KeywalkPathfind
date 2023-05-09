using Newtonsoft.Json;

public class Command
{
    public string Direction { get; set; }
    public bool Take { get; set; }

    public bool IsShift { get; set; }

    public Command(string direction, bool take, bool isShift)
    {
        Direction = direction;
        Take = take;
        IsShift = isShift;
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