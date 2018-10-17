using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql;
using Taxi.Data;
using Taxi.Entities;
using Taxi.Helpers;
using Taxi.Models;
using Taxi.Models.Trips;

namespace Taxi.Services
{
    public class TripsRepository : ITripsRepository
    {
        private ApplicationDbContext _dataContext;
        private IUsersRepository _userRepository;


        public TripsRepository(ApplicationDbContext dataContext, 
            IUsersRepository usersRepository)
        {
            _dataContext = dataContext;
            _userRepository = usersRepository;
        }

        public void InsertTrip(Trip trip, double lat1, double lon1,double lat2,double lon2)
        {
            var query = string.Format(System.IO.File.ReadAllText("InsertTripQuery.txt"));
            
            List<NpgsqlParameter> sqlParameters = new List<NpgsqlParameter>();
            if (trip.Id == default(Guid))
                trip.Id = Guid.NewGuid();
            sqlParameters.Add(new NpgsqlParameter("Id", trip.Id));
            sqlParameters.Add(new NpgsqlParameter("lon1", lon1));
            sqlParameters.Add(new NpgsqlParameter("lat1", lat1));
            sqlParameters.Add(new NpgsqlParameter("lon2", lon2));
            sqlParameters.Add(new NpgsqlParameter("lat2", lat2));
            sqlParameters.Add(new NpgsqlParameter("CustomerId", trip.CustomerId));
            var did =(object) trip.DriverId ?? DBNull.Value;
            sqlParameters.Add(new NpgsqlParameter("DriverId", did));
            sqlParameters.Add(new NpgsqlParameter("LastLat", trip.LastLat));
            sqlParameters.Add(new NpgsqlParameter("LastLon", trip.LastLon));
            sqlParameters.Add(new NpgsqlParameter("Distance", trip.Distance));
            sqlParameters.Add(new NpgsqlParameter("LastUpdateTime", trip.LastUpdateTime));
            sqlParameters.Add(new NpgsqlParameter("CreationTime", trip.CreationTime));
            sqlParameters.Add(new NpgsqlParameter("DriverTakeTripTime", trip.DriverTakeTripTime));
            sqlParameters.Add(new NpgsqlParameter("StartTime", trip.StartTime));
            sqlParameters.Add(new NpgsqlParameter("FinishTime", trip.FinishTime));
            sqlParameters.Add(new NpgsqlParameter("Price", trip.Price));
            sqlParameters.Add(new NpgsqlParameter("ContractId", trip.ContractId));
            //sqlParameters.Add(new NpgsqlParameter("lon1", lon1));
            //sqlParameters.Add(new NpgsqlParameter("lat1", lat1));
            //sqlParameters.Add(new NpgsqlParameter("lon2", lon2));
            //sqlParameters.Add(new NpgsqlParameter("lat2", lat2));
            _dataContext.Database.ExecuteSqlCommand(query, sqlParameters);
        }

