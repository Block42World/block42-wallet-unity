using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Nethereum.Hex.HexTypes;
using Nethereum.KeyStore;
using Nethereum.Signer;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using UnityEngine;
using UnityEngine.Events;

namespace Block42
{

	public static class WalletManager
	{

		#region Variables

		private const string FILE_NAME = "wallet.data";

		public static List<WalletData> walletList = new List<WalletData>();
		private static int _currentWalletIndex = 0;

		#endregion

		#region Events

		public static event UnityAction walletCreated;

		#endregion

		#region Properties

		private static string _filePath => System.IO.Path.Combine(Application.persistentDataPath, FILE_NAME);

		public static WalletData CurrentWallet => walletList.Count > _currentWalletIndex ? walletList[_currentWalletIndex] : null;

		public static string CurrentWalletAddress => CurrentWallet?.address;

		public static string CurrentWalletPrivateKey => CurrentWallet?.privateKey;

		#endregion

		#region Wallet Management

		static WalletManager()
		{
			LoadWallets();
		}

		// Wallet save and load

		public static void LoadWallets() // Public for dmoe
		{
			if (File.Exists(_filePath))
			{
				var bf = new BinaryFormatter();
				var fs = File.Open(_filePath, FileMode.Open);
				walletList = (List<WalletData>)bf.Deserialize(fs);
				fs.Close();
			}
			else
			{
				walletList.Clear();
			}
		}

		private static void SaveWallets()
		{
			var bf = new BinaryFormatter();
			var fs = File.Create(_filePath);
			bf.Serialize(fs, walletList);
			fs.Close();
		}

		// Creates and encrypt a new account
		public static void CreateWallet(string accountName, string password)
		{
			// Create a new random key for public key and private key
			var ecKey = EthECKey.GenerateKey();

			var address = ecKey.GetPublicAddress();
			var privateKeyBytes = ecKey.GetPrivateKeyAsBytes();
			var privateKey = ecKey.GetPrivateKey();

			// Use key stroe service to generate a keystore
			var keystoreservice = new KeyStoreService();
			var encryptedJson = keystoreservice.EncryptAndGenerateDefaultKeyStoreAsJson(password, privateKeyBytes, address);

			// Created the wallet with given info
			AddWallet(accountName, ecKey.GetPublicAddress(), encryptedJson, ecKey.GetPrivateKey());
		}

		public static void ImportWallet(string accountName, string password, string encryptedJson)
		{
			// Use key store service to re-generate the key
			var service = new KeyStoreService();
			byte[] bytes = service.DecryptKeyStoreFromJson(password, encryptedJson);
			var ecKey = new EthECKey(bytes, true);

			// Created the wallet with given info
			AddWallet(accountName, ecKey.GetPublicAddress(), encryptedJson, ecKey.GetPrivateKey());
		}

		private static void AddWallet(string accountName, string address, string encryptedJson, string privateKey)
		{
			WalletData wallet = new WalletData(accountName, address, encryptedJson, privateKey);
			walletList.Add(wallet);

			SaveWallets();

			_currentWalletIndex = walletList.Count - 1; // Use this wallet as current
			walletCreated?.Invoke();
		}

		public static void SetCurrentWalletIndex(int index)
		{
			_currentWalletIndex = index;
		}

		#endregion

		#region Balance

		public static void GetBalance(UnityAction<decimal> onSucceed, UnityAction<System.Exception> onFailed = null)
		{
			GetBalance(CurrentWalletAddress, onSucceed);
		}

		public static void GetBalance(string address, UnityAction<decimal> onSucceed, UnityAction<System.Exception> onFailed = null)
		{
			CoroutineManager.Start(GetBalanceCoroutine(CurrentWalletAddress, onSucceed, onFailed));
		}

