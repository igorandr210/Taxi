using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TaxiCoinCoreLibrary.RequestObjectPatterns;
using TaxiCoinCoreLibrary.TokenAPI;
using TaxiCoinCoreLibrary.Utils;

namespace TaxiCoinCoreLibrary.ControllerFunctions
{
    public class Deploy
    {
        public static async Task<string> DeployContract(DefaultControllerPattern req, User user)
        {
            user.PublicKey = EthECKey.GetPublicAddress(user.PrivateKey);
            TransactionReceipt contractReceipt;
            ContractFunctions contractFunctions;
            try
            {
                contractFunctions = Globals.GetInstance().ContractFunctions;
                contractReceipt = await contractFunctions.DeployContract(user.PublicKey, user.PrivateKey, req.Gas);
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e.Message);
            }

            return JsonConvert.SerializeObject(contractReceipt);
        }

        public static string GetApiFromContractAddress(DeployControllerPattern req)
        {
            Globals.GetInstance().ContractFunctions.ContractAddress = req.Address;
            return JsonConvert.SerializeObject(new { Status = "OK!" });
        }
    }
}
