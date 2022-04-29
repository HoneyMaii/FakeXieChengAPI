using AutoMapper;
using FakeXieCheng.API.Dtos;
using FakeXieCheng.Models;

namespace FakeXieCheng.API.Profiles
{
  public class TouristRoutePictureProfile:Profile
  {
    public TouristRoutePictureProfile()
    {
      CreateMap<TouristRoutePicture, TouristRoutePictureDto>();
      CreateMap<TouristRoutePictureForCreationDto, TouristRoutePicture>();
      CreateMap<TouristRoutePicture, TouristRoutePictureForCreationDto>();
    }
  }
}
