![Block42](http://assets.block42.world/images/icons/block42_logo_200.png)

# Block42 Unity Wallet Demo 4 - Run Node
This demo execute an external executeable [geth](https://github.com/ethereum/go-ethereum) to run an ethereum node.

## Ethereum Setup
In this example, we will run ethereum node, so that we can later do mining, syncing and more on any public or private Ethereum chain.

For demo purpose, we don't want to sync and download all the blocks in any Ethereum public chain, so we just use a private chain. To interact with a private chain, as mentioned before, we set the Network to **Custom** and set the Custom Network Url to **http://localhost:8142** (pick a port of your choice):
![Settings](/Documents/Demo-04-RunNode/01_setting.png)

Now, we need the [go-ethereum](https://github.com/ethereum/go-ethereum) cli client *geth*, read how to [install](https://github.com/ethereum/go-ethereum/wiki/Installing-Geth) it if you havn't yet.

### Why Geth ###
There isn't any Unity or even c# library/client that can run Ethereum node, the closest is the C++. Therefore, a workaround is to running an external executable from Unity using `System.Process`.

After install *geth*, put the executable just beside *Assets* folder, initialize the chaindata folder with command:
```
mkdir chaindata
geth --datadir chaindata init genesis.json
```
*chaindata* is the folder that contain all block data, you can rename it if you like.
*genesis.json* is the json file that contains info of the chain, copy and read more from [here](https://github.com/ethereum/go-ethereum/wiki/Private-network).


## Demo Scene
Open [**RunNodeDemo.unity**](RunNodeDemo.unity) scene and it contain [**MiningDemo.cs**](MiningDemo.cs) and **NetworkStatusDemo.cs** from Demo 1.

Basically the script start the geth with `new Process()`, and set the arguents using:
```
_gethProcess.StartInfo.Arguments = string.Format(
    "--datadir {0} --identity \"Client_{1}\" --port 30303 --rpc --rpcport 8142 " +
    "--rpccorsdomain \"*\" --rpcapi \"db,eth,net,personal,admin,miner,web3\" " +
    "--networkid 8100442 --gasprice \"1000000000\" " +
    "--etherbase \"{2}\" --nodiscover",
    System.IO.Path.Combine(projectPath, "chaindata"),
    SystemInfo.deviceUniqueIdentifier,
    WalletManager.CurrentWalletAddress
);
```
Few things should be noted here:
- datadir: set to *chaindata* folder we created earlier
- identity: seting the device id just for easier spotting
- networkid: your choice of network id of the private chain, has to match the one in **genesis.json**
- etherbase: set to player's wallet address where the mining rewards gives to (skip if you not doing mining)

Run the scene now and you can see geth is run at background, the chain status is read from the running geth node. You can check geth is running by checking:
```
ps -A | grep geth
```

You can then also why peering geth to your online public chain, or any of the testnet.