![Block42](http://assets.block42.world/images/icons/block42_logo_200.png)

# Block42 Unity Wallet Demo 4 - My Wallet Balance
This demo utilizes [**WalletManager.cs**](../../Scripts/WalletManager.cs) and [**ERC20TokenContractController**](../../Scripts/Contracts/ERC20TokenContractController.cs) to obtain current player wallet, then get the ETH balance and token balance of the wallet.

## Demo Scene
Open [**MyWalletBalanceDemo.unity**](MyWalletBalanceDemo.unity) scene, DemoScripts GameObject has 2 components:
![Settings](/Documents/Demo-04-MyWalletBalance/01_demo_scripts.png)

- [**MyWalletBalanceDemo.cs**](MyWalletBalanceDemo.cs): Contains the demo logic and UI display
- [**CubikContractController.cs**](../../Scripts/Contracts/CubikContractController.cs): Cubik is an ERC20 token used in all demos and Block42 game. This script is just a wrapper of [**ERC20TokenContractController.cs**](../../Scripts/Contracts/ERC20TokenContractController.cs), plus providing an instance for easy access. You can have your own ERC20 token and inherent **ERC20TokenContractController.cs**. Contract's ABI, address and decimals have to be entered here.

Play the scene and you can see the balance of current wallet. Stop the game, send some ETH and token to the address in MetaMask or Mist, play the game again to spot the balance change:
![Screenshot](/Documents/Demo-04-MyWalletBalance/02_screenshot.png)

## Scripts Overview
In [MyWalletBalanceDemo.cs](MyWalletBalanceDemo.cs), it is doing 3 tasks that should be noted:

- Wallet Create: If there no wallet was created before, call `WalletManager.CreateWallet()` with wallet name and password to create one:
```
if (WalletManager.CurrentWallet == null)
{
    WalletManager.CreateWallet("TestWallet", "password");
}
```

- Get ETH Balance: Call `WalletManager.GetBalance()` to return the balance of current wallet.

- Get Token Balance: Call `CubikContractController.Instance.BalanceOf()` (or if you prefer using referenced object, call `referencedContract.BalanceOf()`), it maps to the `balanceOf()` method of the deployed ERC20 contract. The contract returns `BigInteger`, but it's converted into decimal as output in **ERC20TokenContractController.cs**, so developer can be care-free about all the unit conversion.