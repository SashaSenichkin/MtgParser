namespace MtgParser.Model;

public class Order : BaseModel
{
    public User User { get; set; }
    public DateTime CreatedDate { get; set; } 
    public DateTime ModifyDate { get; set; } 
    public string? TrackNumber{ get; set; } 
    public string? Comment { get; set; }
    public string Status{ get; set; }
    public string Products { get; set; }
}