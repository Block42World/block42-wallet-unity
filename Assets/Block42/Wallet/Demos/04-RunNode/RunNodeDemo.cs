using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace Block42
{

	/*
	 *  This demo shows how to run a node by executing geth externally.
	 */
	public class RunNodeDemo : MonoBehaviour
	{

		[SerializeField] private NetworkStatusDemo _networkStatusDemo;
		private Process _gethProcess;

		private IEnumerator Start()
		{
			StopGeth();// Stop any geth just in case
			StartGeth();

			yield return new WaitForSeconds(1);

			// Now can get the network status
			_networkStatusDemo.enabled = true;
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

		private void StopGeth()
		{
			if (_gethProcess != null)
			{
				UnityEngine.Debug.Log("Terminating Geth...");
				_gethProcess.CloseMainWindow();
			}
		}

		private void OnApplicationQuit()
		{
			StopGeth();
		}

		public void OnStopGethClick()
		{
			StopGeth();
		}

	}

}