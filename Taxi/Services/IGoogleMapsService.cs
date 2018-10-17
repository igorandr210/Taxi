using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Models.Location;

namespace Taxi.Services
{
    public interface IGoogleMapsService
    {
        Task<DirectionsData> GetDirections(double latFrom, double lonFrom, double latTo, double lonTo);
    }
}
