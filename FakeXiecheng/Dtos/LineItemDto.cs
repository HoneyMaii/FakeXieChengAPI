using System;
using System.ComponentModel.DataAnnotations.Schema;
using FakeXieCheng.Models;

namespace FakeXieCheng.API.Dtos
{
    public class LineItemDto
    {
        public int Id { get; set; }
        public Guid TouristRouteId { get; set; }
        public TouristRoute TouristRoute { get; set; }
        public Guid? ShoppingCartId { get; set; }
        // public Guid? OrderId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal OriginalPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPresent { get; set; }
    }
}