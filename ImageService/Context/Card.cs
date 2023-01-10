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
    
    /// <summary>
    /// поле для фронта. у некоторых карт скрываем русское описание вообще. 
    /// </summary>
    public bool IsRus { get; set; } 
}