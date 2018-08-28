using UnityEngine;

namespace Block42
{

    public class WalletSettings : ScriptableObjectSingleton<WalletSettings>
    {

        public enum EthereumNetwork {
            Local,
            Mainnet,
            Ropsten,
            Kovan,
            Rinkeby,
            Custom
        }

		public EthereumNetwork network = EthereumNetwork.Ropsten;
        public string customNetworkUrl = "http://nsgpwp.oqi.io:9549";

        public string networkUrl {
            get {
                switch (network) {
                    case EthereumNetwork.Local:
                        return "http://localhost:8545";
                    case EthereumNetwork.Mainnet:
                        return "https://api.myetherapi.com/eth";
                    case EthereumNetwork.Ropsten:
						return "https://ropsten.infura.io/v3/6469dd6b6c614a20ab3efb85cc1c7b1d";
                    case EthereumNetwork.Kovan:
						return "https://kovan.infura.io/v3/6469dd6b6c614a20ab3efb85cc1c7b1d";
                    case EthereumNetwork.Rinkeby:
						return "https://rinkeby.infura.io/v3/6469dd6b6c614a20ab3efb85cc1c7b1d";
                    default:
                        return customNetworkUrl;
                }
            }
        }

		// Etherescan url for debug
		public string networkEtherscanUrl {
			get {
				switch (network)
				{
					case EthereumNetwork.Ropsten:
						return "https://ropsten.etherscan.io/";
					case EthereumNetwork.Kovan:
						return "https://kovan.etherscan.io/";
					case EthereumNetwork.Rinkeby:
						return "https://rinkeby.etherscan.io/";
					default:
						return "https://etherscan.io/";
				}
			}
		}

		public int gasPrice = 1;

		public bool debugLog = false;

    }

}