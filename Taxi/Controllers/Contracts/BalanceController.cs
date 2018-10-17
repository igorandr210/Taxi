using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Taxi.Entities;
using Taxi.Models.Contracts;
using Taxi.Models.Location;
using Taxi.Services;
using TaxiCoinCoreLibrary.ControllerFunctions;

namespace Taxi.Controllers.Contracts
{

    [Route("api/balance")]
    public class BalanceController:Controller
    {
        private IUsersRepository _usersRepository;
        private UserManager<AppUser> _userManager;

        public BalanceController(IUsersRepository usersRepository,
            UserManager<AppUser> userManager)
        {
            _usersRepository = usersRepository;
            _userManager = userManager;
        }

        [HttpPut("changekey")]
        [Authorize]
        public async Task<IActionResult> ChangePrivateKey([FromBody]ChangeKeyDto key)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var uid = User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.Id)?.Value;
            var user = await _userManager.FindByIdAsync(uid);
            user.PrivateKey = key.PrivateKey;
            var res = await _userManager.UpdateAsync(user);
            if (!res.Succeeded)
                return Conflict();
            return NoContent();
        }

        [HttpGet("ethereum")]
        [Authorize]
        public async Task<IActionResult> GetEthereumBalanceForUser()
        {
            var uid = User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.Id)?.Value;
            var user = await _userManager.FindByIdAsync(uid);

            if (user == null)
                return NotFound();

            var balance = await Balance.GetEthereumBalance(new TaxiCoinCoreLibrary.RequestObjectPatterns.User(){PrivateKey = user.PrivateKey}, ModelState);
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrEmpty(balance))
                return NotFound();

            return Ok(balance);
        }

        [HttpGet("tokens")]
        [Authorize]
        public async Task<IActionResult> GetTokenBalanceForUser()
        {
            var uid = User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.Id)?.Value;
            var user = await _userManager.FindByIdAsync(uid);

            if (user == null)
                return NotFound();

            var balance = await Balance.GetTokenBalance(new TaxiCoinCoreLibrary.RequestObjectPatterns.User(){PrivateKey = user.PrivateKey}, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrEmpty(balance))
                return NotFound();

            return Ok(balance);
        }

    }
}
