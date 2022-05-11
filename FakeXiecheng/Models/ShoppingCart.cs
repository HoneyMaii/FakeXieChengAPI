using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FakeXieCheng.Models;

namespace FakeXieCheng.API.Models
{
    public class ShoppingCart
    {
        [Key] public Guid Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<LineItem> ShoppingCartItems { get; set; }
    }
}