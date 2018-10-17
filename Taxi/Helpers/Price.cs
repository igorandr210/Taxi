using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Helpers
{
    public static class Price
    {
        public static decimal getPriceInTokens(IConfiguration iConfiguration, double distance)
        {
            return decimal.Round((decimal)(distance * Convert.ToDouble(iConfiguration.GetSection("TokensPerKilometer").Value)), 2, MidpointRounding.AwayFromZero);
        }
        
    }
}