		// Send balance request using EthGetGalanceUnityRequest
		private static IEnumerator GetBalanceCoroutine(string address, UnityAction<decimal> onSucceed, UnityAction<System.Exception> onFailed)
		{
			// Use EthGetBalanceUnityRequest from the Nethereum lib to send balance request
			var balanceRequest = new EthGetBalanceUnityRequest(WalletSettings.current.networkUrl);
			// Then we call the method SendRequest() from the getBalanceRequest we created with the address and the newest created block
			yield return balanceRequest.SendRequest(address, BlockParameter.CreateLatest());

			// Now we check if the request has an exception
			if (balanceRequest.Exception == null)
			{
				// We define balance and assign the value that the getBalanceRequest gave us
				var balance = balanceRequest.Result.Value;
				// Finally we execute the callback and we use the Netherum.Util.UnitConversion
				// to convert the balance from WEI to ETHER (that has 18 decimal places)
				onSucceed?.Invoke(UnitConversion.Convert.FromWei(balance, 18));
			}
			else
			{
				// If there was an error we just call the onFailed callback
				onFailed?.Invoke(balanceRequest.Exception);
			}
		}

		#endregion

		#region Transfer

		public static void Transfer(string addressTo, decimal amount, UnityAction<string> onSubmitted = null, UnityAction onCompleted = null, UnityAction<System.Exception> onFailed = null)
		{
			CoroutineManager.Start(TransferCoroutine(addressTo, amount, onSubmitted, onCompleted, onFailed));
		}

		// Send transfer request using TransactionSignedUnityRequest
		private static IEnumerator TransferCoroutine(string addressTo, decimal amount, UnityAction<string> onSubmitted = null, UnityAction onCompleted = null, UnityAction<System.Exception> onFailed = null)
		{
			// Use TransactionSignedUnityRequest from the Nethereum lib to create a transaction request
			var transactionSignedRequest = new TransactionSignedUnityRequest(WalletSettings.current.networkUrl, CurrentWalletPrivateKey, CurrentWalletAddress);
			// Create the input, if no gas and gas price are used, it uses the default value
			var transactionInput = new TransactionInput(null, addressTo, new HexBigInteger(UnitConversion.Convert.ToWei(amount, 18)));
			// Then we call the method SignAndSendTransaction() from the transactionSignedRequest we created with the transaction input
			yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);

			// Now we check if the request has an exception
			if (transactionSignedRequest.Exception == null)
			{
				onSubmitted?.Invoke(transactionSignedRequest.Result);

				// Keep checking the transaction receipt if an onCompleted used
				if (onCompleted != null)
				{
					GetTransactionReceipt(transactionSignedRequest.Result, 5, (transactionReceipt) => // Retry for every 5 seconds
					{
						if (transactionReceipt != null)
							onCompleted();
					});
				}
			}
			else
			{
				onFailed?.Invoke(transactionSignedRequest.Exception);
			}
		}

		public static void GetTransactionReceipt(string transactionHash, int secondsResendIfNoResult = 5, UnityAction<TransactionReceipt> onSucceed = null, UnityAction<System.Exception> onFailed = null)
		{
			if (WalletSettings.current.debugLog)
				Debug.LogFormat("WalletManager:CheckTransaction({0})", transactionHash);
			CoroutineManager.Start(CheckTransactionCoroutine(transactionHash, secondsResendIfNoResult, onSucceed, onFailed));
		}

		private static IEnumerator CheckTransactionCoroutine(string transactionHash, int secondsResendIfNoResult = 5, UnityAction<TransactionReceipt> onSucceed = null, UnityAction<System.Exception> onFailed = null)
		{
			yield return new WaitForSeconds(secondsResendIfNoResult); // Wait few seconds for receipt to return

			// Use EthGetTransactionReceiptUnityRequest from the Nethereum lib to get transaction receipt
			var getTransactionReceiptRequest = new EthGetTransactionReceiptUnityRequest(WalletSettings.current.networkUrl);
			// Then we call the method SendRequest() from the getTransactionReceiptRequest we created with the transaction hash
			yield return getTransactionReceiptRequest.SendRequest(transactionHash);

			// Now we check if the request has an exception
			if (getTransactionReceiptRequest.Exception == null)
			{
				onSucceed?.Invoke(getTransactionReceiptRequest.Result);
				if (secondsResendIfNoResult > 0 && getTransactionReceiptRequest.Result == null)
				{
					if (WalletSettings.current.debugLog)
						Debug.LogFormat("WalletManager:CheckTransaction - No receipt returned yet, try again in {0} seconds...", secondsResendIfNoResult);
					GetTransactionReceipt(transactionHash, secondsResendIfNoResult, onSucceed, onFailed);
				}
			}
			else
			{
				// If there was an error we just call the onFailed callback
				onFailed?.Invoke(getTransactionReceiptRequest.Exception);
			}
		}

