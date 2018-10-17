using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nethereum.Hex.HexTypes;
using TaxiCoinCoreLibrary.RequestObjectPatterns;
using TaxiCoinCoreLibrary.TokenAPI;
using TaxiCoinCoreLibrary.Utils;

namespace TaxiCoinCoreLibrary.ControllerFunctions
{
    public class Comission
    {
        public static async Task<TransactionReceipt> GetCommision(ComissionControllerPattern req, User user, ModelStateDictionary ModelState)
        {
            TransactionReceipt res = null;
            try
            {
                user.PublicKey = EthECKey.GetPublicAddress(user.PrivateKey);
            }
            catch
            {
                ModelState.AddModelError(nameof(user.PublicKey), "Unable to get public key");
                return res;
            }

            ContractFunctions contractFunctions = Globals.GetInstance().ContractFunctions;
            
            try
            {
                res = await contractFunctions.CallFunctionByNameSendTransaction(user.PublicKey, user.PrivateKey, FunctionNames.SetComission, req.Gas, parametrsOfFunction: req.Comission);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(User), "Unable to complete the operation");    
            }
            if (res.Status.Value.IsZero)
                ModelState.AddModelError(nameof(User), "Operation failed");

            return res;
        }
    }
}
