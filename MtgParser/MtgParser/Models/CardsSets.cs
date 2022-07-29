namespace MtgParser.Model;

public class CardsSets
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public int SetId { get; set; }
    public int Quantity { get; set; }
    public byte IsFoil { get; set; }
}