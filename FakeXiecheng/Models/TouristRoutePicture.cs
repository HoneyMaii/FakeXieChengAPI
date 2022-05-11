using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXieCheng.Models
{
  public class TouristRoutePicture
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [StringLength(100)]
    public string Url { get; set; }
    [ForeignKey("TouristRouteId")]
    public Guid TouristRouteId { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal OriginalPrice { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DiscountPresent { get; set; }
  }
}
