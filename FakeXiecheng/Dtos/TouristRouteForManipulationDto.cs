using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FakeXieCheng.API.ValidationAttributes;

namespace FakeXieCheng.API.Dtos
{
  [TouristRouteTitleMustBeDifferentFromDescription]
  public abstract class TouristRouteForManipulationDto
  {
    [Required(ErrorMessage = "Title 不可为空")]
    [StringLength(100)]
    public string Title { get; set; }
    [Required]
    [StringLength(1500)]
    public virtual string Description { get; set; }
    // 计算方式：原价*折扣
    public decimal Price { get; set; }
    public DateTime? DepartureTime { get; set; }
    public string Features { get; set; } // 卖点
    public string Fees { get; set; } // 费用
    public string Notes { get; set; } // 说明
    public DateTime CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
    public double? Rating { get; set; } // 评分
    public string TravelDays { get; set; } // 旅行天数
    public string TripType { get; set; } // 旅游类型
    public string DepartureCity { get; set; } // 旅游城市
    public ICollection<TouristRoutePictureForCreationDto> TouristRoutePictures { get; set; }
      = new List<TouristRoutePictureForCreationDto>();

    // public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    // {
    //   if (Title == Description)
    //   {
    //     yield return new ValidationResult("路线名称必须与路线描述不同",
    //       new[] {"TouristRouteForCreationDto"}
    //       );
    //   }
    // }
  }
}
