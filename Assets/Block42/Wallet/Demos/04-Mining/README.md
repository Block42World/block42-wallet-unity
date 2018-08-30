![Block42](http://assets.block42.world/images/icons/block42_logo_200.png)

# Block42 Unity Wallet Demo 4 - Mining
This demo utilizes [**Nethereum.Geth**](../../../../Plugins/Nethereum.Geth) and the running Geth to start mining a Ethereum chain, and display the mining result.

## Demo Scene
Open [**Mining.unity**](Mining.unity) scene, DemoScripts GameObject has only 1 components:
![Settings](/Documents/Demo-02-MyWalletBalance/01_demo_scripts.png)

- [**MiningDemo.cs**](MiningDemo.cs): Contains the demo logic and UI display


Run a geth in local first. Then play the scene and click start mining, the script will check the latest block and update the ETH balance if the latest block is mined by this.
![Screenshot](/Documents/Demo-02-MyWalletBalance/02_screenshot.png)

## Scripts Overview
Open [MiningDemo.cs](MiningDemo.cs), it is doing 2 tasks that should be noted:

- Uses `Web3Geth.Miner.Start` and `Web3Geth.Miner.Stop` to start and stop the miner, they are `System.Task` running asynnc from main thread.

- Uses `EthBlockNumberUnityRequest` to get the latest block number.

- Uses `EthGetBlockWithTransactionsByNumberUnityRequest` to get the block info, which check if the block minere matches the player's wallet and display successfully mined.