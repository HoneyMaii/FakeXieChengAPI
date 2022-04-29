using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FakeXieCheng.API.Dtos;
using FakeXieCheng.Models;
using FakeXieCheng.Services;
using Microsoft.AspNetCore.Mvc;

namespace FakeXieCheng.API.Controllers
{
  [Route("api/touristRoutes/{touristRouteId}/pictures")]
  [ApiController]
  public class TouristRoutePicturesController : ControllerBase
  {
    private ITouristRouteRepository _touristRouteRepository;
    private IMapper _mapper;

    public TouristRoutePicturesController(ITouristRouteRepository touristRouteRepository, IMapper mapper)
    {
      _touristRouteRepository = touristRouteRepository;
      _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetPictureListForTouristRoute(Guid touristRouteId)
    {
      if (!_touristRouteRepository.TouristRouteExists(touristRouteId)) return NotFound("旅游路线不存在");
      var picturesFromRepo = _touristRouteRepository.GetPicturesByTouristRouteId(touristRouteId);
      if (picturesFromRepo?.Count() <= 0) return NotFound("照片不存在");
      var picturesDto = _mapper.Map<IEnumerable<TouristRoutePictureDto>>(picturesFromRepo);
      return Ok(picturesDto);
    }

    [HttpGet("{pictureId}", Name = "GetPicture")]
    public IActionResult GetPicture(Guid touristRouteId, int pictureId)
    {
      if (!_touristRouteRepository.TouristRouteExists(touristRouteId)) return NotFound("旅游路线不存在");
      var pictureFromRepo = _touristRouteRepository.GetPicture(pictureId);
      if (pictureFromRepo == null) return NotFound("图片不存在");
      return Ok(_mapper.Map<TouristRoutePictureDto>(pictureFromRepo));
    }

    [HttpPost]
    public IActionResult CreateTouristPicture([FromRoute] Guid touristRouteId, [FromBody] TouristRoutePictureForCreationDto touristRoutePictureForCreationDto)
    {
      if (!_touristRouteRepository.TouristRouteExists(touristRouteId)) return NotFound("旅游路线不存在");
      var pictureModel = _mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);
      _touristRouteRepository.AddTouristRoutePicture(touristRouteId, pictureModel);
      _touristRouteRepository.Save();
      var pictureToReturn = _mapper.Map<TouristRoutePictureDto>(pictureModel);
      return CreatedAtRoute("GetPicture",
        new
        {
          touristRouteId = pictureModel.TouristRouteId,
          pictureId = pictureModel.Id
        },
        pictureToReturn);
    }

    [HttpDelete("{pictureId}")]
    public IActionResult DeletePicture([FromRoute] Guid touristRouteId, [FromRoute] int pictureId)
    {
      if (!_touristRouteRepository.TouristRouteExists(touristRouteId)) return NotFound("旅游路线不存在");
      var picture=_touristRouteRepository.GetPicture(pictureId);
      if (picture == null) return NotFound("图片不存在");
      _touristRouteRepository.DeleteTouristRoutePicture(picture);
      _touristRouteRepository.Save();
      return NoContent();
    }
  }
}
