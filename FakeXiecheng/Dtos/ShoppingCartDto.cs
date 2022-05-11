using System;
using System.Collections.Generic;
using FakeXieCheng.API.Models;

namespace FakeXieCheng.API.Dtos
{
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public ICollection<LineItemDto> ShoppingCartItems { get; set; }
    }
}