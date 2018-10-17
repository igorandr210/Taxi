using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Taxi.Data;
using Taxi.Entities;
using Taxi.Helpers;
using Taxi.Models.Location;

namespace Taxi.Services
{
    public class DriverLocationRepository: IDriverLocRepository
    {
        private ApplicationDbContext _dataContext;

        public DriverLocationRepository(ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<bool> UpdateLocation(Guid driverId, double lat, double lon)
        {
            try
            {
                var query = string.Format(System.IO.File.ReadAllText("UpdateLocQuery.txt"));

                List<NpgsqlParameter> sqlParameters = new List<NpgsqlParameter>();

                sqlParameters.Add(new NpgsqlParameter("lon", lon));

                sqlParameters.Add(new NpgsqlParameter("lat", lat));

                sqlParameters.Add(new NpgsqlParameter("DriverId", driverId));

                await _dataContext.Database.ExecuteSqlCommandAsync(query, sqlParameters);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Driver>> GetNear(double lat, double lon )
        {
            var items = 10;

            var page = 1;

            var query = string.Format(System.IO.File.ReadAllText("LocationQuery.txt"));

            List<NpgsqlParameter> sqlParameters = new List<NpgsqlParameter>();

            sqlParameters.Add(new NpgsqlParameter("lon", lon));

            sqlParameters.Add(new NpgsqlParameter("lat", lat));

            sqlParameters.Add(new NpgsqlParameter("items", items));

            sqlParameters.Add(new NpgsqlParameter("page", page));

            var drivers = await _dataContext.Drivers.FromSql(query, sqlParameters.ToArray()).ToListAsync();

            return drivers;
        }
    }
}
