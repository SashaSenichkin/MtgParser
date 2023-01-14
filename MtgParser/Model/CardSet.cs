#pragma warning disable CS8618
namespace MtgParser.Model;

/// <summary>
/// Отображение физической карты
/// </summary>
public class CardSet: BaseModel
{
    /// <summary>
    /// Связанное описание карты
    /// </summary>
    public Card Card { get; set; }
    
    /// <summary>
    /// Сет принадлежности карты
    /// </summary>
    public Set Set { get; set; }
    
    /// <summary>
    /// Полученные цены
    /// </summary>
    public ICollection<Price> Prices { get; set; }
    
    /// <summary>
    /// Количество таких карт из этого сета в коллекции
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Фойловость карт (блестящее напыление)
    /// </summary>
    public byte IsFoil { get; set; }
    
    /// <summary>
    /// редкость карты в этом сете
    /// </summary>
    public Rarity Rarity { get; set; }
    
    /// <summary>
    /// введённая вручную цена. на неё фронт смотрит, и игнорит все наши, если есть..
    /// </summary>
    public decimal? ManualPrice { get; set; }
}