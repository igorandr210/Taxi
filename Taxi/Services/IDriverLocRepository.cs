using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Entities;
using Taxi.Helpers;

namespace Taxi.Services
{
    public interface IDriverLocRepository
    {
        Task<bool> UpdateLocation(Guid driverId, double lat, double lon);

        Task<List<Driver>> GetNear(double lat, double lon);
    }
}
