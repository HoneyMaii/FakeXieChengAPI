using System.ComponentModel.DataAnnotations;

namespace FakeXieCheng.API.Dtos
{
  public class TouristRouteForUpdateDto:TouristRouteForManipulationDto
  {
    [Required(ErrorMessage = "更新必备")]
    [StringLength(1500)]
    public override string Description { get; set; }
  }
}
