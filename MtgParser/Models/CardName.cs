namespace MtgParser.Model;

/// <summary>
/// Минимально необходимая для парсинга информация. Не писать в эту таблицу из кода. только считываем заведённое вручную 
/// </summary>
public class CardName : BaseModel
{
    /// <summary>
    /// Имя карты, англ
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Имя карты, рус
    /// </summary>
    public string? NameRus { get; set; }
    
    /// <summary>
    /// Количество таких карт из этого сета в коллекции
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Фойловость карт (блестящее напыление)
    /// </summary>
    public bool IsFoil { get; set; }
    
    /// <summary>
    /// Аббревиатура сета
    /// </summary>
    public string SetShort { get; set; }
}