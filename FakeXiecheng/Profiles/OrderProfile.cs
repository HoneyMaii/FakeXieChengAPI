using AutoMapper;
using FakeXieCheng.API.Dtos;
using FakeXieCheng.API.Models;

namespace FakeXieCheng.API.Profiles
{
    public class OrderProfile: Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest=>dest.State,
                    opt=>
                        opt.MapFrom(src=>src.State.ToString()));
            CreateMap<OrderDto, Order>();
        }
    }
}