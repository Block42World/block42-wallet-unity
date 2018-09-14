![Block42](http://assets.block42.world/images/icons/block42_logo_200.png)

# Block42 Unity Wallet Demo 5 - Transfer
This demo continues to utilize [**WalletManager.cs**](../../Scripts/WalletManager.cs) and [**ERC20TokenContractController**](../../Scripts/Contracts/ERC20TokenContractController.cs) to send ETH and token to a another wallet, with hint showing transaction status.

## Demo Scene
Open [**TransferDemo.unity**](TransferDemo.unity) scene, similar to Demo 4, DemoScripts GameObject has 2 components:
![Settings](/Documents/Demo-05-Transfer/01_demo_scripts.png)

Play the scene and you can send ETH and token to a particular address. After submitting the transation, a transaction has is displayed, clicking that will open it in EtherScan. Once the transation completd, the status become "completed" and the balance is refreshed.
![Screenshot](/Documents/Demo-05-Transfer/02_screenshot.png)

## Scripts Overview
Open [TransferDemo.cs](TransferDemo.cs), it is doing 2 tasks that should be noted:

- Transfer ETH Balance: Call `WalletManager.Transfer()` with addressTo and weiAmount, which using `Nethereum.JsonRpc.UnityClient.TransactionSignedUnityRequest` with the request input created using `new Nethereum.RPC.Eth.DTOs.TransactionInput()`.

- Get Token Balance: Call `CubikContractController.Instance.Transfer()` with adressTo and weiAmount, it maps to the `transfer()` method of the deployed ERC20 contract, which also using `Nethereum.JsonRpc.UnityClient.TransactionSignedUnityRequest` but the input is created using `Nethereum.Contracts.CreateTransactionInput()`.