using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo;

public class BondTrendEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string BondName { get; set; }
    public double OverallChange { get; set; }
    public double LastMinuteChange { get; set; }
    public double LastHourChange { get; set; }
}