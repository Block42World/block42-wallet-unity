using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using Nethereum.Geth;
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

		[SerializeField] protected Text _statusText, _buttonText;
		[SerializeField] protected bool _mineInBackground;

		private Process _gethProcess;
		private bool _isMining = false;

		protected override void Start()
		{
			StopGeth();// Stop any geth just in case
			StartGeth();
			base.Start();
		}

		private void StartGeth()
		{
			UnityEngine.Debug.Log("Starting Geth...");

			string projectPath = Application.dataPath.Replace("/Assets", "");

			_gethProcess = new Process();
			_gethProcess.StartInfo.FileName = "geth";

			_gethProcess.StartInfo.Arguments = string.Format(
				"--datadir {0} --identity \"Client_{1}\" --port 30303 --rpc --rpcport 8142 " +
				"--rpccorsdomain \"*\" --rpcapi \"db,eth,net,personal,admin,miner,web3\" " +
				"--networkid 8100442 --gasprice \"1000000000\" " +
				"--etherbase \"{2}\" --nodiscover",
				System.IO.Path.Combine(projectPath, "chaindata"),
				SystemInfo.deviceUniqueIdentifier,
				WalletManager.CurrentWalletAddress
			);

			_gethProcess.StartInfo.UseShellExecute = true;
			_gethProcess.StartInfo.CreateNoWindow = true;
			_gethProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

			_gethProcess.Start();

			UnityEngine.Debug.LogFormat("Geth should be started: {0} {1}", _gethProcess.MainModule.FileName, _gethProcess.StartInfo.Arguments);
		}

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
				_statusText.text = string.Empty;
				StartMiner();
			}
			_isMining = !_isMining;
		}

		private async Task StartMiner()
		{
			Web3Geth web3Geth = new Web3Geth(WalletSettings.current.networkUrl);
			var task = web3Geth.Miner.Start.SendRequestAsync();
			if (await Task.WhenAny(task, Task.Delay(500)) == task) // 5s timeout
			{
				UnityEngine.Debug.LogFormat("MiningDemo:StartMiner - result={0}", task.Result);
				_statusText.text = "Mining started.";

				// Start watching for new block
				StartCoroutine(CheckNewBlock());
			}
			else
			{
				UnityEngine.Debug.LogError("MiningDemo:StartMiner - Timeout, mining start failed.");
				_statusText.text = "<color=red>Mining cannot start, please check if geth is running with the chosen network.</color>";
				OnMiningClick();
			}
		}

		private async Task StopMiner()
		{
			Web3Geth web3Geth = new Web3Geth(WalletSettings.current.networkUrl);
			var result = await web3Geth.Miner.Stop.SendRequestAsync();
			UnityEngine.Debug.LogFormat("MiningDemo:StopMiner - result={0}", result);
			_statusText.text = "Mining stopped.";

			// Stop watching for new block
			StopAllCoroutines();
		}

		private IEnumerator CheckNewBlock()
		{
			UnityEngine.Debug.Log("MiningDemo:CheckNewBlock()");
			System.Numerics.BigInteger lastCheckBlockNumber = 0;
			var wait = new WaitForSeconds(5); // Check every 5 seconds
			while (true)
			{

				var blockNumberRequest = new EthBlockNumberUnityRequest(WalletSettings.current.networkUrl);
				yield return blockNumberRequest.SendRequest();

				if (blockNumberRequest.Exception == null)
				{
					var blockNumber = blockNumberRequest.Result.Value;
					if (lastCheckBlockNumber == 0)
					{
						lastCheckBlockNumber = blockNumber;
					}
					else if (lastCheckBlockNumber != blockNumber)
					{

						lastCheckBlockNumber = blockNumber;
						_statusText.text = string.Format("New block detected, blockNumber={0}.", blockNumber);

						var getBlockByNumberRequest = new EthGetBlockWithTransactionsByNumberUnityRequest(WalletSettings.current.networkUrl);
						yield return getBlockByNumberRequest.SendRequest(new Nethereum.Hex.HexTypes.HexBigInteger(lastCheckBlockNumber));

						if (getBlockByNumberRequest.Exception == null)
						{
							UnityEngine.Debug.LogFormat("MiningDemo:CheckNewBlock - miner={0}", getBlockByNumberRequest.Result.Miner);
							if (getBlockByNumberRequest.Result.Miner.ToLower() == WalletManager.CurrentWalletAddress.ToLower())
							{
								UpdateBalance();
								_statusText.text = string.Format("Successfully mined! Block number: {0}.", blockNumber);
							}
						}
						else
						{
							throw new System.InvalidOperationException("Block number request failed, exception=" + getBlockByNumberRequest.Exception);
						}

					}
				}
				else
				{
					throw new System.InvalidOperationException("Block number request failed, exception=" + blockNumberRequest.Exception);
				}

				yield return wait;

			}
		}

		private void StopGeth()
		{
			if (_gethProcess != null)
			{
				UnityEngine.Debug.Log("Terminating Geth...");
				_gethProcess.CloseMainWindow();
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
			StopGeth();
		}

	}

}