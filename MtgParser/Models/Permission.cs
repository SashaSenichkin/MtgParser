namespace MtgParser.Model;

public class Permission : BaseModel
{
    public User User { get; set; }
    public  string PermissionStr { get; set; }
    public DateTime ModifyDate { get; set; } 
}