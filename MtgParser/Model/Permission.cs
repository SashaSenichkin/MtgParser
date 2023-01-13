#pragma warning disable CS8618
namespace MtgParser.Model;

/// <summary>
/// 
/// </summary>
public class Permission : BaseModel
{
    /// <summary>
    /// 
    /// </summary>
    public User User { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public  string PermissionStr { get; set; }

}