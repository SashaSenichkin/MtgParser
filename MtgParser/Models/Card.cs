namespace MtgParser.Model;

/// <summary>
/// Абстрактное описание карты
/// </summary>
public class Card : BaseModel
{
    /// <summary>
    /// Название на англ
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Название на русском
    /// </summary>
    public string? NameRus { get; set; }
    
    /// <summary>
    /// цвет карты
    /// </summary>
    public string Color { get; set; }
    
    /// <summary>
    /// Концентрированная мана стоимость
    /// </summary>
    public string Cmc { get; set; }
    
    /// <summary>
    /// Тип карты, англ
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// Тип карты, рус
    /// </summary>
    public string? TypeRus { get; set; }
    
    /// <summary>
    /// Описание действий карты, англ
    /// </summary>
    public string Text { get; set; }
    
    /// <summary>
    /// Описание действий карты, рус
    /// </summary>
    public string? TextRus { get; set; }
    
    /// <summary>
    /// Сила карты. может быть прочерк, может быть бесконечность
    /// </summary>
    public string Power { get; set; }
    
    /// <summary>
    /// Прочность карты. может быть прочерк
    /// </summary>
    public string Toughness { get; set; }
    
    /// <summary>
    /// ссылка на картинку
    /// </summary>
    public string Img { get; set; }
    
    /// <summary>
    /// поле для фронта. у некоторых карт скрываем русское описание вообще. 
    /// </summary>
    public bool IsRus { get; set; } 
    
    /// <summary>
    /// Массив связанных ключевых слов
    /// </summary>
    public ICollection<Keyword> Keywords { get; set; }
}