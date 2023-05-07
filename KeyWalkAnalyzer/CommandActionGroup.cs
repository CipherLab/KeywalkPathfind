using Dapper.Contrib.Extensions;

public class CommandActionGroup
{
    [Key]
    public long CommandActionGroupId { get; set; }

    public long CommandActionId { get; set; }
    public long CommandActionHeaderId { get; set; }
    public int Seq { get; set; }
}