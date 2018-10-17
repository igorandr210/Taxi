using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Taxi.Entities;
using Taxi.Models.Contracts;
using Taxi.Models.Location;
using TaxiCoinCoreLibrary.ControllerFunctions;
using TaxiCoinCoreLibrary.RequestObjectPatterns;

namespace Taxi.Controllers.Contracts
{
    [Route("api/deposit")]
    public class DepositController: Controller
    {
        private UserManager<AppUser> _userManager;

        public DepositController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DepositToContractForUser([FromBody]DepositDto deposit)
        {
            var uid = User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.Id)?.Value;
            var user = await _userManager.FindByIdAsync(uid);

            if (user == null)
                return NotFound();

            var depositPattern = new DepositPattern()
            {
                Value = deposit.Value
            };
            try
            {
                var res = Deposit.DepositToContract(depositPattern, new User() {PrivateKey = user.PrivateKey}, ModelState);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(res);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(DepositDto), e.Message);

                return BadRequest(ModelState);
            }
        }

    }
}