		#endregion

		#region Block Number

		public static void GetBlockNumber(UnityAction<int> callback)
		{
			CoroutineManager.Start(GetBlockNumberCoroutine(callback));
		}

		private static IEnumerator GetBlockNumberCoroutine(UnityAction<int> callback)
		{
			var blockNumberRequest = new EthBlockNumberUnityRequest(WalletSettings.current.networkUrl);
			yield return blockNumberRequest.SendRequest();

			if (blockNumberRequest.Exception == null)
			{
				callback((int)blockNumberRequest.Result.Value);
			}
			else
			{
				throw new System.InvalidOperationException("Block number request failed, exception=" + blockNumberRequest.Exception);
			}
		}

		#endregion

		#region Latest Block

		public static void GetLatestBlock(UnityAction<BlockWithTransactions> callback)
		{
			CoroutineManager.Start(GetLatestBlockCoroutine(callback));
		}

		private static IEnumerator GetLatestBlockCoroutine(UnityAction<BlockWithTransactions> callback)
		{
			var blockNumberRequest = new EthBlockNumberUnityRequest(WalletSettings.current.networkUrl);
			yield return blockNumberRequest.SendRequest();

			if (blockNumberRequest.Exception == null)
			{
				var getBlockByNumberRequest = new EthGetBlockWithTransactionsByNumberUnityRequest(WalletSettings.current.networkUrl);
				yield return getBlockByNumberRequest.SendRequest(new Nethereum.Hex.HexTypes.HexBigInteger(blockNumberRequest.Result.Value));

				if (getBlockByNumberRequest.Exception == null)
				{
					callback(getBlockByNumberRequest.Result);
				}
				else
				{
					throw new System.InvalidOperationException("Get block request failed, exception=" + getBlockByNumberRequest.Exception);
				}
			}
			else
			{
				throw new System.InvalidOperationException("Block number request failed, exception=" + blockNumberRequest.Exception);
			}
		}

		public static event UnityAction<int> BlockNumberUpdatedEvent;
		public static event UnityAction<BlockWithTransactions> LatestBlockUpdatedEvent;
		private static IEnumerator _watchNewBlockIenumerator;
		private static int _lastCheckedBlockNumber = -1;

		public static void WatchNewBlocks()
		{
			// Get the current block number first
			GetBlockNumber((blockNumber) =>
			{
				_lastCheckedBlockNumber = blockNumber;
			});
			_watchNewBlockIenumerator = WatchNewBlockCoroutine();
			CoroutineManager.Start(_watchNewBlockIenumerator);
		}

		public static void StopWatchNewBlocks()
		{
			CoroutineManager.Stop(_watchNewBlockIenumerator);
		}

		private static IEnumerator WatchNewBlockCoroutine()
		{
			// Get block number every 5 seconds
			var wait = new WaitForSeconds(5);
			while (true)
			{

				var blockNumberRequest = new EthBlockNumberUnityRequest(WalletSettings.current.networkUrl);
				yield return blockNumberRequest.SendRequest();

				if (blockNumberRequest.Exception == null)
				{
					int blockNumber = (int)blockNumberRequest.Result.Value;

					if (blockNumber > _lastCheckedBlockNumber)
					{
						if (WalletSettings.current.debugLog)
							Debug.LogFormat("WalletManager:WatchNewBlock - block number updated = {1}", blockNumber);
						
						if (blockNumber - _lastCheckedBlockNumber < 50) // If more than 50 blocks behind, just jump to the latest block
						{
							for (int i = _lastCheckedBlockNumber + 1; i <= blockNumber; i++)
							{
								BlockNumberUpdatedEvent?.Invoke(blockNumber);
								CoroutineManager.Start(GetLatestBlockCoroutine(i));
								yield return new WaitForSeconds(1);
							}
						}
						_lastCheckedBlockNumber = blockNumber;
					}
				}
				else
				{
					throw new System.InvalidOperationException("Block number request failed, exception=" + blockNumberRequest.Exception);
				}

				yield return wait;

			}
		}

