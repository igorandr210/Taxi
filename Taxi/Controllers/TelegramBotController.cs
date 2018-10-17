using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Taxi.Data;
using Taxi.Entities;
using Taxi.Models;
using Taxi.Models.TelegramBot;
using Newtonsoft.Json;
using Taxi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Taxi.Services;
using Microsoft.EntityFrameworkCore;
using Taxi.Auth;
using Microsoft.Extensions.Options;

namespace Taxi.Controllers
{
    [Route("api/telegrambot")]
    public class TelegramBotController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private IJwtFactory _jwtFactory;
        private IHttpContextAccessor _httpContextAccessor;
        private IUsersRepository _userRepository;
        private JwtIssuerOptions _jwtOptions;

        public TelegramBotController(UserManager<AppUser> userManager, IUsersRepository usersRepository, IOptions<JwtIssuerOptions> jwtOptions, IJwtFactory jwtFactory, ApplicationDbContext dbContext,IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtOptions = jwtOptions.Value;
            _jwtFactory = jwtFactory;
            _userRepository = usersRepository;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpPost("IsRegistered", Name = "IsRegistered")]
        public IActionResult IsRegistered([FromBody]IsRegisteredDto credentials)
        {
            if (ModelState.IsValid)
            {
                if (_dbContext.Customers.Where(u => u.Identity.Email == credentials.Email).Any())
                {
                    return Ok(true);
                }
                else return Ok(false);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("GetToken", Name = "GetToken")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetToken([FromBody]IsRegisteredDto credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userToVerify = await _userManager.FindByEmailAsync(credentials.Email);

            if (userToVerify == null) return await Task.FromResult<IActionResult>(BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username.", ModelState)));

            var identity = await _jwtFactory.GenerateClaimsIdentity(credentials.Email, userToVerify.Id);

            if (identity == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username.", ModelState));
            }
            var customer = _userRepository.GetCustomerByIdentityId(identity.Claims.Single(c => c.Type == Constants.Strings.JwtClaimIdentifiers.Id).Value);

            if (customer == null)
            {
                return NotFound();
            }
            var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"];

            string jwt;
            try
            {
                jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.Email, _jwtOptions,
                    customer.Id, ip, userAgent);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Conflict();
            }
            catch (Exception e)
            {
                return BadRequest();
            }

            return Ok(JsonConvert.DeserializeObject(jwt)); ;
        }
        
    }
}