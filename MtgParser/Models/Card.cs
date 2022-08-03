namespace MtgParser.Model;

public class Card : BaseModel
{
    public string Name { get; set; }
    public string Color { get; set; }
    public string Cmc { get; set; }
    public string Type { get; set; }
    public string Text { get; set; }
    public string TextRus { get; set; }
    public string Power { get; set; }
    public string Toughness { get; set; }
    public string Img { get; set; }
    public bool IsRus { get; set; } 
    
    public Rarity Rarity { get; set; }
    
    public ICollection<Keyword> Keywords { get; set; }
}