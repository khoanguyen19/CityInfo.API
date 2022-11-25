using AutoMapper;
using CityInfo.Entities;
using CityInfo.Models;
using CityInfo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.Controllers
{
  [ApiController]
  [Route("api/cities")]
  public class CitiesController : ControllerBase
  {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCititesPageSize = 30;
        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterest>>> GetCities(
            string? name, string? querySearch, int pageNumber = 1, int pageSize = 10)
        {
            if(maxCititesPageSize < pageSize)
            {
                pageSize = maxCititesPageSize;
            }
            var (cityEntities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(name, querySearch, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterest>>(cityEntities));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CityDto>> GetCity(int id, bool includePointOfInterest = false)
        {
            var cityToReturn = await _cityInfoRepository.GetCityAsync(id, includePointOfInterest);
            if (cityToReturn == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CityDto>(cityToReturn));
        }
  }
}
