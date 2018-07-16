using UnityEngine;

namespace Block42
{
	// A abstract base class that interact with Ethereum contract, all contracts should inherent from this calss.
	public class CubikContractController : ERC20TokenContractController
	{

		private static CubikContractController _instance;
		public static CubikContractController Instance {
			get {
				if (_instance == null)
					throw new System.Exception("CubikContractController instance is not added into the scene.");
				return _instance;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_instance = this;
		}

	}

}