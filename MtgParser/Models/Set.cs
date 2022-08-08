namespace MtgParser.Model;

public class Set : BaseModel
{
    public string? RusName { get; set; }
    public string FullName { get; set; }
    public string ShortName { get; set; }
    public string SearchText { get; set; }
    public string SetImg { get; set; }
    
    public override Boolean Equals(Object? obj)
    {
        return ShortName == (obj as Set)?.ShortName;
    }
}