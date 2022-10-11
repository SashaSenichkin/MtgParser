namespace MtgParser.Model;

/// <summary>
/// Сеты для карт
/// </summary>
public class Set : BaseModel
{
    /// <summary>
    /// Название сета, рус
    /// </summary>
    public string? RusName { get; set; }
    
    /// <summary>
    /// Название сета, англ
    /// </summary>
    public string FullName { get; set; }
    
    /// <summary>
    /// Аббревиатура сета
    /// </summary>
    public string ShortName { get; set; }
    
    /// <summary>
    /// Для поиска цен, пробелы заменяются на плюсы.. возможно, где-то будут и другие правила замены.. оставлю это для ручной правки
    /// </summary>
    public string SearchText { get; set; }
    
    /// <summary>
    /// Картинка с пиктограммой сета
    /// </summary>
    public string SetImg { get; set; }
}