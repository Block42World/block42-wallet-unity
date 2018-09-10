![Block42](http://assets.block42.world/images/icons/block42_logo_200.png)

# Block42 Unity Wallet Demo 5 - Mining
This demo continue from Demo 4 and uses [**Nethereum.Geth**](../../../../Plugins/Nethereum.Geth) to interact with the running node for mining.

## Demo Scene
Open [**Mining.unity**](Mining.unity) scene, DemoScripts GameObject has only [**MiningDemo.cs**](MiningDemo.cs):
![Settings](/Documents/Demo-05-Mining/01_demo_scripts.png)

Then play the scene and click start mining, the script will check the latest block and update the ETH balance if the latest block is mined by this.
![Screenshot](/Documents/Demo-05-Mining/02_screenshot.png)

## Scripts Overview
3 keys that should be noted in [MiningDemo.cs](MiningDemo.cs):

- Uses `Web3Geth.Miner.Start` and `Web3Geth.Miner.Stop` to start and stop the miner, they are `System.Task` running asynnc from main thread.
- Uses `EthBlockNumberUnityRequest` to get the latest block number.
- Uses `EthGetBlockWithTransactionsByNumberUnityRequest` to get the block info, which check if the block minere matches the player's wallet and display successfully mined.