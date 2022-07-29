namespace MtgParser.Model;

public class Prices
{
    public int Id { get; set; }
    public decimal Value { get; set; }
    public DateTime CreateDate { get; set; }
    public int CardsId { get; set; }
}