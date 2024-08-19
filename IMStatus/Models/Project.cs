namespace IMStatus.Models;

public class Project
{
    
    public int Id { get; set; }
    public string Title { get; set; }
    public string Status { get; set; }
    public string CreationDate { get; set; }
    
    public static Project FromJson(dynamic item)
    {
        return new Project
        {
            Title = item.metadata.name,
            CreationDate = item.metadata.creationTimestamp,
            Status = item.status.phase
        };
    }
    
}