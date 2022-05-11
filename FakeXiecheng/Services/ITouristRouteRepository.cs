using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeXieCheng.Models;

namespace FakeXieCheng.Services
{
  public interface ITouristRouteRepository
  {
    Task<IEnumerable<TouristRoute>> GetTouristRoutesAsync(string keyword,string ratingOperator,int? ratingValue);
    Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId);
    Task<bool> TouristRouteExistsAsync(Guid touristRouteId);
    Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId);
    Task<TouristRoutePicture> GetPictureAsync(int pictureId);
    Task<IEnumerable<TouristRoute>> GetTouristRoutesByIdListAsync(IEnumerable<Guid> ids);
    void AddTouristRoute(TouristRoute touristRoute);
    void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture);
    void DeleteTouristRoute(TouristRoute touristRoute);
    void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);
    void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture);
    bool Save();
  }
}
