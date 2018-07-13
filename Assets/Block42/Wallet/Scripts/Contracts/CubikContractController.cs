using System.Collections;
using System.Numerics;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Util;
using UnityEngine;
using UnityEngine.Events;

namespace Block42
{
	// A abstract base class that interact with Ethereum contract, all contracts should inherent from this calss.
	public class CubikContractController : ContractController
	{

		public static CubikContractController Instance { get; private set; }

		[SerializeField] protected int _decimals = 18;

		private void Awake()
		{
			Instance = this;
		}

		#region BalanceOf

		public void BalanceOf(UnityAction<decimal> onSucceed)
		{
            BalanceOf(WalletManager.CurrentWalletAddress, onSucceed);
		}

        public void BalanceOf(string address, UnityAction<decimal> onSucceed)
		{
            StartCoroutine(CallCoroutine("balanceOf", address, (result) => {
                var balance = DecodeVariable<BigInteger>("balanceOf", result);
                onSucceed?.Invoke(UnitConversion.Convert.FromWei(balance, _decimals));
            }));
		}

		#endregion

		#region Transfer

		// Call contract's Transfer() function using Web3
		//public async Task Transfer(string receiver, BigInteger amount)
		//{
		//    var func = _contract.GetFunction("transfer");
		//    var estimatedGas = await func.EstimateGasAsync(WalletManager.myWalletAddress, new HexBigInteger(0), new HexBigInteger(0), receiver, amount);
		//    //var input = func.createTransactionInput(sender, estimatedGas, 1000000, 0, addr, amount);
		//    var hash = await func.SendTransactionAsync(WalletManager.myWalletAddress, estimatedGas, new HexBigInteger(1000000), new HexBigInteger(0),
		//        receiver, amount);
		//    if (_debugLog)
		//        Debug.LogFormat("{0}:Transfer - Completed, hash={1}", GetType().BaseType, hash);
		//}

		// Call contract's Transfer() function and executes callbacks
		public IEnumerator Transfer(string addressTo, BigInteger transferAmount, UnityAction onStart = null, UnityAction<string> onSucceed = null, UnityAction<System.Exception> onFailed = null)
		{
			//var transactionInput = GetFunction("transfer").CreateTransactionInput(WalletManager.CurrentWalletAddress, new HexBigInteger(50000), new HexBigInteger(10), null, addressTo, transferAmount);
			var transactionInput = GetFunction("transfer").CreateTransactionInput(WalletManager.CurrentWalletAddress, addressTo, transferAmount);

			var transactionSignedRequest = new TransactionSignedUnityRequest(WalletSettings.current.networkUrl, WalletManager.CurrentWalletPrivateKey, WalletManager.CurrentWalletAddress);

			onStart?.Invoke();

			yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);

			if (transactionSignedRequest.Exception == null)
				onSucceed?.Invoke(transactionSignedRequest.Result);
			else
				onFailed?.Invoke(transactionSignedRequest.Exception);
		}

		#endregion

		#region Approve

		#endregion

	}

}