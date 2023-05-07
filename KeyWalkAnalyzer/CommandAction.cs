using Dapper.Contrib.Extensions;
using Newtonsoft.Json;

public class CommandAction
{
    public string Direction { get; set; }
    public bool Take { get; set; }

    [JsonIgnore]
    public bool Shift { get; set; }

    [Key]
    public long CommandActionId { get; internal set; }
}