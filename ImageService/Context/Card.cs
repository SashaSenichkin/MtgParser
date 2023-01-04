namespace ImageService.Context;

/// <summary>
/// Абстрактное описание карты
/// </summary>
public class Card : BaseModel
{
    /// <summary>
    /// ссылка на картинку
    /// </summary>
    public string Img { get; set; }
}