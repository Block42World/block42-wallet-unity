using UnityEngine;
using UnityEngine.UI;

namespace Block42
{
	public class MyWalletEthBalanceDemo : MonoBehaviour
	{

		[SerializeField] protected Text _walletAddressText, _balanceEthText;

		protected virtual void Start()
		{
			if (WalletManager.CurrentWallet == null)
			{
				Debug.Log("MyWalletBalanceDemo:Start - No wallet available, create one now.");
				WalletManager.CreateWallet("TestWallet", "password");
			}

			// Address of current wallet
			_walletAddressText.text = WalletManager.CurrentWalletAddress;

			// Get ETH balance of current wallet
			UpdateBalance();
		}

		protected void UpdateBalance()
		{
			WalletManager.GetBalance((balance) =>
			{
				// When the callback is called, print out the balance on UI text
				Debug.LogFormat("MyWalletBalanceDemo:Start - WalletManager.GetBalance() returned, balance={0}", balance);
				_balanceEthText.text = balance.ToString();
			});
		}

		public void OnWalletLinkClick()
		{
			WalletManager.OpenEtherscanAddress();
		}

		public void OnWalletAddressClick()
		{
			CopyToClipboard(_walletAddressText.text);
		}

		protected void CopyToClipboard(string s)
		{
			TextEditor te = new TextEditor();
			te.text = s;
			te.SelectAll();
			te.Copy();
			Debug.Log("Copied to clipboard: \n" + s);
		}

	}

}