using UnityEngine;
using UnityEngine.UI;

namespace Block42
{
	public class MyWalletDemo : MonoBehaviour
	{

		[SerializeField] private bool _createWalletIfNotExists = true;
		[SerializeField] protected Text _walletAddressText;

		protected virtual void Start()
		{
			if (_createWalletIfNotExists)
				CreateWalletIfNotExists();
			DisplayCurrentWalletAddress();
		}

		protected void CreateWalletIfNotExists()
		{
			if (WalletManager.CurrentWallet == null)
			{
				Debug.Log("MyWalletBalanceDemo:Start - No wallet available, create one now.");
				WalletManager.CreateWallet("TestWallet", "password");
			}
		}

		protected void DisplayCurrentWalletAddress()
		{
			if (_walletAddressText != null)
				_walletAddressText.text = WalletManager.CurrentWalletAddress;
		}

		public void OnWalletLinkClick()
		{
			WalletManager.OpenEtherscanAddress();
		}

		public void OnWalletAddressClick()
		{
			WalletManager.CopyToClipboard(WalletManager.CurrentWalletAddress);
		}

	}

}