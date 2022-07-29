namespace MtgParser.Model;

public class Cards
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public string Cmc { get; set; }
    public string Type { get; set; }
    public string Text { get; set; }
    public string Power { get; set; }
    public string Toughness { get; set; }
    public string Img { get; set; }
    public int RarityId { get; set; }
    public byte IsRus { get; set; }
}