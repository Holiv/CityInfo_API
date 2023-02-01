using System;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<CityDto>> GetCities()
        {
            //return new JsonResult(CitiesDataStore.Current.Cities);
            return Ok(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCity(int id)
        {
            
            CityDto? cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == id);

            if (cityToReturn == null)
            {
                return NotFound();
            }
            return Ok(cityToReturn);          
            //return new JsonResult(CitiesDataStore.Current.Cities.Single(city => city.Id == id));
        }
    }
}

