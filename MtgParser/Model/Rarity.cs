#pragma warning disable CS8618
namespace MtgParser.Model;

/// <summary>
/// Возможные редкости карты. на данный момент - 5. должно биться с названиями на mtg.ru
/// ЗАВОДЯТСЯ В БАЗУ ВРУЧНУЮ
/// </summary>
public class Rarity : BaseModel
{
    /// <summary>
    /// имя, англ
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// имя, рус
    /// </summary>
    public string RusName { get; set; }
}