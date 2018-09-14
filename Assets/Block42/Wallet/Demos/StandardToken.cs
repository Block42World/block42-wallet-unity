using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using UnityEngine;
using System.Threading.Tasks;
using Nethereum.Web3;
using System.Numerics;

namespace Block42
{

	// A ERC20 standard token class using Web3 for demo purposes, you can delete this file if you not using it.
    public class StandardToken
    {

		private string _abi = "[{\"constant\":true,\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"name\":\"\",\"type\":\"string\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_spender\",\"type\":\"address\"},{\"name\":\"_value\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"token\",\"type\":\"address\"}],\"name\":\"reclaimToken\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_from\",\"type\":\"address\"},{\"name\":\"_to\",\"type\":\"address\"},{\"name\":\"_value\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"INITIAL_SUPPLY\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"name\":\"\",\"type\":\"uint8\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[],\"name\":\"unpause\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"paused\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_spender\",\"type\":\"address\"},{\"name\":\"_subtractedValue\",\"type\":\"uint256\"}],\"name\":\"decreaseApproval\",\"outputs\":[{\"name\":\"success\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[],\"name\":\"pause\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"name\":\"\",\"type\":\"address\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"name\":\"\",\"type\":\"string\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_to\",\"type\":\"address\"},{\"name\":\"_value\",\"type\":\"uint256\"}],\"name\":\"transfer\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"from_\",\"type\":\"address\"},{\"name\":\"value_\",\"type\":\"uint256\"},{\"name\":\"data_\",\"type\":\"bytes\"}],\"name\":\"tokenFallback\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_spender\",\"type\":\"address\"},{\"name\":\"_addedValue\",\"type\":\"uint256\"}],\"name\":\"increaseApproval\",\"outputs\":[{\"name\":\"success\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_owner\",\"type\":\"address\"},{\"name\":\"_spender\",\"type\":\"address\"}],\"name\":\"allowance\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[],\"name\":\"Pause\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[],\"name\":\"Unpause\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"previousOwner\",\"type\":\"address\"}],\"name\":\"OwnershipRenounced\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"spender\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"}]";
		private string _contractAddress = "0xb4fddd37602b03fa086c42bfa7b9739be38682c3";

        private Web3 _web3;
        private Contract _tokenContract;

        public StandardToken()
        {
            _web3 = new Web3(WalletSettings.current.networkUrl);

            // The contract is created as a new one in Nethereum, the abi and contractAddress must be specified.
			_tokenContract = _web3.Eth.GetContract(_abi, _contractAddress);
            // Basically, this contract can display balance of an account, transfer ERC20 token from one account 
            // to another.

            Debug.Assert(_tokenContract != null);
        }

        public async Task<BigInteger> GetBalance(string addr)
        {
            return await _tokenContract.GetFunction("balanceOf").CallAsync<BigInteger>(addr);
        }

        public async Task UnlockAcct(string addr, string password)
        {
            Debug.Assert(await _web3.Personal.UnlockAccount.SendRequestAsync(addr, password, new HexBigInteger(300)));
        }

        public async Task Transfer(string receiver, BigInteger amount, string sender)
        {
            var func = _tokenContract.GetFunction("transfer");
            var estimatedGas = await func.EstimateGasAsync(sender, new HexBigInteger(0), new HexBigInteger(0), receiver, amount);
            //var input = func.createTransactionInput(sender, estimatedGas, 1000000, 0, addr, amount);
            var hash = await func.SendTransactionAsync(sender, estimatedGas, new HexBigInteger(1000000), new HexBigInteger(0),
                receiver, amount);

            Debug.Log("Transaction Hash: " + hash);
        }

        public async void Approve(string receiver, BigInteger amount, string sender)
        {
            var func = _tokenContract.GetFunction("approve");
            //var estimatedGas = await func.EstimateGasAsync(addr, amount);
            //var input = func.createTransactionInput(sender, estimatedGas, 1000000, 0, addr, amount);
            var hash = await func.SendTransactionAsync(sender, new HexBigInteger(0), new HexBigInteger(1000000), new HexBigInteger(0), receiver, amount.ToString());

            Debug.Log("Transaction Hash: " + hash);
        }

        public async void ApproveAndCall(string spender, BigInteger amount, string extraData, string sender)
        {
            var func = _tokenContract.GetFunction("approveAndCall");
            //var estimatedGas = await func.EstimateGasAsync(addr, amount);
            //var input = func.createTransactionInput(sender, estimatedGas, 1000000, 0, addr, amount);
            var hash = await func.SendTransactionAsync(sender, new HexBigInteger(0), new HexBigInteger(1000000), new HexBigInteger(0), spender, amount.ToString(), extraData);

            Debug.Log("Transaction Hash: " + hash);
        }

        public async void TransferFrom(string fromaddr, string toaddr, BigInteger amount, string sender)
        {
            var func = _tokenContract.GetFunction("transferFrom");
            //var estimatedGas = await func.EstimateGasAsync(addr, amount);
            //var input = func.createTransactionInput(sender, estimatedGas, 1000000, 0, addr, amount);
            var hash = await func.SendTransactionAsync(sender, new HexBigInteger(0), new HexBigInteger(1000000), new HexBigInteger(0), fromaddr, toaddr, amount.ToString());

            Debug.Log("Transaction Hash: " + hash);
        }

        public async void TransferOwnership(string newOwner, string sender)
        {
            var func = _tokenContract.GetFunction("transferOwnership");
            //var estimatedGas = await func.EstimateGasAsync(addr, amount);
            //var input = func.createTransactionInput(sender, estimatedGas, 1000000, 0, addr, amount);
            var hash = await func.SendTransactionAsync(sender, new HexBigInteger(0), new HexBigInteger(1000000), new HexBigInteger(0), newOwner);

            Debug.Log("Transaction Hash: " + hash);
        }

        public async void FreezeAccount(string targetAddress, bool freeze, string sender)
        {
            var func = _tokenContract.GetFunction("freezeAccount");
            //var estimatedGas = await func.EstimateGasAsync(addr, amount);
            //var input = func.createTransactionInput(sender, estimatedGas, 1000000, 0, addr, amount);
            var hash = await func.SendTransactionAsync(sender, new HexBigInteger(0), new HexBigInteger(1000000), new HexBigInteger(0), targetAddress, freeze);
            Debug.Log("Transaction Hash: " + hash);
        }			

    }

}