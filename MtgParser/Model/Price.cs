#pragma warning disable CS8618
namespace MtgParser.Model;

/// <summary>
/// Цена на конкректную физическую карту, с днём получения из api
/// </summary>
public class Price : BaseModel
{
    /// <summary>
    /// сумма, $
    /// </summary>
    public decimal Value { get; set; }
    
    /// <summary>
    /// дата создания записи в таблице
    /// </summary>
    public DateTime CreateDate { get; set; }
    
    /// <summary>
    /// привязка к физической карте
    /// </summary>
    public CardSet CardSet { get; set; }
}