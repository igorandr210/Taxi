using Nethereum.Signer;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nethereum.Hex.HexTypes;
using TaxiCoinCoreLibrary.RequestObjectPatterns;
using TaxiCoinCoreLibrary.TokenAPI;
using TaxiCoinCoreLibrary.Utils;

namespace TaxiCoinCoreLibrary.ControllerFunctions
{
    public class Balance
    {
        public static async Task<string> GetTokenBalance(User user, ModelStateDictionary ModelState)
        {
            try
            {
                user.PublicKey = EthECKey.GetPublicAddress(user.PrivateKey);
            }
            catch
            {
                ModelState.AddModelError(nameof(user.PublicKey), "Unable to get public key");
                return null;
            }
            ulong res;
            var contractFunctions = Globals.GetInstance().ContractFunctions;

            try
            {
                res = await contractFunctions.CallFunctionByName<UInt64>(user.PublicKey, user.PrivateKey, FunctionNames.Balance, user.PublicKey);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(user), "Unable to get balance");
                return null;
            }
            return res.ToString();
        }

        public static async Task<string> GetEthereumBalance(User user, ModelStateDictionary ModelState)
        {
            try
            {
                user.PublicKey = EthECKey.GetPublicAddress(user.PrivateKey);
            }
            catch
            {
                ModelState.AddModelError(nameof(user.PublicKey), "Unable to get public key");
                return null;
            }

            var contractFunctions = Globals.GetInstance().ContractFunctions;
            string res = null;

            try
            {
                res = await contractFunctions.GetUserBalance(user.PublicKey, user.PrivateKey);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(user), "Unable to get balance");
                return null;
            }
            
            return res;
        }
    }
}
