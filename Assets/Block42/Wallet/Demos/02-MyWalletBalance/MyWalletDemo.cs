using UnityEngine;
using UnityEngine.UI;

namespace Block42
{
	public class MyWalletDemo : MonoBehaviour
	{

		protected virtual void Start()
		{
			CreateWalletIfNotExists();
		}

		protected void CreateWalletIfNotExists()
		{
			if (WalletManager.CurrentWallet == null)
			{
				Debug.Log("MyWalletBalanceDemo:Start - No wallet available, create one now.");
				WalletManager.CreateWallet("TestWallet", "password");
			}
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