namespace MtgParser.Model;

public class CardSet: BaseModel
{
    public Card Card { get; set; }
    
    public Set Set { get; set; }
    
    public ICollection<Price> Prices { get; set; }
    
    public int Quantity { get; set; }
    public byte IsFoil { get; set; }
}