namespace MtgParser.Model;

public class User : BaseModel
{
    public  string Email { get; set; }
    public  string Password{ get; set; }
    public  string? Name{ get; set; }
    public  string? Surname{ get; set; } 
    public  string? Address { get; set; }
    public  DateTime RegistrationDate{ get; set; }
    public  string? CheckWord{ get; set; }
    public  string Status{ get; set; }
    public  DateTime BanDate{ get; set; }
}