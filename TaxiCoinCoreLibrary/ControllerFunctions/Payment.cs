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
    public class Payment
    {
        public static async Task<TokenAPI.Payment> GetById(UInt64 id, User user, ModelStateDictionary ModelState)
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
            ContractFunctions contractFunctions = Globals.GetInstance().ContractFunctions;
            TokenAPI.Payment res;
            try
            {
                res = await contractFunctions.DeserializePaymentById(user.PublicKey, user.PrivateKey, id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(User), e.Message);
                return null;
            }
            
            return res;
        }

        public static TransactionReceipt Create(UInt64 id, CreatePaymentPattern req, User user, ModelStateDictionary ModelState)
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
            TransactionReceipt result;
            try
            {
                result = TokenFunctionsResults<int>.InvokeByTransaction(user, FunctionNames.CreatePayment, req.Gas, new object[] { id, req.Value });
            }
            catch (Exception e)
            {

                ModelState.AddModelError(nameof(User), e.Message);
                return null;
            }
            if (result.Status.Value.IsZero)
                ModelState.AddModelError(nameof(User), "Operation failed");

            return result;
        }
    }
}
