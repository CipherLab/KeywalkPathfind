using Dapper.Contrib.Extensions;

public class FileProcessed
{
    [Key]
    public int FileProcessedId { get; set; }

    public string FileName { get; set; }
    public bool IsProcessed { get; set; }
}