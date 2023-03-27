using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo;

public class BondEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string BondName { get; set; }
    public int Price { get; set; }
    public DateTime TimeStamp { get; set; }
}