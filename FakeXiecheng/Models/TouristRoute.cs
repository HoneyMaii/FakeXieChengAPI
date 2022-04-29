using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FakeXieCheng.API.Models;

namespace FakeXieCheng.Models
{
  public class TouristRoute
  {
    [Key]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }
    [Required]
    [StringLength(1500)]
    public string Description { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal OriginalPrice { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DiscountPresent { get; set; }
    public DateTime? DepartureTime { get; set; }
    [MaxLength]
    public string Features { get; set; } // 卖点
    [MaxLength]
    public string Fees { get; set; } // 费用
    [MaxLength]
    public string Notes { get; set; } // 说明
    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public double? Rating { get; set; } // 评分
    public TravelDays TravelDays { get; set; } // 旅行天数
    public TripType TripType { get; set; } // 旅游类型
    public DepartureCity DepartureCity { get; set; } // 旅游城市
    
    public ICollection<TouristRoutePicture> TouristRoutePictures { get; set; }
      = new List<TouristRoutePicture>();
  }
}
