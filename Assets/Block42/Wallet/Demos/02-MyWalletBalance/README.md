![Block42](http://assets.block42.world/images/icons/block42_logo_200.png)

# Block42 Unity Wallet Demo 2 - My Wallet Balance
This demo utilizes [**WalletManager.cs**](../../Scripts/WalletManager.cs) to obtain current player wallet, then get the ETH balance and token balance of the wallet.

## Demo Scene
Open [**MyWalletBalanceDemo.unity**](MyWalletBalanceDemo.unity) scene, DemoScripts GameObject has 2 components:
![Settings](/Documents/Demo-02-MyWalletBalance/01_settings.png)

- [**MyWalletBalanceDemo.cs**](MyWalletBalanceDemo.cs): Contains the demo logic and UI display
- [**CubikContractController.cs**](../../Scripts/Contracts/CubikContractController.cs): Cubik is a ERC20 token used in Block42. This script is just a wrapper of [**ERC20TokenContractController.cs**](../../Scripts/Contracts/ERC20TokenContractController.cs) plus providing an instance for easy access. You can have your own ERC20 token and inherent **ERC20TokenContractController.cs**. Contract's ABI, address and decimals have to be entered here.


Now play the scene and you can see your own wallet with balance, try send some ETH and token to the address and see the change:
![Screenshot](/Documents/Demo-02-MyWalletBalance/02_screenshot.png)

## Scripts Overview
Open [MyWalletBalanceDemo.cs](MyWalletBalanceDemo.cs), it is doing 3 task that should be noted:

- Wallet Create: **WalletManager.cs** saved all wallets locally. If there no wallet was created before, you have to call `WalletManager.CreateWallet()` with wallet name and password to create one, a better way with UI will be covered later.
```
if (WalletManager.CurrentWallet == null)
{
    WalletManager.CreateWallet("TestWallet", "password");
}
```

- Get ETH Balance: Simply call `WalletManager.GetBalance()` to return the balance of current wallet. It's the same to [__Demo 1__](../01-StaticWalletBalance) but code is wrapped in **WalletManager.cs**.

- Get Token Balance: Call `CubikContractController.Instance.BalanceOf()` (or if you prefer using referenced object, call `referencedContract.BalanceOf()`), it maps to the `balanceOf()` method of the deployed ERC20 contract. The contract returns `BigInteger`, but it's converted into decimal as output in **ERC20TokenContractController.cs**, so developer can be care-free about all the unit conversion.