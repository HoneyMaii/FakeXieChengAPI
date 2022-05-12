using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FakeXieCheng.API.Dtos;
using FakeXieCheng.API.Helper;
using FakeXieCheng.API.Models;
using FakeXieCheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FakeXieCheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;

        public ShoppingCartController(
            IHttpContextAccessor httpContextAccessor,
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetShoppingCart()
        {
            // 1. 获得当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // 2. 使用 userid 获取购物车
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserId(userId);
            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }

        [HttpPost("items")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> AddShoppingCartItem([FromBody] AddShoppingCartItemDto addShoppingCartItemDto)
        {
            // 1. 获得当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // 2. 使用 userid 获取购物车
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserId(userId);
            // 3. 创建 lineItem
            var touristRoute =
                await _touristRouteRepository.GetTouristRouteAsync(addShoppingCartItemDto.TouristRouteId);
            if (touristRoute == null) return NotFound("旅游路线不存在");
            var lineItem = new LineItem
            {
                TouristRouteId = addShoppingCartItemDto.TouristRouteId,
                ShoppingCartId = shoppingCart.Id,
                OriginalPrice = touristRoute.OriginalPrice,
                DiscountPresent = touristRoute.DiscountPresent
            };
            await _touristRouteRepository.AddShoppingCartItem(lineItem);
            await _touristRouteRepository.SaveAsync();
            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }

        [HttpDelete("items/{itemId:int}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItem([FromRoute] int itemId)
        {
            var lineItem = await _touristRouteRepository.GetShoppingCartItemByItemId(itemId);
            if (lineItem == null) return NotFound();
            _touristRouteRepository.DeleteShoppingCartItem(lineItem);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }

        [HttpDelete("items/({itemIDs})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> RemoveShoppingCartItems(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] [FromRoute]
            IEnumerable<int> itemIDs
        )
        {
            if (itemIDs == null) return BadRequest();
            var lineItems = await _touristRouteRepository.GetShoppingCartItemsByIdListAsync(itemIDs);
            _touristRouteRepository.DeleteSHoppingCartItems(lineItems);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
    }
}