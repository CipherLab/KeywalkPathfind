using Newtonsoft.Json;

public class Command
{
    public string Direction { get; set; }
    public bool Take { get; set; }

    [JsonIgnore]
    public bool IsShift { get; set; }

    public Command(string direction, bool take, bool isShift)
    {
        Direction = direction;
        Take = take;
        IsShift = isShift;
    }

    public override string ToString()
    {
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
        else
        {
            return this.Direction;
        }
    }
}