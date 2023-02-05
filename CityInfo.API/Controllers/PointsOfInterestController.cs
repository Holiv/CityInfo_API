using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using CityInfo.API.Models.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : Controller
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _localMailService;
        private readonly CitiesDataStore _citiesDataStore;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService localMailService, CitiesDataStore citiesDataStore)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localMailService = localMailService ?? throw new ArgumentNullException(nameof(localMailService));
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(localMailService));
        }

        [HttpGet]
        public ActionResult<IEnumerable<PointsOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            CityDto? city = _citiesDataStore.Cities.FirstOrDefault(city => city.Id == cityId);

            try
            {
                if (city == null)
                {
                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }
            catch
            {
                _logger.LogCritical($"City with id {cityId} wasn't found when accessing points of interest.");
                return StatusCode(500, "A problem happened while handling your request");
            }

            
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public ActionResult<PointsOfInterestDto> GetPointOfInterest(
            int cityId, int pointOfInterestId)
        {
            CityDto? city = _citiesDataStore.Cities.FirstOrDefault(city => city.Id == cityId);

            if (city == null)
            {
               return NotFound();
            }

            PointsOfInterestDto? pointsOfInterest = city.PointsOfInterest.FirstOrDefault(pi => pi.Id == pointOfInterestId);
            if (pointsOfInterest == null)
            {
                return NotFound();
            }
            return Ok(pointsOfInterest);
        }


        [HttpPost] 
        public ActionResult<PointsOfInterestDto> CreatePointOfInterest(int cityId, PointsOfInterestForCreationDTO pointsOfInterest)
        {
           //getting the list of cities in a variable to avoid repetition
            var cities = _citiesDataStore.Cities;

            //checks if city id sent in the URI(cityId) exists
            //if exists, get it into the variable
            var city = cities.FirstOrDefault(city => city.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            
            //getting the max Id value from the existing cities points of interests
            var maxPointOfInterest = cities
                                    .SelectMany(city => city.PointsOfInterest)
                                    .Max(maxId => maxId.Id);

            //creating a new instance of the PointsOfInterestDTO class
            //feed the new instance with the data passed in the API request Body
            PointsOfInterestDto finalPointOfInteres = new PointsOfInterestDto()
            {
                Id = ++maxPointOfInterest,
                Name = pointsOfInterest.Name,
                Description = pointsOfInterest.Description
            };

            //Adding the new instance of the PointsOfInterest to the respective City
            city.PointsOfInterest.Add(finalPointOfInteres);

            //returning the status code to the user
            //CreatedAtRout returns the 204 Created status code and accepts three parameters
            //1 - Action identifier where the new added data can be found and requested
            //2 - Object data sent by the user can see exactly what was created
            //3 - The new created object 
            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = finalPointOfInteres.Id
                },
                finalPointOfInteres);
        }

        // 3 Parameters in the Put Action to update nested data
        // - cityId to get the correct city where the nested data lives
        // - pointOfInterestId to identify the data to be updated
        // - New instance of the object to be updated that is injected and receives the data from the request body
        [HttpPut("{pointofinterestid}")]
        public ActionResult UpdatePointsOfInterest(int cityId, int pointOfInterestId, PointsOfInterestForUpdatingDto pointsOfInterestForUpdatingDto)
        {
            var cities = _citiesDataStore.Cities;

            //getting the correct city / checking if it exists
            var city = cities.FirstOrDefault(city => city.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            //getting the correct nested data to be updates / checking if it exists
            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(pInt => pInt.Id == pointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound();
            };

            //updating operation
            pointOfInterest.Name = pointsOfInterestForUpdatingDto.Name;
            pointOfInterest.Description = pointsOfInterestForUpdatingDto.Description;

            return NoContent();
        }

        // Third parameter is JsonPatchDocument<T> type that will allows to work with the Json Path Document format sent in the request body.
        // The data sent is used to feed a new instance of the Dto object then the JsonPatchDocument instance is applied to it. We can also checks the ModelState in case of a user mistake in the request body
        [HttpPatch("{pointofinterestid}")]
        public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointsOfInterestForUpdatingDto> patchDocument)
        {
            var cities = _citiesDataStore.Cities;

            var city = cities.FirstOrDefault(city => city.Id == cityId);
            if (city is null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(pInt => pInt.Id == pointOfInterestId);
            if (pointOfInterest is null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch =
                new PointsOfInterestForUpdatingDto()
                {
                    Name = pointOfInterest.Name,
                    Description = pointOfInterest.Description
                };
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            // ModelState will check if the Model Input is valid
            // The user could be passed a property that does not exist
            // The Model input in this case is the patchDocument (JsonPatchDocument)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            };

            // Validante the Model for Validation Annotation on the Dto
            // The last validation does not checks for empty fields, only if the structure of the document to patch matches the structure of the Dto object
            // To check for validation annotation the TryValidateModel is used on the object created that is going to be used in the patch process
            if (!TryValidateModel(pointOfInterestToPatch))
                return BadRequest(ModelState);
            

            pointOfInterest.Name = pointOfInterestToPatch.Name;
            pointOfInterest.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{pointofinterestid}")]
        public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var cities = _citiesDataStore.Cities;

            var city = cities.FirstOrDefault(city => city.Id == cityId);
            if (city is null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(pInt => pInt.Id == pointOfInterestId);
            if (pointOfInterest is null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterest);
            _localMailService.Send($"Point of interest deleted.", $"Point of interest {pointOfInterest.Name} with id {pointOfInterest.Id} was deleted");
            return NoContent();
        }
    }
}

