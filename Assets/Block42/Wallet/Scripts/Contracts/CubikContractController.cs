using UnityEngine;

namespace Block42
{
	// A abstract base class that interact with Ethereum contract, all contracts should inherent from this calss.
	public class CubikContractController : ERC20TokenContractController
	{

		public static CubikContractController Instance { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			Instance = this;
		}

	}

}