using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FakeXieCheng.API.Dtos;
using FakeXieCheng.API.Helper;
using FakeXieCheng.API.ResourceParameters;
using FakeXieCheng.Models;
using Microsoft.AspNetCore.Mvc;
using FakeXieCheng.Services;
using Microsoft.AspNetCore.JsonPatch;

namespace FakeXieCheng.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TouristRoutesController : ControllerBase
  {
    private ITouristRouteRepository _touristRouteRepository;
    private readonly IMapper _mapper;
    public TouristRoutesController(ITouristRouteRepository touristRouteRepository,IMapper mapper)
    {
      _touristRouteRepository = touristRouteRepository;
      _mapper = mapper;
    }

    [HttpGet]
    [HttpHead]
    public IActionResult GetTouristRoutes([FromQuery] TouristRouteParameters parameters)
    {
      var touristRoutesFromRepo=_touristRouteRepository.GetTouristRoutes(parameters.Keyword,parameters.RatingOperator,parameters.RatingValue);
      if (touristRoutesFromRepo?.Count() <= 0) return NotFound("没有旅游路线");
      var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
      return Ok(touristRoutesDto);
    }

    // api/touristRoutes/{touristRouteId}
    [HttpGet("{touristRouteId:Guid}",Name = "GetTouristRouteById")]
    [HttpHead]
    public IActionResult GetTouristRouteById(Guid touristRouteId)
    {
      var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
      if (touristRouteFromRepo == null) return NotFound($"旅游路线 {touristRouteId} 不存在");
      var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
      return Ok(touristRouteDto);
    }

    [HttpPost]
    public IActionResult CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
    {
      var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
      _touristRouteRepository.AddTouristRoute(touristRouteModel);
      _touristRouteRepository.Save();
      var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);
      return CreatedAtRoute("GetTouristRouteById", new {touristRouteId = touristRouteToReturn.Id},touristRouteToReturn);
    }

    [HttpPut("{touristRouteId}")]
    public IActionResult UpdateTouristRoute([FromRoute]Guid touristRouteId,[FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto)
    {
      if (!_touristRouteRepository.TouristRouteExists(touristRouteId)) return NotFound("旅游路线找不到");
      var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
      // 1. 映射 dto
      // 2. 更新 dto
      // 3. 映射 model
      _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);
      _touristRouteRepository.Save();
      return NoContent();
    }

    [HttpPatch("{touristRouteId}")]
    public IActionResult PartiallyUpdateTouristRoute(
      [FromRoute]Guid touristRouteId,
      [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument)
    {
      if (!_touristRouteRepository.TouristRouteExists(touristRouteId)) return NotFound("旅游路线找不到");
      var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
      var touristRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
      // JsonPatch 无法进行数据验证，因为数据验证是 JsonPatchDocument 类型
      // 之前的TouristRouteForUpdateDto验证失效
      // 下面利用 ModelState 和 TryValidateModel 进行更新前数据验证
      patchDocument.ApplyTo(touristRouteToPatch,ModelState);
      if (!TryValidateModel(touristRouteToPatch)) return ValidationProblem(ModelState);
      _mapper.Map(touristRouteToPatch, touristRouteFromRepo);
      _touristRouteRepository.Save();
      return NoContent();
    }

    [HttpDelete("{touristRouteId}")]
    public IActionResult DeleteTouristRoute([FromRoute] Guid touristRouteId)
    {
      if (!_touristRouteRepository.TouristRouteExists(touristRouteId)) return NotFound("旅游路线找不到");
      var touristRoute = _touristRouteRepository.GetTouristRoute(touristRouteId);
      _touristRouteRepository.DeleteTouristRoute(touristRoute);
      _touristRouteRepository.Save();
      return NoContent();
    }

    [HttpDelete("({touristIds})")]
    public IActionResult DeleteByIds([ModelBinder(BinderType = typeof(ArrayModelBinder))][FromRoute]IEnumerable<Guid> touristIds)
    {
      if (touristIds == null) return BadRequest();
      var touristRoutesFromRepo = _touristRouteRepository.GetTouristRoutesByIdList(touristIds);
      _touristRouteRepository.DeleteTouristRoutes(touristRoutesFromRepo);
      _touristRouteRepository.Save();
      return NoContent();
    }
  }
}
