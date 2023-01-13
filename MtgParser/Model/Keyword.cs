#pragma warning disable CS8618
namespace MtgParser.Model;

/// <summary>
/// Ключевое слово. согласно https://en.wikipedia.org/wiki/List_of_Magic:_The_Gathering_keywords их больше сотни. сейчас тестил только на Evergreen.
/// ЗАВОДЯТСЯ В БАЗУ ВРУЧНУЮ
/// </summary>
public class Keyword : BaseModel
{
    /// <summary>
    /// название, англ
    /// </summary>

    public string Name { get; set; }
    
    /// <summary>
    /// название, рус
    /// </summary>
    public string RusName { get; set; }
    
    /// <summary>
    /// связь с картой - многие ко многим, EF создал промежуточную таблицу
    /// </summary>
    public ICollection<Card> Cards { get; set; }
}