using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXieCheng.API.Dtos
{
    public class TouristRouteDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        // 计算方式：原价*折扣
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
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
        public ICollection<TouristRoutePictureDto> TouristRoutePictures { get; set; }
    }
}