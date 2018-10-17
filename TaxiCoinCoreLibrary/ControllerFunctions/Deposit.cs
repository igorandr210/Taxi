using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Newtonsoft.Json;
using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nethereum.Hex.HexTypes;
using TaxiCoinCoreLibrary.RequestObjectPatterns;
using TaxiCoinCoreLibrary.TokenAPI;
using TaxiCoinCoreLibrary.Utils;

namespace TaxiCoinCoreLibrary.ControllerFunctions
{
    public class Deposit
    {
        public static TransactionReceipt DepositToContract(DepositPattern req, User user, ModelStateDictionary ModelState)
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
            //removed try catch
            result = TokenFunctionsResults<UInt64>.InvokeByTransaction(user, FunctionNames.Deposit, Value: req.Value, Gas: req.Gas);
            if (result.Status.Value.IsZero)
                ModelState.AddModelError(nameof(User), "Operation failed");

            return result;
        }
    }
}
