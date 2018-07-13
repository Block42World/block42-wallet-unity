using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;
using Nethereum.JsonRpc.UnityClient;

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

		private static string FilePath => System.IO.Path.Combine(Application.persistentDataPath, FILE_NAME);

		public static WalletData CurrentWallet => walletList.Count > _currentWalletIndex ? walletList[_currentWalletIndex] : null;

		public static string CurrentWalletAddress => CurrentWallet?.address;

		public static string CurrentWalletPrivateKey => CurrentWallet?.privateKey;

		#endregion

		#region Methods

		static WalletManager()
		{
			LoadWallets();
		}

		// Wallet save and load

		private static void LoadWallets()
		{
			if (File.Exists(FilePath)) {
				var bf = new BinaryFormatter();
				var fs = File.Open(FilePath, FileMode.Open);
				walletList = (List<WalletData>)bf.Deserialize(fs);
				fs.Close();
			}
		}

		private static void SaveWallets()
		{
			var bf = new BinaryFormatter();
			var fs = File.Create(FilePath);
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

        // Balance

        public static void GetBalance(UnityAction<decimal> callback)
        {
            GetBalance(CurrentWalletAddress, callback);
        }

        public static void GetBalance(string address, UnityAction<decimal> callback)
        {
            CoroutineManager.Start(GetBalanceCoroutine(CurrentWalletAddress, callback));
        }

        // Send balance request using EthGetGalanceUnityRequest, note that it uses coroutine
        private static IEnumerator GetBalanceCoroutine(string address, UnityAction<decimal> callback)
        {
            // Use EthGetBalanceUnityRequest from the Nethereum lib to send balance request
            var balanceRequest = new EthGetBalanceUnityRequest(WalletSettings.current.networkUrl);
            // Then we call the method SendRequest() from the getBalanceRequest we created with the address and the newest created teblock.
            yield return balanceRequest.SendRequest(address, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

            // Now we check if the request has an exception
            if (balanceRequest.Exception == null)
            {
                // We define balance and assign the value that the getBalanceRequest gave us.
                var balance = balanceRequest.Result.Value;
                // Finally we execute the callback and we use the Netherum.Util.UnitConversion
                // to convert the balance from WEI to ETHER (that has 18 decimal places)
                callback(Nethereum.Util.UnitConversion.Convert.FromWei(balance, 18));
            }
            else
            {
                // If there was an error we just throw an exception.
                throw new System.InvalidOperationException("Get balance request failed, exception=" + balanceRequest.Exception);
            }
        }

        #endregion

    }

}