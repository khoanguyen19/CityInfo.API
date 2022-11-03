using CityInfo.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        /*[HttpGet]
        public JsonResult GetCities()
        {
            return new JsonResult(CityDataStore.Current.Cities);
        }*/

        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCityByName([FromQuery] string? name)
        {
            if(name == null)
            {
                return Ok(CityDataStore.Current.Cities);
            }
            return Ok(CityDataStore.Current.Cities.FirstOrDefault(x => x.Name == name));
        }

        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCity(int id)
        {
            var cityToReturn = CityDataStore.Current.Cities.FirstOrDefault(x => x.Id == id);
            if(cityToReturn == null)
            {
                return NotFound();
            }
            return Ok(cityToReturn);
        }
    }
}
