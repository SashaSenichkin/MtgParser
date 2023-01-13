#pragma warning disable CS8618
namespace MtgParser.Model;

/// <summary>
/// 
/// </summary>
public class User : BaseModel
{
    /// <summary>
    /// 
    /// </summary>
    public  string Email { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public  string Password{ get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public  string? Name{ get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public  string? Surname{ get; set; } 
    
    /// <summary>
    /// 
    /// </summary>
    public  string? Address { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public  DateTime RegistrationDate{ get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public  string? CheckWord{ get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public  string Status{ get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public  DateTime BanDate{ get; set; }
}