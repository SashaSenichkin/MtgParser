#pragma warning disable CS8618
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
    
    /// <summary>
    /// use it to get word to seek in search engine
    /// </summary>
    public string? SeekName => string.IsNullOrEmpty(Name) ? NameRus : Name;
    
        
    /// <summary>
    /// Is card parsed already
    /// </summary>
    public bool IsParsed { get; set; }

    /// <summary>
    /// Проверка совпадений сущности и карты..
    /// </summary>
    /// <param name="candidate">карта для проверки соответствия текущему CardName</param>
    /// <returns>совпадение или нет</returns>
    public bool IsCardEqual(Card candidate)
    {
        switch (candidate.IsRus)
        {
            case true when string.IsNullOrEmpty(candidate.NameRus):
            case false when string.IsNullOrEmpty(candidate.Name):
                return false;
        }

        if (!string.IsNullOrEmpty(candidate.Name) && !string.IsNullOrEmpty(Name) )
        {
            return candidate.Name.Contains(Name);
        }
        
        if (!string.IsNullOrEmpty(candidate.NameRus) && !string.IsNullOrEmpty(NameRus) )
        {
            return candidate.NameRus.Contains(NameRus);
        }

        return false;
    }
}