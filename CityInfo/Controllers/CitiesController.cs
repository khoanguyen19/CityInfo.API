using AutoMapper;
using CityInfo.Entities;
using CityInfo.Models;
using CityInfo.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.Controllers
{
  [ApiController]
  [Route("api/cities")]
  public class CitiesController : ControllerBase
  {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
    {
            _cityInfoRepository = cityInfoRepository;
            _mapper = mapper;
        }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterest>>> GetCityies()
    {
        var cityEntities = await _cityInfoRepository.GetCitiesAsync();
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
