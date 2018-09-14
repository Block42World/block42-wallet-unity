![Block42](http://assets.block42.world/images/icons/block42_logo_200.png)

# Block42 Unity Wallet Demo 6 - Run Node
This demo executes an external executable [geth](https://github.com/ethereum/go-ethereum) to run an ethereum node.

## Ethereum Setup
In this example, we will run an ethereum node, so that we later can do mining, syncing and more on any public or private Ethereum chain.

For demo purpose, we don't want to sync and download all the blocks in any Ethereum public chain, so we just use a private chain. To interact with a private chain, as mentioned in intro, we set the Network to **Custom** and set the Custom Network Url to **http://localhost:8142** (pick a port of your choice):
![Settings](/Documents/Demo-06-RunNode/01_wallet_setting.png)

Now, we need the [go-ethereum](https://github.com/ethereum/go-ethereum) cli client **geth**, read how to [install](https://github.com/ethereum/go-ethereum/wiki/Installing-Geth) it if you havn't done yet.

### Why Using External Executable Geth? ###
There isn't any Unity or even C# library/client that can run Ethereum node, the closest is the [C++](https://ethereum.stackexchange.com/a/279/39970). Therefore, a workaround is to running an external executable from Unity using `System.Process`, then use Netehereum library to interact with it, just like in testnet or mainnet.
In theory, Ethereum node client should be able to run in any language, as long as it follows the consensus and mining algorium, but that required a full understanding of the full source code of the Geth, so that it can be re-written into C# for Unity. We will keep that in mind and wish to do it sometime soon in the future.

A geth client is chain data is already included, but you can setup your own chain in following steps:

Install *geth* and put the executable inside [*StreamingAssets*](https://docs.unity3d.com/Manual/StreamingAssets.html) folder, so it can be run in both Editor and build.
initialize the chaindata folder with commands:
```
cd <path-to-project-StreamingAssets>
rm -rf chaindata # remove the chain data that connect to our Block42 chain
mkdir chaindata
geth --datadir chaindata init genesis.json
```
- *chaindata* is the folder that contain all block data, you can rename it if you like.
- *genesis.json* is the json file that contains initializing info of the chain, write your own one from [here](https://github.com/ethereum/go-ethereum/wiki/Private-network), basically put a new network id that's not currently used.


## Demo Scene
Open [**RunNodeDemo.unity**](RunNodeDemo.unity) scene and it contain [**RunNodeDemo.cs**](MiningDemo.cs) and **NetworkStatusDemo.cs** from Demo 1.

Basically the script starts the *geth* with `new Process()`, and set the arguents using:
```
_gethProcess.StartInfo.Arguments = string.Format(
    "--datadir {0} --identity \"Client_{1}\" --port {2} --rpc --rpcport {3} " +
    "--rpccorsdomain \"*\" --rpcapi \"db,eth,net,personal,admin,miner,web3\" " +
    "--networkid {4} --gasprice \"{5}\" " +
    "--etherbase \"{6}\" --nodiscover --ipcdisable",
    System.IO.Path.Combine(Application.streamingAssetsPath, GethSettings.current.datadir),
    SystemInfo.deviceUniqueIdentifier,
    GethSettings.current.port,
    GethSettings.current.rpcport,
    GethSettings.current.networkId,
    Nethereum.Util.UnitConversion.Convert.ToWei(WalletSettings.current.gasPrice, 9), // GWEI
    WalletManager.CurrentWalletAddress
);
```
Few keys should be noted:
- *datadir*: set to *chaindata* folder we created earlier
- *identity*: set the device id just for easier spotting
- *networkid*: your choice of network id of the private chain, has to match the one in **genesis.json**
- *etherbase*: set to player's wallet address where the mining rewards gives to (skip if you not doing mining)
Use `geth help` to check what other options do. 

These options can be set in **Block42 > Wallet > Geth Settins** menu:
![Settings](/Documents/Demo-06-RunNode/03_geth_settings.png)

Play the scene now and you can see geth is run at background, the chain status is read from the running geth node and displayed. You can check if geth is running by checking:
```
ps -A | grep geth
```