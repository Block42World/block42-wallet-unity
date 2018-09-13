using UnityEngine;
using UnityEngine.UI;

namespace Block42
{
	public class MyWalletEthBalanceDemo : MyWalletDemo
	{

		[SerializeField] protected Text _walletAddressText, _balanceEthText;

		protected override void Start()
		{
			base.Start();

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

	}

}