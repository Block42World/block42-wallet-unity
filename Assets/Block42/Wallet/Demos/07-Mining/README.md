![Block42](http://assets.block42.world/images/icons/block42_logo_200.png)

# Block42 Unity Wallet Demo 7 - Mining
This demo continues from Demo 6 and uses [**Nethereum.Geth**](../../../../Plugins/Nethereum.Geth) to interact with the running node for mining. Isn't that great that you can actually mine in game, while it's not affecting the gaming experience too much? (No fake mining simulation in some mobile blockchain games)

## Demo Scene
Play [**Mining.unity**](Mining.unity) scene and click start mining, the script will check the latest block and update the ETH balance if the latest block is mined by this wallet.
![Screenshot](/Documents/Demo-07-Mining/02_screenshot.png)

## Scripts Overview
3 keys that should be noted in [MiningDemo.cs](MiningDemo.cs):

- Uses `Web3Geth.Miner.Start` and `Web3Geth.Miner.Stop` to start and stop the miner, they are `System.Task` running asynnc from main thread.
- Uses `EthBlockNumberUnityRequest` to get the latest block number.
- Uses `EthGetBlockWithTransactionsByNumberUnityRequest` to get the block info, which check if the block miner matches the player's wallet and display successfully mined.

### In-Game Mining Difficulity ###
Althought **geht** works great as a workaround for in-game mining, there's few difficulties/security isses that should be noted.

**geth** uses CPU to mine, while in Ethereum all miners are using GPU mining, which is 10+ times faster than CPU mining. Mining with CPU in mainnet may causes you years for one successful block even you have a very good PC.

If you want to do in-game mining with **geth** in private chain like us, and don't want any GPU miners (such as [ethminer](https://github.com/ethereum-mining/ethminer)) to mine your chain and greatly break the mining function in game, there's few ideas we are studying and may implement in soon future:
- Change the source code of **geth** and only able to mine with a hard-coded password:
This may stop GPU ming (before they hack and decompile your geth), but still cannot stop players extracting the **geth** from your Unity package and do CPU-mine without the opening the game.

- Change the cource code of **geth** and only able to mine with user-specific API key:
By using a centralized account server, each player is given an API key. Each time **geth** start mining, it requires the API key and check if the corresponding player is logged in. The cons of this are that advanced players(hackers) can still simulate the login behavour to the server, and being too centralized of course. De-compiling **geth** can also completedly bypass this check too.