using System.ComponentModel.DataAnnotations;
using FakeXieCheng.API.Dtos;

namespace FakeXieCheng.API.ValidationAttributes
{
  public class TouristRouteTitleMustBeDifferentFromDescriptionAttribute:ValidationAttribute
  {
    /// <summary>
    /// 重写 IsValid
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext">访问的是类级别的数据</param>
    /// <returns></returns>
    protected override ValidationResult IsValid(
      object value,
      ValidationContext validationContext)
    {
      var touristRouteDto = (TouristRouteForManipulationDto)validationContext.ObjectInstance;
      if (touristRouteDto.Title == touristRouteDto.Description)
      {
        return new ValidationResult("路线名称必须与路线描述不同",
          new[] { "TouristRouteForCreationDto" }
        );
      }
      return ValidationResult.Success;
    }
  }
}
