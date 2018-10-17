using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using TaxiCoinCoreLibrary.RequestObjectPatterns;
using TaxiCoinCoreLibrary.TokenAPI;

namespace TaxiCoinCoreLibrary.Utils
{
    public class TokenFunctionsResults<TResult>
    {
        public static TResult InvokeByCall(UInt64 id, User user, string funcName, params object[] funcParametrs)
        {
            ContractFunctions contractFunctions = Globals.GetInstance().ContractFunctions;
            var param = new List<object>() { id };
            param.AddRange(funcParametrs);
            return contractFunctions.CallFunctionByName<TResult>(user.PublicKey, user.PrivateKey, funcName, param.ToArray()).Result;
        }

        public static TResult InvokeByCall(User user, string funcName)
        {
            ContractFunctions contractFunctions = Globals.GetInstance().ContractFunctions;
            return contractFunctions.CallFunctionByName<TResult>(user.PublicKey, user.PrivateKey, funcName, null).Result;
        }


        public static TransactionReceipt InvokeByTransaction(User user, string funcName, UInt64 Gas, params object[] funcParametrs)
        {
            ContractFunctions contractFunctions = Globals.GetInstance().ContractFunctions;
            return contractFunctions.CallFunctionByNameSendTransaction(user.PublicKey, user.PrivateKey, funcName, Gas, funcParametrs).Result;
        }

        public static TransactionReceipt InvokeByTransaction(User user, string funcName, UInt64 Value, UInt64 Gas)
        {
            ContractFunctions contractFunctions = Globals.GetInstance().ContractFunctions;
            return contractFunctions.CallFunctionByNameSendTransaction(user.PublicKey, user.PrivateKey, funcName, Value: Value, Gas: Gas, parametrsOfFunction: null).Result;
        }

        public static TransactionReceipt InvokeByTransaction(User user, string funcName, ulong Gas)
        {
            ContractFunctions contractFunctions = Globals.GetInstance().ContractFunctions;
            return contractFunctions.CallFunctionByNameSendTransaction(user.PublicKey, user.PrivateKey, funcName, Gas, null).Result;
        }
    }
}
