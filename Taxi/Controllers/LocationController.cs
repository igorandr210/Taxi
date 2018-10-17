using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Services;
using Taxi.Helpers;
using Taxi.Models;

namespace Taxi.Controllers
{
    [Route ("api/location")]
    public class LocationController: Controller
    {
        private IDriverLocRepository _locationRepository;
        public LocationController( IDriverLocRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        //[Authorize(Policy = "Driver")]
        //[ProducesResponseType(204)]
        //[HttpPost("driver")]
        //public IActionResult AddDriverToMap(LatLonDto latLonDto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var driverid = User.Claims.Single(c => c.Type == Constants.Strings.JwtClaimIdentifiers.DriverId).Value;
        //    var res  = _driverLocationRepository.AddUser(Guid.Parse(driverid), latLonDto.Longitude, latLonDto.Latitude);

        //    if (res != true)
        //        return BadRequest();
        //    return NoContent();
        //}

        [Authorize(Policy = "Driver")]
        [ProducesResponseType(204)]
        [HttpPut("driver")]
        public async Task<IActionResult> UpdateDriverLocation([FromBody]LatLonDto latLonDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var driverid = User.Claims.Single(c => c.Type == Constants.Strings.JwtClaimIdentifiers.DriverId).Value;

            //var res = _driverLocationRepository.UpdateUser(Guid.Parse(driverid), latLonDto.Longitude, latLonDto.Latitude, DateTime.Now);

            var res = await _locationRepository.UpdateLocation(Guid.Parse(driverid), latLonDto.Latitude, latLonDto.Longitude);

            if (res != true)
                return Conflict();

            return NoContent();
        }

        //[Authorize(Policy = "Driver")]
        //[ProducesResponseType(204)]
        //[HttpDelete("driver")]
        //public IActionResult RemoveDriverFromMap()
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);
        //    var driverid = User.Claims.Single(c => c.Type == Constants.Strings.JwtClaimIdentifiers.DriverId).Value;

        //    var res = _driverLocationRepository.RemoveUser(Guid.Parse(driverid));
        //    if (res != true)
        //        return BadRequest();
            
        //    return NoContent();
        //}

        [Authorize(Policy = "Customer")]
        [ProducesResponseType(200)]
        [HttpGet]
        public async Task<IActionResult> GetNearDrivers(LatLonDto latLonDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var drivers = await _locationRepository.GetNear(latLonDto.Latitude, latLonDto.Longitude);
            
            var driverLocationToReturn = new List<DriverLocationDto>();

            foreach (var driver in drivers)
            {
                var place = Helpers.Location.PointToPlaceDto(driver.Location);
                driverLocationToReturn.Add(new DriverLocationDto()
                {
                    DriverId = driver.Id,
                    Latitude = place.Latitude,
                    Longitude = place.Longitude
                });
            }

          
            return Ok(driverLocationToReturn);
        }
    }
}
