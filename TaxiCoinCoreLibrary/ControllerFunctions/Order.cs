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
    public class Order
    {
        public static TransactionReceipt GetOrder(UInt64 id, DefaultControllerPattern req, User user, ModelStateDictionary ModelState)
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
                result = TokenFunctionsResults<TransactionReceipt>.InvokeByTransaction(user, FunctionNames.GetOrder, req.Gas, funcParametrs: id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(user), e.Message);
                return null;
            }
            if (result.Status.Value.IsZero)
                ModelState.AddModelError(nameof(User), "Operation failed");

            return result;
        }

        public static TransactionReceipt CompleteOrder(UInt64 id, DefaultControllerPattern req, User user, ModelStateDictionary ModelState)
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
                result = TokenFunctionsResults<int>.InvokeByTransaction(user, FunctionNames.CompleteOrder, req.Gas, funcParametrs: id);
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
