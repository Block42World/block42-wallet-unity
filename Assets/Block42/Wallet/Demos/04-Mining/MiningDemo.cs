using System.Threading.Tasks;
using System.Collections;
using Nethereum.Geth;
using UnityEngine;
using UnityEngine.UI;
using Nethereum.JsonRpc.UnityClient;

namespace Block42
{

	/*
	 *  This demo simple start/stop mining on local or a chain which runing Geth
	 */
	public class MiningDemo : MyWalletEthBalanceDemo
	{

		[SerializeField] protected Text _statusText, _buttonText;
		[SerializeField] protected bool _mineInBackground;

		private bool _isMining = false;

		public void OnMiningClick()
		{
			if (_isMining)
			{
				_buttonText.text = "Start";
				StopMiner();
			}
			else
			{
				_buttonText.text = "Stop";
				StartMiner();
			}
			_isMining = !_isMining;
		}

		private async Task StartMiner()
		{
			Web3Geth web3Geth = new Web3Geth(WalletSettings.current.networkUrl);
			var result = await web3Geth.Miner.Start.SendRequestAsync();
			Debug.LogFormat("MiningDemo:StartMiner - result={0}", result);
			_statusText.text = "Mining started.";

			// Start watching for new block
			StartCoroutine(CheckNewBlock());
		}

		private async Task StopMiner()
		{
			Web3Geth web3Geth = new Web3Geth(WalletSettings.current.networkUrl);
			var result = await web3Geth.Miner.Stop.SendRequestAsync();
			Debug.LogFormat("MiningDemo:StopMiner - result={0}", result);
			_statusText.text = "Mining stopped.";

			// Stop watching for new block
			StopAllCoroutines();
		}

		private IEnumerator CheckNewBlock()
		{
			Debug.Log("MiningDemo:CheckNewBlock()");
			System.Numerics.BigInteger lastCheckBlockNumber = 0;
			var wait = new WaitForSeconds(5); // Check every 5 seconds
			while (true) {
				var blockNumberRequest = new EthBlockNumberUnityRequest(WalletSettings.current.networkUrl);
				yield return blockNumberRequest.SendRequest();
				if (blockNumberRequest.Exception == null) {
					var blockNumber = blockNumberRequest.Result.Value;
					if (lastCheckBlockNumber == 0)
					{
						lastCheckBlockNumber = blockNumber;
					}
					else if (lastCheckBlockNumber != blockNumber)
					{
						lastCheckBlockNumber = blockNumber;
						Debug.LogFormat("MiningDemo:CheckNewBlock - New block detected, blockNumber={0}", blockNumber);
						var getBlockWithNumberRequest = new EthGetBlockWithTransactionsByNumberUnityRequest(WalletSettings.current.networkUrl);
						yield return getBlockWithNumberRequest.SendRequest(new Nethereum.Hex.HexTypes.HexBigInteger(lastCheckBlockNumber));
						if (getBlockWithNumberRequest.Exception == null)
						{
							if (getBlockWithNumberRequest.Result.Miner.ToLower() == WalletManager.CurrentWalletAddress.ToLower()) {
								UpdateBalance();
								_statusText.text = string.Format("Successfully mined! Block number: {0}.", blockNumber);
							}
						}
					}
				}
				yield return wait;
			}
		}

		// Make sure miner stop on pause
		private void OnApplicationPause(bool pause)
		{
			if (!_mineInBackground && pause && _isMining)
				OnMiningClick();
		}

		// Make sure miner stop on quit
		private void OnApplicationQuit()
		{
			if (_isMining)
				OnMiningClick();
		}

	}

}