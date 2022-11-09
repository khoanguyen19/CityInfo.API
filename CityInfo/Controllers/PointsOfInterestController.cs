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
    private readonly CityDataStore _cityDataStore;

    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, CityDataStore cityDataStore)
    {
      _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
      _cityDataStore = cityDataStore;
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
      try
      {
        var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
        {
          _logger.LogInformation($"City with id {cityId} wasn't found");
          return NotFound();
        }

        return Ok(city.PointsOfInterest);

      }
      catch (Exception ex)
      {
        _logger.LogCritical($"Exception while getting points of interest", ex);
        return StatusCode(500, "Internal Server Error");
      }
    }

    [HttpGet("{pointofinterestid}", Name = "GetPointOfInterestById")]
    public ActionResult<PointOfInterestDto> GetPointOfInterestById(int cityId, int pointofinterestid)
    {
      var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
      if (city == null)
      {
        return NotFound();
      }

      var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofinterestid);
      if (pointOfInterest == null)
      {
        return NotFound();
      }

      return Ok(pointOfInterest);
    }

    [HttpGet("pointofinterestname")]
    public ActionResult<PointOfInterestDto> GetPointOfInterestByName(int cityId, string pointofinterestname)
    {
      var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
      if (city == null)
      {
        return NotFound();
      }

      var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Name.ToLower().Trim().Contains(pointofinterestname.ToLower().Trim()));
      if (pointOfInterest == null)
      {
        return NotFound();
      }

      return Ok(pointOfInterest);
    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(
        int cityId, PointOfInterestForCreationDto pointOfInterest)
    {
      var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
      if (city == null)
      {
        return NotFound();
      }

      var maxPointOfInterestId = _cityDataStore.Cities
          .SelectMany(c => c.PointsOfInterest)
          .Max(p => p.Id);

      var finalPointOfInterest = new PointOfInterestDto()
      {
        Id = ++maxPointOfInterestId,
        Name = pointOfInterest.Name,
        Description = pointOfInterest.Description,
      };

      city.PointsOfInterest.Add(finalPointOfInterest);

      return CreatedAtRoute("GetPointOfInterestById",
          new
          {
            cityId = cityId,
            pointofinterestid = finalPointOfInterest.Id,
          },
          finalPointOfInterest);
    }

    [HttpPut("{pointofinterestid}")]
    public ActionResult UpdatePointOfInterest(int cityId, int pointofinterestid, PointOfInterestForUpdateDto pointOfInterest)
    {
      var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
      if (city == null)
      {
        return NotFound();
      }

      // Find point of interest
      var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointofinterestid);
      if (pointOfInterestFromStore == null)
      {
        return NotFound();
      }

      pointOfInterestFromStore.Name = pointOfInterest.Name;
      pointOfInterestFromStore.Description = pointOfInterest.Description;

      return NoContent();
    }

    [HttpPatch("{pointofinterestid}")]
    public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointofinterestid, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
      var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
      if (city == null)
      {
        return NotFound();
      }

      // Find point of interest
      var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointofinterestid);
      if (pointOfInterestFromStore == null)
      {
        return NotFound();
      }

      var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
      {
        Name = pointOfInterestFromStore.Name,
        Description = pointOfInterestFromStore.Description
      };

      patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

      // Validate and Check Not Null Input
      if (!ModelState.IsValid || !TryValidateModel(pointOfInterestToPatch))
      {
        return BadRequest(ModelState);
      }

      pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
      pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

      return NoContent();
    }

    [HttpDelete("{pointofinterestid}")]
    public ActionResult DeletePointOfInterest(int cityId, int pointofinterestid)
    {
      var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
      if (city == null)
      {
        return NotFound();
      }

      // Find point of interest
      var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointofinterestid);
      if (pointOfInterestFromStore == null)
      {
        return NotFound();
      }

      city.PointsOfInterest.Remove(pointOfInterestFromStore);
      _mailService.Send("Point of interest deleted", $"point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id}");
      return NoContent();
    }
  }
}
