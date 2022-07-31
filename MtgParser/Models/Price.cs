namespace MtgParser.Model;

public class Price : BaseModel
{
    public decimal Value { get; set; }
    
    public DateTime CreateDate { get; set; }
    
    public CardSet CardSet { get; set; }
}