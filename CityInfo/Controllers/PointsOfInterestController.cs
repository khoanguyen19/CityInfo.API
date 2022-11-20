using AutoMapper;
using CityInfo.Models;
using CityInfo.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.Controllers
{
  [ApiController]
  [Route("api/cities/{cityId}/pointsofinterest")]
  public class PointsOfInterestController : Controller
  {

        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, 
            IMailService mailService, 
            ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {

            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            try
            {
                if (await _cityInfoRepository.CityExistAsync(cityId))
                {
                    var pointsOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

                    return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
                }

                _logger.LogInformation($"City with id {cityId} wasn't found");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterestById")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterestById(int cityId, int pointofinterestid)
        {
            //var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointofinterestid);
            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        [HttpGet("pointofinterestname")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterestByName(int cityId, string pointofinterestname)
        {
            if (await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _cityInfoRepository.GetPointOfInterestByName(cityId, pointofinterestname);
            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        //[HttpPost]
        //public ActionResult<PointOfInterestDto> CreatePointOfInterest(
        //    int cityId, PointOfInterestForCreationDto pointOfInterest)
        //{
        //    var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //    return NotFound();
        //    }

        //    var maxPointOfInterestId = _cityDataStore.Cities
        //        .SelectMany(c => c.PointsOfInterest)
        //        .Max(p => p.Id);

        //    var finalPointOfInterest = new PointOfInterestDto()
        //    {
        //    Id = ++maxPointOfInterestId,
        //    Name = pointOfInterest.Name,
        //    Description = pointOfInterest.Description,
        //    };

        //    city.PointsOfInterest.Add(finalPointOfInterest);

        //    return CreatedAtRoute("GetPointOfInterestById",
        //        new
        //        {
        //        cityId = cityId,
        //        pointofinterestid = finalPointOfInterest.Id,
        //        },
        //        finalPointOfInterest);
        //}

        //[HttpPut("{pointofinterestid}")]
        //public ActionResult UpdatePointOfInterest(int cityId, int pointofinterestid, PointOfInterestForUpdateDto pointOfInterest)
        //{
        //    var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //    return NotFound();
        //    }

        //    // Find point of interest
        //    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointofinterestid);
        //    if (pointOfInterestFromStore == null)
        //    {
        //    return NotFound();
        //    }

        //    pointOfInterestFromStore.Name = pointOfInterest.Name;
        //    pointOfInterestFromStore.Description = pointOfInterest.Description;

        //    return NoContent();
        //}

        //[HttpPatch("{pointofinterestid}")]
        //public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointofinterestid, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        //{
        //    var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //    return NotFound();
        //    }

        //    // Find point of interest
        //    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointofinterestid);
        //    if (pointOfInterestFromStore == null)
        //    {
        //    return NotFound();
        //    }

        //    var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
        //    {
        //    Name = pointOfInterestFromStore.Name,
        //    Description = pointOfInterestFromStore.Description
        //    };

        //    patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        //    // Validate and Check Not Null Input
        //    if (!ModelState.IsValid || !TryValidateModel(pointOfInterestToPatch))
        //    {
        //    return BadRequest(ModelState);
        //    }

        //    pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
        //    pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

        //    return NoContent();
        //}

        //[HttpDelete("{pointofinterestid}")]
        //public ActionResult DeletePointOfInterest(int cityId, int pointofinterestid)
        //{
        //    var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //    return NotFound();
        //    }

        //    // Find point of interest
        //    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointofinterestid);
        //    if (pointOfInterestFromStore == null)
        //    {
        //    return NotFound();
        //    }

        //    city.PointsOfInterest.Remove(pointOfInterestFromStore);
        //    _mailService.Send("Point of interest deleted", $"point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id}");
        //    return NoContent();
        //}
    }
}
