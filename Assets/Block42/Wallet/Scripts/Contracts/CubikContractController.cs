using UnityEngine;
using UnityEngine.Events;

namespace Block42
{
	// This is just a singleton wrapper of a ERC20TokenContractController, for easier access without reference.
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