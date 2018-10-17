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
    public class Refund
    {
        public static TransactionReceipt Create(UInt64 id, DefaultControllerPattern req, User user, ModelStateDictionary ModelState)
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
                result = TokenFunctionsResults<int>.InvokeByTransaction(user, FunctionNames.Refund, req.Gas, funcParametrs: id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(User),e.Message);
                return null;
            }

            if (result.Status.Value.IsZero)
                ModelState.AddModelError(nameof(User), "Operation failed");


            return result;
        }

        public static TransactionReceipt Approve(UInt64 id, DefaultControllerPattern req, User user, ModelStateDictionary ModelState)
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
                result = TokenFunctionsResults<int>.InvokeByTransaction(user, FunctionNames.ApproveRefund, req.Gas, funcParametrs: id);
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

        public static TransactionReceipt DisApprove(UInt64 id, DefaultControllerPattern req, User user, ModelStateDictionary ModelState)
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
                result = TokenFunctionsResults<int>.InvokeByTransaction(user, FunctionNames.DisApproveRefund, req.Gas, funcParametrs: id);
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
