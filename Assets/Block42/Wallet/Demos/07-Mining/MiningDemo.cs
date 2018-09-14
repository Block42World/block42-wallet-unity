using System.Collections;
using Nethereum.JsonRpc.UnityClient;
using UnityEngine;
using UnityEngine.UI;

namespace Block42
{

	/*
	 *  This demo simple start/stop mining on local or a chain which runing Geth
	 */
	public class MiningDemo : MyWalletEthBalanceDemo
	{

		[SerializeField] protected Text _blockNumberText, _statusText, _buttonText;

		private Queue _miningStatusQueue = new Queue();

		private void Awake()
		{
			WalletManager.BlockNumberUpdatedEvent += OnBlockNumberUpdated;
			WalletManager.LatestBlockUpdatedEvent += OnLatestBlockUpdated;
		}

		protected override void Start()
		{
			CreateWalletIfNotExists();
			GethManager.StartGeth();
			base.Start();
			UpdateBlockNumber();
		}

		private void UpdateBlockNumber()
		{
			WalletManager.GetBlockNumber((blockNumber) =>
			{
				_blockNumberText.text = blockNumber.ToString();
			});
		}

		public void OnMiningClick()
		{
			if (GethManager.isMining)
			{
				_buttonText.text = "Start";
				var t = GethManager.StopMiner(() => {
					QueueStatus("Mining stopped.");
					WalletManager.StopWatchNewBlocks();
				});
			}
			else
			{
				_buttonText.text = "Stop";
				var t = GethManager.StartMiner(() => {
					QueueStatus("Mining started.");
					WalletManager.WatchNewBlocks();
				}, () => {
					QueueStatus("<color=red>Mining cannot start, please check if geth is running with the chosen network.</color>");
					OnMiningClick();
				});
			}
		}

		// Callback when the block number updated
		private void OnBlockNumberUpdated(int blockNumber)
		{
			_blockNumberText.text = blockNumber.ToString();
			QueueStatus(string.Format("New block #{0} detected.", blockNumber));
		}

		// Callback when latest block updated
		private void OnLatestBlockUpdated(Nethereum.RPC.Eth.DTOs.BlockWithTransactions block)
		{
			if (block.Miner.ToLower() == WalletManager.CurrentWalletAddress.ToLower())
			{
				UpdateBalance();
				QueueStatus(string.Format("<color=blue>Successfully mined block #{0}!</color>", (int)block.Number.Value));
			}
		}

		// Helper function to queue and display the status with timestamps
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