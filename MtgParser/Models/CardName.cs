namespace MtgParser.Model;

public class CardName : BaseModel
{
    public string? Name { get; set; }
    public string? NameRus { get; set; }
    public int Quantity { get; set; }
    public bool IsFoil { get; set; }
    public string SetShort { get; set; }
}