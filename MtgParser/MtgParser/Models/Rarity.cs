using System.ComponentModel.DataAnnotations.Schema;

namespace MtgParser.Model;

[Table("Rarities")]
public class Rarity : BaseModel
{
    public string Name { get; set; }
    public string RusName { get; set; }
}