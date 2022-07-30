using System.ComponentModel.DataAnnotations.Schema;

namespace MtgParser.Model;

[Table("Cards_Sets")]
public class CardSet: BaseModel
{
    public Card Card { get; set; }
    
    public Set Set { get; set; }
    
    public ICollection<Price> Prices { get; set; }
    
    public int Quantity { get; set; }
    public byte IsFoil { get; set; }
}