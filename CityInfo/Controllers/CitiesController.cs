using CityInfo.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly CityDataStore _cityDataStore;

        /*[HttpGet]
public JsonResult GetCities()
{
return new JsonResult(_cityDataStore.Cities);
}*/

        public CitiesController(CityDataStore cityDataStore)
        {
            _cityDataStore = cityDataStore;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCityByName([FromQuery] string? name)
        {
            if(name == null)
            {
                return Ok(_cityDataStore.Cities);
            }
            return Ok(_cityDataStore.Cities.FirstOrDefault(x => x.Name == name));
        }

        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCity(int id)
        {
            var cityToReturn = _cityDataStore.Cities.FirstOrDefault(x => x.Id == id);
            if(cityToReturn == null)
            {
                return NotFound();
            }
            return Ok(cityToReturn);
        }
    }
}
