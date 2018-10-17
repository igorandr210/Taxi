using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Taxi.Auth;
using Taxi.Models;

namespace Taxi.Helpers
{
    public class Tokens
    {
        public static async Task<string> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, JwtIssuerOptions jwtOptions, Guid id, string ip, string userAgent)
        {

            var refreshToken = await jwtFactory.GenerateRefreshToken(userName, identity,ip, userAgent);
            
            var response = new
            {
                id = id,//identity.Claims.Single(c => c.Type == "id").Value,
                auth_token = await jwtFactory.GenerateEncodedToken(userName, identity),
                expires_in = (int)jwtOptions.ValidFor.TotalSeconds,
                refresh_token = refreshToken
            };

            return JsonConvert.SerializeObject(response);
        }

        public static async Task<string> GenerateRegistrationJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, JwtIssuerOptions jwtOptions, Guid id)
        {
            var response = new
            {
                id = id,//identity.Claims.Single(c => c.Type == "id").Value,
                auth_token = await jwtFactory.GenerateEncodedToken(userName, identity),
                expires_in = (int)jwtOptions.ValidFor.TotalSeconds,
            };

            return JsonConvert.SerializeObject(response);
        }
    }
}
