using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using UnityEngine;
using UnityEngine.Events;

namespace Block42
{

	[System.Serializable]
	public class WalletData
	{
		public string name;
		public string address;
		public string encryptedJson;
		public string privateKey;

		public WalletData(string name, string address, string encryptedJson, string privateKey)
		{
			this.name = name;
			this.address = address;
			this.encryptedJson = encryptedJson;
			this.privateKey = privateKey;
		}

		public override string ToString()
		{
			return string.Format("[WalletData - name={0}, address={1}, encryptedJson={2}, privateKey={3}", name, address, encryptedJson, privateKey);
		}
	}

	public static class WalletManager
	{

		#region Variables

		private const string FILE_NAME = "wallet.data";

		private static List<WalletData> walletList = new List<WalletData>();
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

		private static void LoadWallets()
		{
			if (File.Exists(_filePath))
			{
				var bf = new BinaryFormatter();
				var fs = File.Open(_filePath, FileMode.Open);
				walletList = (List<WalletData>)bf.Deserialize(fs);
				fs.Close();
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
			// Uses Nethereum.Signer to generate a new secret key
			var ecKey = Nethereum.Signer.EthECKey.GenerateKey();

			// After creating the secret key, we can get the public address and the private key with
			// ecKey.GetPublicAddress() and ecKey.GetPrivateKeyAsBytes()
			// (so it return it as bytes to be encrypted)
			var address = ecKey.GetPublicAddress();
			var privateKeyBytes = ecKey.GetPrivateKeyAsBytes();
			var privateKey = ecKey.GetPrivateKey();

			// Then we define a new KeyStore service
			var keystoreservice = new Nethereum.KeyStore.KeyStoreService();

			// And we can proceed to define encryptedJson with EncryptAndGenerateDefaultKeyStoreAsJson(),
			// and send it the password, the private key and the address to be encrypted.
			var encryptedJson = keystoreservice.EncryptAndGenerateDefaultKeyStoreAsJson(password, privateKeyBytes, address);

			WalletData wallet = new WalletData(accountName, address, encryptedJson, privateKey);
			walletList.Add(wallet);

			SaveWallets();

			_currentWalletIndex = walletList.Count - 1;

			walletCreated?.Invoke();
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
			// Then we call the method SendRequest() from the getBalanceRequest we created with the address and the newest created teblock.
			yield return balanceRequest.SendRequest(address, BlockParameter.CreateLatest());

			// Now we check if the request has an exception
			if (balanceRequest.Exception == null)
			{
				// We define balance and assign the value that the getBalanceRequest gave us.
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

		public static void Transfer(string addressTo, decimal amount, UnityAction<string> onSucceed = null, UnityAction<System.Exception> onFailed = null)
		{
			CoroutineManager.Start(TransferCoroutine(addressTo, amount, onSucceed, onFailed));
		}

		// Send transfer request using TransactionSignedUnityRequest
		private static IEnumerator TransferCoroutine(string addressTo, decimal amount, UnityAction<string> onSucceed, UnityAction<System.Exception> onFailed)
		{
			// Create a request
			var transactionSignedRequest = new TransactionSignedUnityRequest(WalletSettings.current.networkUrl, CurrentWalletPrivateKey, CurrentWalletAddress);

			// Create the input, if no gas and gas price are used, it uses the default value
			var transactionInput = new TransactionInput(null, addressTo, new HexBigInteger(UnitConversion.Convert.ToWei(amount, 18)));

			yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);

			if (transactionSignedRequest.Exception == null)
			{
				onSucceed?.Invoke(transactionSignedRequest.Result);
			}
			else
			{
				onFailed?.Invoke(transactionSignedRequest.Exception);
			}
		}

		public static void CheckTransaction(string transactionHash, UnityAction<decimal> onSucceed = null, UnityAction<System.Exception> onFailed = null)
		{
			CoroutineManager.Start(CheckTransactionCoroutine(transactionHash, onSucceed, onFailed));
		}

		// Send balance request using EthGetGalanceUnityRequest
		private static IEnumerator CheckTransactionCoroutine(string transactionHash, UnityAction<decimal> onSucceed, UnityAction<System.Exception> onFailed)
		{
			// Use EthGetBalanceUnityRequest from the Nethereum lib to send balance request
			var newPendingTransactionRequest = new EthNewPendingTransactionFilterUnityRequest(WalletSettings.current.networkUrl);
			// Then we call the method SendRequest() from the getBalanceRequest we created with the address and the newest created teblock.
			yield return newPendingTransactionRequest.SendRequest();

			// Now we check if the request has an exception
			if (newPendingTransactionRequest.Exception == null)
			{
				// We define balance and assign the value that the getBalanceRequest gave us.
				var result = newPendingTransactionRequest.Result;
				Debug.Log("CheckTransactionCoroutine|" + result);
				//// Finally we execute the callback and we use the Netherum.Util.UnitConversion
				//// to convert the balance from WEI to ETHER (that has 18 decimal places)
				//onSucceed?.Invoke(UnitConversion.Convert.FromWei(balance, 18));
			}
			else
			{
				// If there was an error we just call the onFailed callback
				onFailed?.Invoke(newPendingTransactionRequest.Exception);
			}
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

		#endregion

#if UNITY_EDITOR

		[UnityEditor.MenuItem("Block42/Wallet/Remove All Wallets")]
		private static void RemoveAllSavedWallets()
		{
			if (UnityEditor.EditorUtility.DisplayDialog("Remove Wallets", "Are you sure to remove all saved wallets? This cannot be reverted.", "Yes", "Cancel"))
			{
				File.Delete(_filePath);
				if (Application.isPlaying)
				{ // Reload scene
					UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
				}
			}
		}

#endif

	}

}