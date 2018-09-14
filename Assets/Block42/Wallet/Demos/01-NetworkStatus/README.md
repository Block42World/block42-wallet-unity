![Block42](http://assets.block42.world/images/icons/block42_logo_200.png)

# Block42 Unity Wallet Demo 1 - Network Status
The first demo shows the chain network status, such as block number, miner, etc.

## Demo Scene
Play [NetworkStatusDemo.unity](NetworkStatusDemo.unity) scene, the info of the blockchain network will be retrived and printed out:
![Screenshot](/Documents/Demo-01-NetworkStatus/01_screenshot.png)

## Scripts Overview
In [NetworkStatusDemo.cs](NetworkStatusDemo.cs), it uses EthXxxxRequest in **WalletManager** from Nethereum library to interact with Ethereum:

- **`EthBlockNumberUnityRequest`**: Gets the total block number in the chaiin.
- **`EthGasPriceUnityRequest`**: Gets the current gas price in the chain.
- **`EthGetBlockWithTransactionsByNumberUnityRequest`**: Gets the block info given a block number in the chain.

Notes that EthXxxxRequest is a coroutine and has to run using `StartCoroutine()`. More use-cases and explanations for different EthXxxxRequest will be cover in next demos.