		private static IEnumerator GetLatestBlockCoroutine(int blockNumber)
		{
			var getBlockByNumberRequest = new EthGetBlockWithTransactionsByNumberUnityRequest(WalletSettings.current.networkUrl);
			yield return getBlockByNumberRequest.SendRequest(new HexBigInteger(blockNumber));

			if (getBlockByNumberRequest.Exception == null)
			{
				if (WalletSettings.current.debugLog)
					Debug.LogFormat("WalletManager:GetLatestBlock - Latest block #{0}, miner={1}, hash={2}", blockNumber, getBlockByNumberRequest.Result.Miner, getBlockByNumberRequest.Result.BlockHash);
				LatestBlockUpdatedEvent?.Invoke(getBlockByNumberRequest.Result);
			}
			else
			{
				throw new System.InvalidOperationException("Block number request failed, exception=" + getBlockByNumberRequest.Exception);
			}
		}

		#endregion

		#region Gas Price

		public static void GetGasPrice(UnityAction<decimal> callback)
		{
			CoroutineManager.Start(GetGasPriceCoroutine(callback));
		}

		private static IEnumerator GetGasPriceCoroutine(UnityAction<decimal> callback)
		{
			var gasPriceRequest = new EthGasPriceUnityRequest(WalletSettings.current.networkUrl);
			yield return gasPriceRequest.SendRequest();

			if (gasPriceRequest.Exception == null)
			{
				callback(UnitConversion.Convert.FromWei(gasPriceRequest.Result.Value, 9));
			}
			else
			{
				throw new System.InvalidOperationException("Gas price request failed, exception=" + gasPriceRequest.Exception);
			}
		}

		#endregion

		#region Tools

		public static void CopyToClipboard(string s)
		{
			TextEditor te = new TextEditor
			{
				text = s
			};
			te.SelectAll();
			te.Copy();
			Debug.Log("WalletManager:CopyToClipboard - copied.");
		}

		#endregion

		#region Debug

		public static void OpenEtherscanAddress()
		{
			OpenEtherscanAddress(CurrentWalletAddress);
		}

		public static void OpenEtherscanAddress(string address)
		{
			Application.OpenURL(WalletSettings.current.networkEtherscanUrl + "address/" + address);
		}

		public static void DeleteWalletDataFile()
		{
			File.Delete(_filePath);
		}

		#endregion

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Block42/Wallet/Remove All Wallets")]
		private static void RemoveAllSavedWallets()
		{
			if (UnityEditor.EditorUtility.DisplayDialog("Remove Wallets", "Are you sure to remove all saved wallets? This cannot be reverted.", "Yes", "Cancel"))
			{
				DeleteWalletDataFile();
				if (Application.isPlaying)
				{ // Reload scene
					UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
				}
			}
		}

		[UnityEditor.MenuItem("Block42/Wallet/Open Local Wallet Data Folder")]
		private static void OpenLocalWalletDataFolder()
		{
			UnityEditor.EditorUtility.RevealInFinder(_filePath);
		}

		[UnityEditor.MenuItem("Block42/Wallet/Print First Wallet Address")]
		private static void PrintFirstWalletAddress()
		{
			if (CurrentWallet == null)
				LoadWallets();
			if (CurrentWallet == null)
				CreateWallet("Account", "Password");
			Debug.Log(CurrentWalletAddress);
			CopyToClipboard(CurrentWalletAddress);
		}

#endif

	}

}