        public bool AddRefundRequest(RefundRequest refundRequest)
        {
            _dataContext.RefundRequests.Add(refundRequest);
            try
            {
                _dataContext.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool AddContract(Contract contract)
        {
            _dataContext.Contracts.Add(contract);
            try
            {
                _dataContext.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public Contract GetContract(ulong id)
        {
            return _dataContext.Contracts.FirstOrDefault(c => c.Id == (long)id);
        }

        public async Task<bool> AddTripHistory(TripHistory tripHistory)
        {
            await _dataContext.TripHistories.AddAsync(tripHistory);
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public PagedList<TripDto> GetNearTrips(double lon, double lat, PaginationParameters paginationParameters)
        {
            //probably change it
            //var trips = _dataContext.Places.Where(p => p.IsFrom == true)
            //    .OrderBy(d => d.Location.Distance(Helpers.Location.pointFromLatLng(lat, lon))).Include(t=> t.Trip);
            var query = string.Format(System.IO.File.ReadAllText("GetNearQuery.txt"));
            List<NpgsqlParameter> sqlParameters = new List<NpgsqlParameter>();
            sqlParameters.Add(new NpgsqlParameter("lon", lon));
            sqlParameters.Add(new NpgsqlParameter("lat", lat));
            sqlParameters.Add(new NpgsqlParameter("items", paginationParameters.PageSize));
            sqlParameters.Add(new NpgsqlParameter("page", paginationParameters.PageNumber));
            var trips = _dataContext.Trips.FromSql(query, sqlParameters.ToArray()).ToList();
            
            //var trips = _dataContext.Trips.Where(p => p.DriverId == null)
            //    .OrderBy(d => d.From.Distance(Helpers.Location.pointFromLatLng(lat, lon)))
            //    .ToList();

            var tripsDto = new List<TripDto>();

            foreach (var t in trips)
            {
                
                var customer = _userRepository.GetCustomerById(t.CustomerId);
                
                tripsDto.Add(new TripDto() {
                    CustomerId = customer.Id,
                    FirstName = customer.Identity.FirstName,
                    LastName = customer.Identity.LastName,
                    From = Helpers.Location.PointToPlaceDto(t.From),
                    To = Helpers.Location.PointToPlaceDto(t.To)
                });
            }

            var pagedList = new PagedList<TripDto>(tripsDto, _dataContext.Trips.Count(), paginationParameters.PageNumber, paginationParameters.PageSize);

            return pagedList;
        }

        public Trip GetTrip(Guid customerId, bool includeRoutes = false)
        {
            if (!includeRoutes) return _dataContext.Trips.FirstOrDefault(t => t.CustomerId == customerId);
            
            return _dataContext.Trips.Include(t=>t.RouteNodes).FirstOrDefault(t => t.CustomerId == customerId);
            
        }
        
        public async Task<bool> AddNode(TripRouteNode node)
        {
            await _dataContext.TripRouteNodes.AddAsync(node);
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }
        public Trip GetTripByDriver(Guid driverId, bool includeRoutes = false)
        {
            if (!includeRoutes)
            {
                return _dataContext.Trips.FirstOrDefault(t => t.DriverId == driverId);
            }
            else
            {
                return _dataContext.Trips.Include(tr => tr.RouteNodes).FirstOrDefault(t => t.DriverId == driverId);
            }
        }

        public PagedList<TripHistory> GetTripHistoriesForCustomer(Guid CustomerId, TripHistoryResourceParameters resourceParameters)
        {
            var beforePaging = _dataContext.TripHistories.OrderByDescending(h => h.FinishTime).Where(t => t.CustomerId == CustomerId);
            return PagedList<TripHistory>.Create(beforePaging, resourceParameters.PageNumber, resourceParameters.PageSize);
        }

        public PagedList<TripHistory> GetTripHistoriesForDriver(Guid DriverID, TripHistoryResourceParameters resourceParameters)
        {
            var beforePaging = _dataContext.TripHistories.OrderByDescending(h => h.FinishTime).Where(t => t.DriverId == DriverID)
                ;
            return PagedList<TripHistory>.Create(beforePaging, resourceParameters.PageNumber, resourceParameters.PageSize);
        }

        public async Task<TripHistory> GetTripHistory(Guid id)
        {
            return await _dataContext.TripHistories.FindAsync(id);
        }

        public async Task<List<TripHistoryRouteNode>> GetTripRouteNodes(Guid tripHistoryId)
        {
            return await _dataContext.TripHistoryRouteNodes.OrderBy(o => o.UpdateTime).Where(n=>n.TripHistoryId == tripHistoryId)
                .ToListAsync();
        }

        public bool RemoveTrip(Guid customerId)
        {
            var tripToRemove = _dataContext.Trips.FirstOrDefault(t => t.CustomerId == customerId);

            if (tripToRemove != null)
            {
                _dataContext.Trips.Remove(tripToRemove);
                try
                {
                    _dataContext.SaveChanges();
                }
                catch
                {
                    return false;
                }
                
            }
            return true;
        }

        public async Task<bool> UpdateTrip(Trip trip, PlaceDto from=null , PlaceDto to = null)
        {
            try
            {
                _dataContext.Update(trip);

                _dataContext.SaveChanges();

                if (from != null)
                {
                    var queryfrom = string.Format(System.IO.File.ReadAllText("UpdateFromQuery.txt"));

                    List<NpgsqlParameter> sqlParameters = new List<NpgsqlParameter>();

                    sqlParameters.Add(new NpgsqlParameter("lon", from.Longitude));

                    sqlParameters.Add(new NpgsqlParameter("lat", from.Latitude));

                    sqlParameters.Add(new NpgsqlParameter("CustomerId", trip.CustomerId));

                    await _dataContext.Database.ExecuteSqlCommandAsync(queryfrom, sqlParameters);
                }

                if (to != null)
                {
                    var queryto = string.Format(System.IO.File.ReadAllText("UpdateToQuery.txt"));

                    List<NpgsqlParameter> sqlParameters = new List<NpgsqlParameter>();

                    sqlParameters.Add(new NpgsqlParameter("lon", to.Longitude));

                    sqlParameters.Add(new NpgsqlParameter("lat", to.Latitude));

                    sqlParameters.Add(new NpgsqlParameter("CustomerId", trip.CustomerId));

                    await _dataContext.Database.ExecuteSqlCommandAsync(queryto, sqlParameters);
                }
                _dataContext.Entry(trip).Reload();
            }
            catch(Exception e)
            {
                
                return false;
            }

            return true;
        }
      
    }
}
