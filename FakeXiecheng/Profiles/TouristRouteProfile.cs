using System;
using AutoMapper;
using FakeXieCheng.API.Dtos;
using FakeXieCheng.Models;

namespace FakeXieCheng.API.Profiles
{
  public class TouristRouteProfile : Profile
  {
    public TouristRouteProfile()
    {
      CreateMap<TouristRoute, TouristRouteDto>()
        .ForMember(dest => dest.Price,
          opt => opt.MapFrom(src => src.OriginalPrice * (decimal)(src.DiscountPresent ?? 0)))
        .ForMember(
          dest => dest.TravelDays,
          opt => opt.MapFrom(src => src.TravelDays.ToString()))
        .ForMember(
          dest => dest.TripType,
          opt => opt.MapFrom(src => src.TripType.ToString()))
        .ForMember(
          dest => dest.DepartureCity,
          opt => opt.MapFrom(src => src.DepartureCity.ToString()));

      CreateMap<TouristRouteForCreationDto, TouristRoute>()
        .ForMember(dest => dest.Id,
          opt => opt.MapFrom(src => new Guid()));

      CreateMap<TouristRouteForUpdateDto, TouristRoute>();

      CreateMap<TouristRoute, TouristRouteForUpdateDto>();
    }
  }
}
