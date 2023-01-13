#pragma warning disable CS8618
namespace ImageService.Context;

/// <summary>
/// Сеты для карт
/// </summary>
public class Set : BaseModel
{
    /// <summary>
    /// Картинка с пиктограммой сета
    /// </summary>
    public string SetImg { get; set; }
}