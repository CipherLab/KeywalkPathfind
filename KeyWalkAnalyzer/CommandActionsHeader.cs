using Dapper.Contrib.Extensions;

public class CommandActionsHeader
{
    public string CommandHash { get; set; }
    public int ObservedCount { get; set; }

    [Key]
    public long CommandActionsHeaderId { get; internal set; }
}