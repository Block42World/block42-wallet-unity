![Block42](http://assets.block42.world/images/icons/block42_logo_200.png)

# Block42 Wallet Unity Project

## About
A Ethereum wallet and transaction system in Unity. This plug-in is inspired by Alto's [unit3d-blockchain-wallet](https://github.com/alto-io/unity3d-blockchain-wallet) and aim to be scalable to be used for all other projects with minimal coding. Changes includes separating UI logic from manager logic, optimize the code to follow Unity standard, as well as adding a number of demos learning purposes.

Please note that this is still in prototype and under heavy development. The final product may be subject to a number of quality assurance tests to verify conformance with specifications.

## Requirements
- Unity 2017+
- Set Scripting Runtime Version to .NET 4.x Equivalent in PlayerSettings
![PlayerSettings](Documents/00_playersettings.png)

## Settings
ScriptableObject is used globally for wallet setting, assets the settings in menu through Block42 > Wallet > Settings
![Menu Wallet Settings](Documents/01_menu_wallet_settings.png)

Inspector will show:
![Wallet Settings Inspector](Documents/02_wallet_settings_inspector.png)

Ropsten is set as default and used throughout all demos, you can use different testnet or custom network and provide the URL in settings.

## Demos
This projects contains a number of demos from basic to intermediate, go to [Demos](Assets/Block42/Wallet/Demos) folder for more details.


## Notes
- [Nethereum](https://github.com/Nethereum/Nethereum) is used as the .NET integration library for Ethereum here. If you already have that in your project, you can safely delete [Plugins](Assets/Block42/Plugins) folder. 
- [Nethereum.Web3](Assets/Block42/Plugins/Neteherum/Nethereum.Web3.dll) is used to demonstrate the difference of using Web3 and UnityRPCRequests in Demo [01-StaticWalletBalance](Assets/Block42/Wallet/Demos/01-StaticWalletBalance). If you are not using any Web3 library in your project, you can delete both to save building space.