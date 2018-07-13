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
        //public string tokenAbi;
        //public string tokenContractAddress = "0xb4fddd37602b03fa086c42bfa7b9739be38682c3";
        //public string tokenOwnerAddress = "0x6e62b9d357f65b774c15cf3f571310af246bbe1f";

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
    }

}