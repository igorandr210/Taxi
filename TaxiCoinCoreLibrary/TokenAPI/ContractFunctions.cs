using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Threading.Tasks;

namespace TaxiCoinCoreLibrary.TokenAPI
{
    public class ContractFunctions
    {
        public string Abi { get; set; }
        public string ByteCode { get; set; }
        public string ContractAddress { get; set; }

        public ContractFunctions(string abi, string byteCode)
        {
            Abi = abi;
            ByteCode = byteCode;
        }

        public async Task<TransactionReceipt> DeployContract(string senderAddress, string password, ulong gas)
        {
            var web3 = GetWeb3Account(senderAddress, password);

            var receipt = await web3.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(Abi, ByteCode, senderAddress, new Nethereum.Hex.HexTypes.HexBigInteger(gas), null);
            ContractAddress = receipt.ContractAddress;
            return receipt;
        }

        public async Task<TypeOfResult> CallFunctionByName<TypeOfResult>(string senderAddress, string password, string functionName, params object[] parametrsOfFunction)
        {
            var web3 = GetWeb3Account(senderAddress, password);

            var contract = web3.Eth.GetContract(Abi, ContractAddress);
            var calledFunction = contract.GetFunction(functionName);

            var result = await calledFunction.CallAsync<TypeOfResult>(parametrsOfFunction);
            return result;
        }

        public async Task<TransactionReceipt> CallFunctionByNameSendTransaction(string senderAddress, string password, string functionName, UInt64 Value, UInt64 Gas, params object[] parametrsOfFunction)
        {
            var web3 = GetWeb3Account(senderAddress, password);

            var contract = web3.Eth.GetContract(Abi, ContractAddress);
            var calledFunction = contract.GetFunction(functionName);

            var gas = new HexBigInteger(Gas);
            var receipt = await calledFunction.SendTransactionAndWaitForReceiptAsync(senderAddress, gas, new HexBigInteger(Value), null, parametrsOfFunction);
            return receipt;
        }

        public async Task<TransactionReceipt> CallFunctionByNameSendTransaction(string senderAddress, string password, string functionName, UInt64 Gas, params object[] parametrsOfFunction)
        {
            var web3 = GetWeb3Account(senderAddress, password);

            var contract = web3.Eth.GetContract(Abi, ContractAddress);
            var calledFunction = contract.GetFunction(functionName);

            var gas = new HexBigInteger(Gas);
            var receipt = await calledFunction.SendTransactionAndWaitForReceiptAsync(senderAddress, gas, null, null, parametrsOfFunction);
            return receipt;
        }

        public async Task<string> GetUserBalance(string senderAddress, string password)
        {
            var web3 = GetWeb3Account(senderAddress, password);
            var res = await web3.Eth.GetBalance.SendRequestAsync(senderAddress);
            return res.Value.ToString();
        }

        public async Task<Payment> DeserializePaymentById(string senderAddress, string password, UInt64 id)
        {
            var web3 = GetWeb3Account(senderAddress, password);
            var contract = web3.Eth.GetContract(Abi, ContractAddress);
            var payments = contract.GetFunction("payments");

            var result = await payments.CallDeserializingToObjectAsync<Payment>(id);
            return result;
        }


        private Web3 GetWeb3Account(string senderAddress, string password)
        {
            var account = new Account(password);
            return new Web3(account, "https://kovan.infura.io/wEt3RDye41RKD5g4XKNS");
        }
    }
}
