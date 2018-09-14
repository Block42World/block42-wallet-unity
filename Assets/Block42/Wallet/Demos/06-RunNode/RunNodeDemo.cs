using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace Block42
{

	/*
	 *  This demo shows how to run a node by executing geth externally.
	 */
	public class RunNodeDemo : MyWalletDemo
	{

		protected override void Start()
		{
			base.Start();

			GethManager.StartGeth();

			// Wait 1s then get the network status
			Invoke("EnableNetworkStatusDemo", 1);
		}

		private void EnableNetworkStatusDemo()
		{
			GetComponent<NetworkStatusDemo>().enabled = true;
		}

		public void OnStopGethClick()
		{
			GethManager.StopGeth();
		}

	}

}