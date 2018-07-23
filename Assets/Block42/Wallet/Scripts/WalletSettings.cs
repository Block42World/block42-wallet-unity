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
                        return "https://ropsten.infura.io/93Pkd62SaFUrBJZC646Ah";
                    case EthereumNetwork.Kovan:
                        return "https://kovan.infura.io/93Pkd62SaFUrBJZC646A";
                    case EthereumNetwork.Rinkeby:
                        return "https://rinkeby.infura.io/93Pkd62SaFUrBJZC646Ah";
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

    }

}