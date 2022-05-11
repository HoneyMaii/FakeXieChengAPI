using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeXieCheng.API.Database;
using FakeXieCheng.API.Dtos;
using FakeXieCheng.API.Models;
using FakeXieCheng.API.Services;
using FakeXieCheng.Models;
using Microsoft.EntityFrameworkCore;

namespace FakeXieCheng.Services
{
  public class TouristRouteRepository:ITouristRouteRepository
  {
    private readonly AppDbContext _context;

    public TouristRouteRepository(AppDbContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<TouristRoute>> GetTouristRoutesAsync(string keyword, string ratingOperator, int? ratingValue)
    {
      IQueryable<TouristRoute> result = _context.TouristRoutes.Include(t => t.TouristRoutePictures);
      if (!string.IsNullOrWhiteSpace(keyword))
      {
        keyword = keyword.Trim();
        result=result.Where(t => t.Title.Contains(keyword));
      }

      if (ratingValue > 0)
      {
        // 函数式写法
        result = ratingOperator switch
        {
          "largerThan" => result.Where(t => t.Rating >= ratingValue),
          "lessThan" => result.Where(t => t.Rating <= ratingValue),
          _ => result.Where(t => t.Rating == ratingValue)
        };
      }
      // Eager Load 立即加载 （include vs join ）
      return await result.ToListAsync();
    }

    public async Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId)
    {
      return await _context.TouristRoutes.Where(n => n.Id == touristRouteId).Include(p=>p.TouristRoutePictures).FirstOrDefaultAsync();
    }

    public async Task<bool> TouristRouteExistsAsync(Guid touristRouteId)
    {
      return await _context.TouristRoutes.AnyAsync(n => n.Id == touristRouteId);
    }

    public async Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId)
    {
      return await _context.TouristRoutePictures.Where(p => p.TouristRouteId == touristRouteId).ToListAsync();
    }

    public async Task<TouristRoutePicture> GetPictureAsync(int pictureId)
    {
      return await _context.TouristRoutePictures.FirstOrDefaultAsync(p=>p.Id==pictureId);
    }

    public async Task<IEnumerable<TouristRoute>> GetTouristRoutesByIdListAsync(IEnumerable<Guid> ids)
    {
      return await _context.TouristRoutes.Where(t => ids.Contains(t.Id)).ToListAsync();
    }

    public void AddTouristRoute(TouristRoute touristRoute)
    {
      if (touristRoute == null)
      {
        throw new ArgumentNullException(nameof(touristRoute));
      }
      _context.TouristRoutes.Add(touristRoute);
    }

    public void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture)
    {
      if (touristRouteId == Guid.Empty) throw new ArgumentNullException(nameof(touristRouteId));
      if (touristRoutePicture == null) throw new ArgumentNullException(nameof(touristRouteId));
      touristRoutePicture.TouristRouteId = touristRouteId;
      _context.TouristRoutePictures.Add(touristRoutePicture);
    }

    public void DeleteTouristRoute(TouristRoute touristRoute)
    {
      _context.TouristRoutes.Remove(touristRoute);
    }

    public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
    {
      _context.TouristRoutes.RemoveRange(touristRoutes);
    }

    public void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture)
    {
      _context.TouristRoutePictures.Remove(touristRoutePicture);
    }

    public async Task<ShoppingCart> GetShoppingCartByUserId(string userId)
    {
      return await _context.ShoppingCarts
        .Include(s => s.User)
        .Include(s => s.ShoppingCartItems).ThenInclude(li => li.TouristRoute)
        .Where(s => s.UserId == userId)
        .FirstOrDefaultAsync();
    }

    public async Task CreateShoppingCart(ShoppingCart shoppingCart)
    {
      await _context.ShoppingCarts.AddAsync(shoppingCart);
    }

    public async Task AddShoppingCartItem(LineItem lineItem)
    {
      await _context.LineItems.AddAsync(lineItem);
    }

    public async Task<LineItem> GetShoppingCartItemByItemId(int lineItemId)
    {
      return await _context.LineItems.Where(li => li.Id == lineItemId).FirstOrDefaultAsync();
    }

    public async void DeleteShoppingCartItem(LineItem lineItem)
    {
      _context.LineItems.Remove(lineItem);
    }

    public async Task<IEnumerable<LineItem>> GetShoppingCartItemsByIdListAsync(IEnumerable<int> ids)
    {
      return await _context.LineItems.Where(li => ids.Contains(li.Id)).ToListAsync();
    }

    public void DeleteSHoppingCartItems(IEnumerable<LineItem> lineItems)
    {
      _context.LineItems.RemoveRange(lineItems);
    }

    public async Task<bool> SaveAsync()
    {
      return await _context.SaveChangesAsync() > 0;
    }
  }
}
