using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FakeXieCheng.API.Dtos;
using FakeXieCheng.API.Helper;
using FakeXieCheng.API.ResourceParameters;
using FakeXieCheng.API.Services;
using FakeXieCheng.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace FakeXieCheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;

        public TouristRoutesController(
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            IPropertyMappingService propertyMappingService
        )
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _propertyMappingService = propertyMappingService;
        }

        // Url生成器：生成上/下一页 路径信息
        private string GenerateTouristRouteResourceUrl(
            TouristRouteParameters parameters,
            PaginationResourceParameters pageParameters,
            ResourceUriType type
        )
        {
            return type switch
            {
                ResourceUriType.PreviousPage => _urlHelper.Link("GetTouristRoutes", new
                {
                    fields = parameters.Fields,
                    orderBy = parameters.OrderBy,
                    keyword = parameters.Keyword,
                    rating = parameters.Rating,
                    pageNumber = pageParameters.PageNumber - 1,
                    pageSize = pageParameters.PageSize
                }),
                ResourceUriType.NextPage => _urlHelper.Link("GetTouristRoutes", new
                {
                    fields = parameters.Fields,
                    orderBy = parameters.OrderBy,
                    keyword = parameters.Keyword,
                    rating = parameters.Rating,
                    pageNumber = pageParameters.PageNumber + 1,
                    pageSize = pageParameters.PageSize
                }),
                // 默认页
                _ => _urlHelper.Link("GetTouristRoutes", new
                {
                    fields = parameters.Fields,
                    orderBy = parameters.OrderBy,
                    keyword = parameters.Keyword,
                    rating = parameters.Rating,
                    pageNumber = pageParameters.PageNumber,
                    pageSize = pageParameters.PageSize
                })
            };
        }

        [HttpGet(Name = "GetTouristRoutes")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRoutes([FromQuery] TouristRouteParameters parameters,
            [FromQuery] PaginationResourceParameters pageParameters)
        {
            if (!_propertyMappingService.IsMappingExists<TouristRouteDto, TouristRoute>(parameters.OrderBy))
                return BadRequest("请输入正确的排序参数");
            if (!_propertyMappingService.IsPropertiesExists<TouristRoute>(parameters.Fields))
                return BadRequest("请输入正确的塑形参数");
            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesAsync(
                parameters.Keyword,
                parameters.RatingOperator,
                parameters.RatingValue,
                pageParameters.PageSize,
                pageParameters.PageNumber,
                parameters.OrderBy
            );
            if (touristRoutesFromRepo?.Count <= 0) return NotFound("没有旅游路线");
            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
            var previousPageLink = touristRoutesFromRepo.HasPrevious
                ? GenerateTouristRouteResourceUrl(
                    parameters, pageParameters, ResourceUriType.PreviousPage)
                : null;
            var nextPageLink = touristRoutesFromRepo.HasNext
                ? GenerateTouristRouteResourceUrl(
                    parameters, pageParameters, ResourceUriType.NextPage
                )
                : null;

            // x-pagination
            var paginationMetadata = new
            {
                previousPageLink,
                nextPageLink,
                totalCount = touristRoutesFromRepo.TotalCount,
                pageSize = touristRoutesFromRepo.PageSize,
                currentPage = touristRoutesFromRepo.CurrentPage,
                totalPages = touristRoutesFromRepo.TotalPages
            };
            Response.Headers.Add("x-pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            return Ok(touristRoutesDto.ShapeData(parameters.Fields));
        }

        private IEnumerable<LinkDto> CreateLinkForTouristRoute(
            Guid touristRouteId,
            string fields
        )
        {
            var links = new List<LinkDto>();
            links.Add(new LinkDto(
                Url.Link("GetTouristRouteById", new {touristRouteId, fields}),
                "self",
                "GET"
            ));
            // 更新
            links.Add(new LinkDto(
                Url.Link("UpdateTouristRoute", new {touristRouteId}),
                "update",
                "PUT"
            ));
            // 局部更新
            links.Add(new LinkDto(
                Url.Link("PartiallyUpdateTouristRoute", new {touristRouteId}),
                "partially_update",
                "PATCH"
            ));
            // 删除
            links.Add(new LinkDto(
                Url.Link("DeleteTouristRoute", new {touristRouteId}),
                "delete",
                "DELETE"
            ));
            // 获取旅游路线图片
            links.Add(new LinkDto(
                Url.Link("GetPictureListForTouristRoute", new {touristRouteId}),
                "get_pictures",
                "GET"
            ));
            // 添加新图片
            links.Add(new LinkDto(
                Url.Link("CreateTouristPicture", new {touristRouteId}),
                "create_picture",
                "POST"
            ));
            return links;
        }

        // api/touristRoutes/{touristRouteId}
        [HttpGet("{touristRouteId:Guid}", Name = "GetTouristRouteById")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRouteById(Guid touristRouteId, string fields)
        {
            if (!_propertyMappingService.IsPropertiesExists<TouristRoute>(fields))
                return BadRequest("请输入正确的塑形参数");
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            if (touristRouteFromRepo == null) return NotFound($"旅游路线 {touristRouteId} 不存在");
            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
            // return Ok(touristRouteDto.ShapeData(fields));
            var linkDtos = CreateLinkForTouristRoute(touristRouteId, fields);
            var result = touristRouteDto.ShapeData(fields)
                as IDictionary<string,object>;
            result.Add("links",linkDtos);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            _touristRouteRepository.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);
            return CreatedAtRoute("GetTouristRouteById", new {touristRouteId = touristRouteToReturn.Id},
                touristRouteToReturn);
        }

        [HttpPut("{touristRouteId}", Name = "UpdateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTouristRoute([FromRoute] Guid touristRouteId,
            [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)) return NotFound("旅游路线找不到");
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            // 1. 映射 dto
            // 2. 更新 dto
            // 3. 映射 model
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }

        [HttpPatch("{touristRouteId}", Name = "PartiallyUpdateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PartiallyUpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)) return NotFound("旅游路线找不到");
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            var touristRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
            // JsonPatch 无法进行数据验证，因为数据验证是 JsonPatchDocument 类型
            // 之前的TouristRouteForUpdateDto验证失效
            // 下面利用 ModelState 和 TryValidateModel 进行更新前数据验证
            patchDocument.ApplyTo(touristRouteToPatch, ModelState);
            if (!TryValidateModel(touristRouteToPatch)) return ValidationProblem(ModelState);
            _mapper.Map(touristRouteToPatch, touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{touristRouteId}", Name = "DeleteTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTouristRoute([FromRoute] Guid touristRouteId)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)) return NotFound("旅游路线找不到");
            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _touristRouteRepository.DeleteTouristRoute(touristRoute);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }

        [HttpDelete("({touristIds})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteByIds(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] [FromRoute]
            IEnumerable<Guid> touristIds)
        {
            if (touristIds == null) return BadRequest();
            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesByIdListAsync(touristIds);
            _touristRouteRepository.DeleteTouristRoutes(touristRoutesFromRepo);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }
    }
}