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
			GethManager.StartGeth();

			yield return new WaitForSeconds(1);

			// Now can get the network status
			_networkStatusDemo.enabled = true;
		}

	}

}