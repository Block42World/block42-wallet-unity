using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
            // Get balance with EthGetBalanceUnityRequest in WalletManager, using coroutine
			WalletManager.GetBalance(_address, (balance) =>
            {
                // When the callback is called, print out the balance on UI text
				Debug.LogFormat("StaticWalletBalance:Start - EthGetBalanceUnityRequest returned, balance={0}", balance);
                _balanceUnityRequestText.text = balance.ToString();
            });

            // Get balance with Web3, using Task and await
            var t = GetBalanceByWeb3(_address, (balance) => // Put result as var t just to avoid warning in VisualStudio
            {
                // When the callback is called, print out the balance on UI text
                Debug.LogFormat("StaticWalletBalance:Start - Web3 returned, balance={0}", balance);
                _balanceWeb3Text.text = balance.ToString();
            });
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