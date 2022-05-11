using AutoMapper;
using FakeXieCheng.API.Dtos;
using FakeXieCheng.API.Models;

namespace FakeXieCheng.API.Profiles
{
    public class ShoppingCartProfile:Profile
    {
        public ShoppingCartProfile()
        {
            CreateMap<ShoppingCart, ShoppingCartDto>();
            CreateMap<LineItem, LineItemDto>();
        }
    }
}