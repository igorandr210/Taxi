using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Entities;
using Taxi.Models;
using Taxi.Services;
using TaxiCoinCoreLibrary.RequestObjectPatterns;

namespace Taxi.Hubs
{
    public class RouteHub : Hub
    {

        private IUsersRepository _usersRepository;
        private IDriverLocRepository _locationRepository;

        public RouteHub(IUsersRepository usersRepository,
            IDriverLocRepository locationRepository)   {
            _usersRepository = usersRepository;
            _locationRepository = locationRepository;
        }
        

        public async Task Send(string customerConnectionId, double lat, double lon)
        {
            if(!string.IsNullOrEmpty(customerConnectionId))
            {
                await Clients.Client(customerConnectionId).SendAsync("postGeoData", lat, lon);
            }          
        }

        [Authorize(Policy = "Driver")]
        public async Task UpdateLocation( double lat, double lon)
        {
            var driverid = Context.User.Claims.Single(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.DriverId).Value;
            
            var res = await _locationRepository.UpdateLocation(Guid.Parse(driverid), lat, lon);
        }


        [Authorize(Policy = "Customer")]
        
        public async Task GetNearDrivers(double lat, double lon)
        {
            
            var drivers = await _locationRepository.GetNear(lat, lon);

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
            await Clients.Client(Context.ConnectionId).SendAsync("nearDrivers", driverLocationToReturn);
        }

        [Authorize(Policy = "Customer")]
        public async Task ConnectCustomer()
        {
            var customerId = Context.User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.CustomerId)?.Value;
            var customer = _usersRepository.GetCustomerById(Guid.Parse(customerId));
            customer.ConnectionId = Context.ConnectionId;
            var res = await _usersRepository.UpdateCustomer(customer);
            if (!res)
                throw new Exception("update failed");
        }

        [Authorize(Policy = "Driver")]
        public async Task ConnectDriver()
        {          
            var driverId = Context.User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.DriverId)?.Value;
            var driver = _usersRepository.GetDriverById(Guid.Parse( driverId));
            driver.ConnectionId = Context.ConnectionId;
            var res = await _usersRepository.UpdateDriver(driver);
            if (!res)
                throw new Exception("update failed");
        }


        public override async Task OnDisconnectedAsync(Exception e)
        {
            //var customerId = Context.User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.CustomerId)?.Value;
            var connId = Context.ConnectionId;
            
            var customer = _usersRepository.GetCustomerByConnectionId( connId);
            if (customer != null)
            {
                customer.ConnectionId = null;
                var res = await _usersRepository.UpdateCustomer(customer);
                if (!res)
                    throw new Exception("update failed");
            }
            
            var driver = _usersRepository.GetDriverByConnectionId( connId);
            if (driver != null)
            {
                driver.ConnectionId = null;
                var res = await _usersRepository.UpdateDriver(driver);
                if (!res)
                    throw new Exception("update failed");
            }

                
            await base.OnDisconnectedAsync(e);
        }
    }
}
