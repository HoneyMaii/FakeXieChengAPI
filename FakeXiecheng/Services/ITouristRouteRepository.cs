using System;
using System.Collections.Generic;
using FakeXieCheng.Models;

namespace FakeXieCheng.Services
{
  public interface ITouristRouteRepository
  {
    IEnumerable<TouristRoute> GetTouristRoutes(string keyword,string ratingOperator,int? ratingValue);
    TouristRoute GetTouristRoute(Guid touristRouteId);
    bool TouristRouteExists(Guid touristRouteId);
    IEnumerable<TouristRoutePicture> GetPicturesByTouristRouteId(Guid touristRouteId);
    TouristRoutePicture GetPicture(int pictureId);
    IEnumerable<TouristRoute> GetTouristRoutesByIdList(IEnumerable<Guid> ids);
    void AddTouristRoute(TouristRoute touristRoute);
    void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture);
    void DeleteTouristRoute(TouristRoute touristRoute);
    void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);
    void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture);
    bool Save();
  }
}
