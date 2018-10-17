using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Taxi.Models;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Taxi.Entities;
using Taxi.Services;
using Taxi.Helpers;

namespace Taxi.Auth
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IUsersRepository _repository;
        private readonly UserManager<AppUser> _userManager;
        private IEmailSender _emailSender;

        public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions,
            IUsersRepository repository,
            UserManager<AppUser> userManager,
            IEmailSender emailSender)
        {
            _jwtOptions = jwtOptions.Value;
            _repository = repository;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<string> GenerateEncodedToken(string userName, ClaimsIdentity claimsIdentity )
        {
            var claims = new List<Claim>
            {
                new Claim (JwtRegisteredClaimNames.Sub, userName),
                new Claim (JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                claimsIdentity.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Id)
            };
            foreach (var claim in claimsIdentity.Claims)
            {
                if (claim.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.Rol ||
                    claim.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.CustomerId ||
                    claim.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.DriverId ||
                    claim.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.AdminId)
                {
                    claims.Add(claim);
                }
            }

     
            var jwt = new JwtSecurityToken(
                 issuer: _jwtOptions.Issuer,
                 audience: _jwtOptions.Audience,
                 claims: claims,
                 notBefore: _jwtOptions.NotBefore,
                 expires: _jwtOptions.Expiration,
                 signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            
            return encodedJwt;
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() -
                new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalSeconds);

        public async Task <ClaimsIdentity> GenerateClaimsIdentity(string userName, string id)
        {
            
            var user = await _userManager.FindByIdAsync(id);

            var allClaims = (await _userManager.GetClaimsAsync(user)).ToList();

            var rolClaims = (allClaims).Where(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.Rol);

            var idClaims = new List<Claim>();

            var customerIdClaim = (allClaims).FirstOrDefault(c => c.Type == Constants.Strings.JwtClaimIdentifiers.CustomerId);

            var driverIdClaim = (allClaims).FirstOrDefault(c => c.Type == Constants.Strings.JwtClaimIdentifiers.DriverId);
            
            var adminIdClaim = (allClaims).FirstOrDefault(c => c.Type == Constants.Strings.JwtClaimIdentifiers.AdminId);

            if (adminIdClaim != null)
            {
                idClaims.Add(adminIdClaim);
            }

            if (customerIdClaim != null)
            {
                idClaims.Add(customerIdClaim);
            }

            if (driverIdClaim != null)
            {
                idClaims.Add(driverIdClaim);
            }

            ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
            {
                new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Id, id)  
            }.Union(rolClaims).Union(idClaims)); 

            return identity;
        }

        public async Task<string> GenerateRefreshToken(string userName, ClaimsIdentity claimsIdentity, string ip, string userAgent, RefreshToken oldToken =null)
        {
            var claims = new List<Claim>
            {
                new Claim (JwtRegisteredClaimNames.Sub, userName),
                new Claim (JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                claimsIdentity.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Id)
            };
            
            var jwt = new JwtSecurityToken(
                 issuer: _jwtOptions.Issuer,
                 audience: _jwtOptions.Audience,
                 claims: claims,
                 notBefore: _jwtOptions.NotBefore,
                 expires: _jwtOptions.RefleshExpiration,
                 signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var hashedJwt = _userManager.PasswordHasher.HashPassword(new AppUser(), encodedJwt);
            //remove tokens for user if strange activity

            bool delRes = true;

            if (oldToken != null)
            {
                delRes = await _repository.DeleteRefleshToken(oldToken);
            }

            var tokensFromDb = _repository.GetTokensForUser(claimsIdentity.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Id).Value).ToList();

            if (tokensFromDb.Count() > 20)
            {
                foreach (var t in tokensFromDb.ToArray())
                {
                    if (t != null)
                        delRes = await _repository.DeleteRefleshToken(t);
                }
            }
            else
            {
                foreach (var t in tokensFromDb.ToArray())
                {
                    if (t.Expiration < ToUnixEpochDate(DateTime.UtcNow))
                    {
                        delRes = await _repository.DeleteRefleshToken(t);
                    }
                }
            }
            
            await _repository.AddRefreshToken(new Entities.RefreshToken()
            {
                Token = hashedJwt,
                IdentityId = claimsIdentity.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Id).Value,
                Expiration = ToUnixEpochDate(_jwtOptions.RefleshExpiration),
                Ip = ip,
                Useragent = userAgent
            });
            
            return encodedJwt;
        }

        public async Task<TokensDto> RefreshToken(string refreshToken, JwtIssuerOptions jwtOptions, string ip, string userAgent)
        {
            var handler = new JwtSecurityTokenHandler();

            var tokenClaims = (handler.ReadToken(refreshToken) as JwtSecurityToken)?.Claims;

            var expirationTime = tokenClaims.FirstOrDefault(o => o.Type == "exp")?.Value;

            if (expirationTime == null)
            {
                return null;
            }
            var date = ToUnixEpochDate(DateTime.UtcNow);
            if (long.Parse(expirationTime) < ToUnixEpochDate(DateTime.UtcNow))
            {
                return null;
            }

            var uid = tokenClaims.FirstOrDefault(o => o.Type == "id").Value;

            if (uid == null)
            {
                return null;
            }

            var tokensFromDb = _repository.GetTokensForUser(uid).ToList();
            //check if (token + ip) hash match one of user refresh tokens 
            var curToken = tokensFromDb
                .SingleOrDefault(t => (_userManager.PasswordHasher
                .VerifyHashedPassword(new AppUser(), t.Token, refreshToken)) == PasswordVerificationResult.Success);
            
            if (curToken == null)
            {
                return null;
            }

            
            var user = await _userManager.FindByIdAsync(curToken.IdentityId);

            if (curToken.Ip != ip || curToken.Useragent != userAgent)
            {
                try
                {
                    await _emailSender.SendEmailAsync(user.Email, "Security",
                        $"Somebody was attempting to login to your account from {userAgent}, if it was not you consider changing your password");
                }
                catch
                {
                    return null;
                }

                return null;
            }


            //if (tokensFromDb.Count() > 20)
            //{
            //    foreach (var t in tokensFromDb.ToList())
            //    {
            //        if (t!= null)
            //            await _repository.DeleteRefleshToken(t);
            //    }
            //} else
            //{
            //    if (curToken != null)
            //        await _repository.DeleteRefleshToken(curToken);
            //}


            if (user == null)
            {
                return null;
            }

            var claimsIdentity = await GenerateClaimsIdentity(user.UserName, user.Id);
                        
            var newRefreshToken = await GenerateRefreshToken(user.UserName, claimsIdentity, ip, userAgent, curToken);
            
            var newAccessToken =  await GenerateEncodedToken(user.UserName, claimsIdentity);

            var responce = new TokensDto()
            {
                expires_in = (int)jwtOptions.ValidFor.TotalSeconds,
                auth_token = newAccessToken,
                refresh_token = newRefreshToken
            };

            return responce;
        }

        public async Task RemoveRefreshTokens(string userId)
        {
            var tokensFromDb = _repository.GetTokensForUser(userId);

            foreach (var t in tokensFromDb.ToList())
            {
                await _repository.DeleteRefleshToken(t);
            }
        }

        public async Task<ClaimsIdentity> GenerateClaimsIdentityForRegistration(string userName, string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var allClaims = (await _userManager.GetClaimsAsync(user)).ToList();


            var idClaims = new List<Claim>();

            var customerIdClaim = (allClaims).FirstOrDefault(c => c.Type == Constants.Strings.JwtClaimIdentifiers.CustomerId);

            var driverIdClaim = (allClaims).FirstOrDefault(c => c.Type == Constants.Strings.JwtClaimIdentifiers.DriverId);

            var adminIdClaim = (allClaims).FirstOrDefault(c => c.Type == Constants.Strings.JwtClaimIdentifiers.AdminId);

            if (adminIdClaim != null)
            {
                idClaims.Add(adminIdClaim);
            }

            if (customerIdClaim != null)
            {
                idClaims.Add(customerIdClaim);
            }

            if (driverIdClaim != null)
            {
                idClaims.Add(driverIdClaim);
            }

            ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
            {
                new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Id, id)
            }.Union(idClaims));

            return identity;
        }
    }
}
