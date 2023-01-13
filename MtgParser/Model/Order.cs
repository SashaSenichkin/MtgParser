#pragma warning disable CS8618
namespace MtgParser.Model;

/// <summary>
/// 
/// </summary>
public class Order : BaseModel
{
    /// <summary>
    /// 
    /// </summary>
    public User User { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public DateTime CreatedDate { get; set; } 
    
    /// <summary>
    /// 
    /// </summary>
    public DateTime ModifyDate { get; set; } 
    
    /// <summary>
    /// 
    /// </summary>
    public string? TrackNumber{ get; set; } 
    
    /// <summary>
    /// 
    /// </summary>
    public string? Comment { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string Status{ get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string Products { get; set; }
}