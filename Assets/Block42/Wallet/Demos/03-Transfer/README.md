![Block42](http://assets.block42.world/images/icons/block42_logo_200.png)

# Block42 Unity Wallet Demo 3 - Transfer
This demo continues to utilize [**WalletManager.cs**](../../Scripts/WalletManager.cs) and [**ERC20TokenContractController**](../../Scripts/Contracts/ERC20TokenContractController.cs) to send ETH and token to a particular address, with hint showing transaction status.

## Demo Scene
Open [**TransferDemo.unity**](TransferDemo.unity) scene, similar to Demo2, DemoScripts GameObject has 2 components:
![Settings](/Documents/Demo-02-MyWalletBalance/01_demo_scripts.png)

- [**TransferDemo.cs**](TransferDemo.cs): Contains the demo logic and UI display
- [**CubikContractController.cs**](../../Scripts/Contracts/CubikContractController.cs): Cubik is a ERC20 token used in Block42. This script is just a wrapper of [**ERC20TokenContractController.cs**](../../Scripts/Contracts/ERC20TokenContractController.cs) plus providing an instance for easy access. You can have your own ERC20 token and inherent **ERC20TokenContractController.cs**. Contract's ABI, address and decimals have to be entered here.


Play the scene and you can send ETH and token to a particular address. After submitting the transation, a transaction has is displayed, clicking that will open it in EtherScan. Once the transation completd, the status become "completed" and the balance is refreshed.
![Screenshot](/Documents/Demo-02-MyWalletBalance/02_screenshot.png)

## Scripts Overview
Open [TransferDemo.cs](TransferDemo.cs), it is doing 2 tasks that should be noted:

- Transfer ETH Balance: Call `WalletManager.Transfer()` with addressTo and weiAmount.

- Get Token Balance: Call `CubikContractController.Instance.Transfer()` with adressTo and weiAmount, it maps to the `transfer()` method of the deployed ERC20 contract.