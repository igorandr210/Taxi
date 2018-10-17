using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Taxi.Entities;
using Taxi.Models;

namespace Taxi.Auth
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity claimsIdentity);
        Task<ClaimsIdentity> GenerateClaimsIdentity(string userName, string id);
        Task<ClaimsIdentity> GenerateClaimsIdentityForRegistration(string userName, string id);
        Task<string> GenerateRefreshToken(string userName, ClaimsIdentity claimsIdentity, string ip, string userAgent, RefreshToken oldToken = null);
        Task<TokensDto> RefreshToken(string refreshToken, JwtIssuerOptions jwtOptions, string ip, string userAgent);
        Task RemoveRefreshTokens(string userId);
    }
}
