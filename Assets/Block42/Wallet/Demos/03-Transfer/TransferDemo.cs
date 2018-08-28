using Nethereum.Hex.HexTypes;
using UnityEngine;
using UnityEngine.UI;

namespace Block42
{

	public class TransferDemo : MyWalletBalanceDemo
	{

		[SerializeField] private InputField _ethToAdressInputField, _ethToAmountInputField;
		[SerializeField] private InputField _tokenToAdressInputField, _tokenToAmountInputField;
		[SerializeField] private Text _ethToStatusText, _tokenToStatusText;

		private string _ethToResult, _tokenToResult;

		public void OnEthTransferClick()
		{
			Debug.LogFormat("TransferDemo:OnEthTransferClick - Start transfering {0} ETH.", decimal.Parse(_ethToAmountInputField.text));
			WalletManager.Transfer(_ethToAdressInputField.text, decimal.Parse(_ethToAmountInputField.text), (result) =>
			{
				_ethToResult = result; // Cache it for opening Etherscan
				_ethToStatusText.text = "Transaction submitted, waiting to be complete. Hash=" + result; // Display UI
				Debug.Log("TransferDemo:OnEthTransferClick - Transaction submitted, waiting to be complete.");
			}, () =>
			{
				Start(); // Update balances
				_ethToResult = string.Empty;
				_ethToStatusText.text = "Transaction succeed!";
				Debug.Log("TransferDemo:OnEthTransferClick - Transaction succeed!");
			}, (exception) =>
			{
				_ethToStatusText.text = string.Format("Error: {0}", exception.Message);
				Debug.LogErrorFormat("TransferDemo:OnEthTransferClick - Error: {0}", exception.Message);
			});
		}

		public void OnEthResultClick()
		{
			if (!string.IsNullOrEmpty(_ethToResult))
				Application.OpenURL(WalletSettings.current.networkEtherscanUrl + "tx/" + _ethToResult);
		}

		public void OnTokenTransferClick()
		{
			Debug.LogFormat("TransferDemo:OnEthTransferClick - Start transfering {0} Cubik.", decimal.Parse(_tokenToAmountInputField.text));
			CubikContractController.Instance.Transfer(_tokenToAdressInputField.text, decimal.Parse(_tokenToAmountInputField.text),  (result) =>
			{
				_tokenToResult = result; // Cache it for opening Etherscan
				_tokenToStatusText.text = "Transaction submitted, hash=" + result;
				Debug.Log("TransferDemo:OnTokenTransferClick - Transaction submitted, waiting to be complete.");
			}, () =>
			{
				Start(); // Update balances
				_tokenToResult = string.Empty;
				_tokenToStatusText.text = "Transaction succeed!";
				Debug.Log("TransferDemo:OnTokenTransferClick - Transaction succeed!");
			}, (exception) =>
			{
				_tokenToStatusText.text = exception.Message;
				Debug.LogErrorFormat("TransferDemo:OnTokenTransferClick - Error: {0}", exception.Message);
			});
		}

		public void OnTokenResultClick()
		{
			if (!string.IsNullOrEmpty(_ethToResult))
				Application.OpenURL(WalletSettings.current.networkEtherscanUrl + "tx/" + _tokenToResult);
		}

	}

}