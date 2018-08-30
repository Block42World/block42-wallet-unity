using UnityEngine;
using UnityEngine.UI;

namespace Block42
{
	public class MyWalletBalanceDemo : MyWalletEthBalanceDemo
	{

		[SerializeField] protected Text _balanceTokenText;

		protected override void Start()
		{
			base.Start();

			// Token balance of current wallet
			CubikContractController.Instance.BalanceOf((balance) =>
			{
                // When the callback is called, print out the balance on UI text
                Debug.LogFormat("MyWalletBalanceDemo:Start - CubikContractController.Instance.BalanceOf() returned, balance={0}", balance);
                _balanceTokenText.text = balance.ToString();
			});
		}

	}

}