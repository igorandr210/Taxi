using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Taxi.Entities;
using Taxi.Models.Trips;

namespace Taxi.Helpers
{
    public static class ComplexMapping
    {
        public static TripHistory HistoryFromTrip(Trip trip)
        {
            var tripHistory = Mapper.Map<TripHistory>(trip);

            foreach (var rnode in trip.RouteNodes)
            {
                tripHistory.TripHistoryRouteNodes.Add(Mapper.Map<TripHistoryRouteNode>(rnode));
            }

            return tripHistory;
        }
    }
}
