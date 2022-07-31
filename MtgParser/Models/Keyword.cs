namespace MtgParser.Model;

public class Keyword : BaseModel
{
    public string Name { get; set; }
    public string RusName { get; set; }
    public ICollection<Card> Cards { get; set; }
}