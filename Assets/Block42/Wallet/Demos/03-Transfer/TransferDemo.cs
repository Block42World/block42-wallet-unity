using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

namespace Block42
{

	public class TransferDemo : MyWalletBalanceDemo
	{

		[SerializeField] private InputField _ethToAdressInputField, _ethToAmountInputField;
		[SerializeField] private InputField _tokenToAdressInputField, _tokenToAmountInputField;
		[SerializeField] private Text _ethToStatusText, _tokenToStatusText;

		private string _ethToResult;

		public void OnEthTransferClick()
		{
			WalletManager.Transfer(_ethToAdressInputField.text, decimal.Parse(_ethToAmountInputField.text), (result) =>
			{
				Start(); // Update the balance
				_ethToResult = result; // Cache it for opening Etherscan
				_ethToStatusText.text = "Transaction submitted, hash=" + result;



			}, (exception) =>
			{
				_ethToStatusText.text = exception.Message;
			});
		}

		public void OnEthResultClick()
		{
			if (!string.IsNullOrEmpty(_ethToResult))
				Application.OpenURL(WalletSettings.current.networkEtherscanUrl + "tx/" + _ethToStatusText);
		}

	}

}