using System.ComponentModel.DataAnnotations.Schema;

namespace MtgParser.Model;

[Table("Cards_Names")]
public class CardName : BaseModel
{
    public string NameRus { get; set; }
    public string SetShort { get; set; }
}