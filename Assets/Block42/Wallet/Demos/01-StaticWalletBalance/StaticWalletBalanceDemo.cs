﻿using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Nethereum.JsonRpc.UnityClient;

namespace Block42
{
    public class StaticWalletBalanceDemo : MonoBehaviour
    {

        [SerializeField] protected string _address = "0x6e62b9d357f65b774c15cf3f571310af246bbe1f";
        [SerializeField] protected Text _walletAddressText, _balanceUnityRequestText, _balanceWeb3Text;

        void Start()
        {
            // Address
            _walletAddressText.text = _address;

            // Get balance with EthGetGalanceUnityRequest, using coroutine
            StartCoroutine(GetBalanceCoroutine(_address, (balance) =>
            {
                // When the callback is called, print out the balance on UI text
                Debug.LogFormat("StaticWalletBalance:Start - EthGetGalanceUnityRequest returned, balance={0}", balance);
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
            // Create a Web3 object using Nethereum lib
            var web3 = new Nethereum.Web3.Web3(WalletSettings.current.networkUrl);
            // Use GetBalance request 
            var balance = await web3.Eth.GetBalance.SendRequestAsync(address);
            callback(Nethereum.Util.UnitConversion.Convert.FromWei(balance, 18));
        }

    }

}