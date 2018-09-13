using System.Collections;
using System.Threading.Tasks;
using Nethereum.Geth;
using Nethereum.JsonRpc.UnityClient;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Block42
{

	/*
	 *  This demo simple start/stop mining on local or a chain which runing Geth
	 */
	public class MiningDemo : MyWalletEthBalanceDemo
	{

		[SerializeField] protected Text _blockNumberText, _statusText, _buttonText;
		[SerializeField] protected bool _mineInBackground;

		private bool _isMining = false;
		private int _lastCheckedBlockNumber = -1;
		private Queue _miningStatusQueue = new Queue();

		protected override void Start()
		{
			CreateWalletIfNotExists();
			GethManager.StartGeth();
			base.Start();
			UpdateBlockNumber();
		}

		private void UpdateBlockNumber()
		{
			StartCoroutine(GetBlockNumber((blockNumber) =>
			{
				_blockNumberText.text = blockNumber.ToString();
				_lastCheckedBlockNumber = blockNumber;
			}));
		}

		private IEnumerator GetBlockNumber(UnityAction<int> callback)
		{
			// Use EthBlockNumberUnityRequest from the Nethereum lib to send block number request
			var blockNumberRequest = new EthBlockNumberUnityRequest(WalletSettings.current.networkUrl);
			yield return blockNumberRequest.SendRequest();

			if (blockNumberRequest.Exception == null)
				callback((int)blockNumberRequest.Result.Value);
			else
				throw new System.InvalidOperationException("Block number request failed, exception=" + blockNumberRequest.Exception);
		}

		public void OnMiningClick()
		{
			if (_isMining)
			{
				_buttonText.text = "Start";
				var t = GethManager.StopMiner(() => {
					QueueStatus("Mining stopped.");
					StopAllCoroutines();
				});
			}
			else
			{
				_buttonText.text = "Stop";
				//StartMiner();
				var t = GethManager.StartMiner(() => {
					QueueStatus("Mining started.");
					StartCoroutine(CheckNewBlock());
				}, () => {
					QueueStatus("<color=red>Mining cannot start, please check if geth is running with the chosen network.</color>");
					OnMiningClick();
				});
			}
			_isMining = !_isMining;
		}

		private IEnumerator CheckNewBlock()
		{
			UnityEngine.Debug.Log("MiningDemo:CheckNewBlock()");

			var wait = new WaitForSeconds(5); // Check every 5 seconds
			while (true)
			{

				var blockNumberRequest = new EthBlockNumberUnityRequest(WalletSettings.current.networkUrl);
				yield return blockNumberRequest.SendRequest();

				if (blockNumberRequest.Exception == null)
				{
					var blockNumber = (int)blockNumberRequest.Result.Value;

					if (blockNumber > _lastCheckedBlockNumber)
					{
						if (blockNumber - _lastCheckedBlockNumber < 50) // If more than 50 blocks behind, just jump to the latest block
						{
							for (int i = _lastCheckedBlockNumber + 1; i <= blockNumber; i++)
							{
								StartCoroutine(CheckBlock(i));
								_blockNumberText.text = i.ToString();
								yield return new WaitForSeconds(1);
							}
						}
						_lastCheckedBlockNumber = blockNumber;
					}
				}
				else
				{
					throw new System.InvalidOperationException("Block number request failed, exception=" + blockNumberRequest.Exception);
				}

				yield return wait;

			}
		}

		private IEnumerator CheckBlock(int blockNumber)
		{
			QueueStatus(string.Format("New block #{0} detected.", blockNumber));

			var getBlockByNumberRequest = new EthGetBlockWithTransactionsByNumberUnityRequest(WalletSettings.current.networkUrl);
			yield return getBlockByNumberRequest.SendRequest(new Nethereum.Hex.HexTypes.HexBigInteger(blockNumber));

			if (getBlockByNumberRequest.Exception == null)
			{
				UnityEngine.Debug.LogFormat("MiningDemo:CheckBlock({0}) - miner={1}", blockNumber, getBlockByNumberRequest.Result.Miner);
				if (getBlockByNumberRequest.Result.Miner.ToLower() == WalletManager.CurrentWalletAddress.ToLower())
				{
					UpdateBalance();
					QueueStatus(string.Format("<color=blue>Successfully mined block #{0}!</color>", blockNumber));
				}
			}
			else
			{
				throw new System.InvalidOperationException("Block number request failed, exception=" + getBlockByNumberRequest.Exception);
			}
		}

		private void QueueStatus(string status)
		{
			while (_miningStatusQueue.Count > 5)
				_miningStatusQueue.Dequeue();
			_miningStatusQueue.Enqueue(string.Format("[{0}] {1}", System.DateTime.Now.ToString("T"), status));
			_statusText.text = string.Empty;
			foreach (var o in _miningStatusQueue)
				_statusText.text += o.ToString() + "\n";
		}

	}

}