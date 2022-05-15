using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FakeXieCheng.API.Dtos;
using FakeXieCheng.API.ResourceParameters;
using FakeXieCheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FakeXieCheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public OrderController(
            ITouristRouteRepository touristRouteRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        )
        {
            _touristRouteRepository = touristRouteRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetOrders")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrders([FromQuery] PaginationResourceParameters pageParameters)
        {
            // 1. 获取当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // 2. 使用userid 获取订单历史记录
            var orders =
                await _touristRouteRepository.GetOrdersByUserId(userId, pageParameters.PageSize,
                    pageParameters.PageNumber);
            return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
        }

        [HttpGet("{orderId:guid}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
        {
            // 1. 获取当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // 2. 
            var order = await _touristRouteRepository.GetOrderById(orderId);
            return Ok(_mapper.Map<OrderDto>(order));
        }
    }
}