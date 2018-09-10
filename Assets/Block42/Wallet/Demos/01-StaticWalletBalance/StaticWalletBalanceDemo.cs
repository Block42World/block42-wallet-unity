using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Nethereum.JsonRpc.UnityClient;

namespace Block42
{
    public class StaticWalletBalanceDemo : MonoBehaviour
    {

		[SerializeField] protected string _address = "0x7eaec37c6baf7110de72a8c5b908fa518089676a";
        [SerializeField] protected Text _walletAddressText, _balanceUnityRequestText, _balanceWeb3Text;

		private void Awake()
		{
			// Address
			_walletAddressText.text = _address;
		}

		private void Start()
        {
            // Get balance with EthGetBalanceUnityRequest, using coroutine
            StartCoroutine(GetBalanceCoroutine(_address, (balance) =>
            {
                // When the callback is called, print out the balance on UI text
				Debug.LogFormat("StaticWalletBalance:Start - EthGetBalanceUnityRequest returned, balance={0}", balance);
                _balanceUnityRequestText.text = balance.ToString();
            }));

            // Get balance with Web3, using Task and await
            var t = GetBalanceByWeb3(_address, (balance) => // Put result as var t just to avoid warning in VisualStudio
            {
                // When the callback is called, print out the balance on UI text
                Debug.LogFormat("StaticWalletBalance:Start - Web3 returned, balance={0}", balance);
                _balanceWeb3Text.text = balance.ToString();
            });
        }

        // Send balance request using EthGetGalanceUnityRequest, note that it uses coroutine
        private IEnumerator GetBalanceCoroutine(string address, UnityAction<decimal> callback)
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

        // Send balance request using Web3, note that Web3 uses Task and await
		private async Task GetBalanceByWeb3(string address, UnityAction<decimal> callback)
        {
			Debug.Log("StaticWalletBalance:GetBalanceByWeb3()");
            // Create a Web3 object using Nethereum lib
            var web3 = new Nethereum.Web3.Web3(WalletSettings.current.networkUrl);
            // Use GetBalance request 
            var balance = await web3.Eth.GetBalance.SendRequestAsync(address);
			Debug.Log("StaticWalletBalance:GetBalanceByWeb3 - balance returned");


			var web3geth = new Nethereum.Geth.Web3Geth(WalletSettings.current.networkUrl);
			var result = await web3geth.Miner.Start.SendRequestAsync();
			Debug.Log(result);

            callback(Nethereum.Util.UnitConversion.Convert.FromWei(balance, 18));
        }

		public void OnWalletLinkClick()
		{
			WalletManager.OpenEtherscanAddress(_address);
		}

		public void OnWalletAddressClick()
		{
			WalletManager.CopyToClipboard(_address);
		}

    }

}