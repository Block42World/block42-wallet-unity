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

- Wallet Create: If there no wallet was created before, call `WalletManager.CreateWallet()` 'with wallet name and password to to make sure there's one:
```
if (WalletManager.CurrentWallet == null)
{
    WalletManager.CreateWallet("TestWallet", "password");
}
```

- Get ETH Balance: Call `WalletManager.GetBalance()` to return the balance of current wallet, using `EthGetBalanceUnityRequest()`:
```
// Use EthGetBalanceUnityRequest from the Nethereum lib to send balance request
var balanceRequest = new EthGetBalanceUnityRequest(WalletSettings.current.networkUrl);
// Then we call the method SendRequest() from the getBalanceRequest we created with the address and the newest created block
yield return balanceRequest.SendRequest(address, BlockParameter.CreateLatest());

// Now we check if the request has an exception
if (balanceRequest.Exception == null)
{
    // We define balance and assign the value that the getBalanceRequest gave us
    var balance = balanceRequest.Result.Value;
    // Finally we execute the callback and we use the Netherum.Util.UnitConversion
    // to convert the balance from WEI to ETHER (that has 18 decimal places)
    onSucceed?.Invoke(UnitConversion.Convert.FromWei(balance, 18));
}
else
{
    // If there was an error we just call the onFailed callback
    onFailed?.Invoke(balanceRequest.Exception);
}
```

- Get Token Balance: Call `CubikContractController.Instance.BalanceOf()` (or if you prefer using referenced object, call `referencedContract.BalanceOf()`), it maps to the `balanceOf()` method of the deployed ERC20 contract. The contract returns `BigInteger`, but it's converted into decimal as output in **ERC20TokenContractController.cs**, so developer can be care-free about all the unit conversion.
All contract calls are using `GetFunction()`, `CreateCallInput()` then `EthCallUnityRequest()` with the function name:
```
protected Function GetFunction(string name)
{
    return _contract.GetFunction(name);
}

protected CallInput CreateCallInput(string name, params object[] functionInput)
{
    return GetFunction(name)?.CreateCallInput(functionInput);
}


protected IEnumerator CallCoroutine(CallInput functionInput, UnityAction<string> onSuccess = null, UnityAction<System.Exception> onFailed = null)
{
    var request = new EthCallUnityRequest(WalletSettings.current.networkUrl);
    yield return request.SendRequest(functionInput, BlockParameter.CreateLatest());
    if (request.Exception == null)
        onSuccess?.Invoke(request.Result);
    else
        onFailed?.Invoke(request.Exception);
}
```

These are wrapped into **ContractController** already and you can simply use `CallCoroutine("functionName", input, callback)`, e.g. getting token balance is:
```
public void BalanceOf(string address, UnityAction<decimal> onSucceed, UnityAction<System.Exception> onFailed = null)
{
    StartCoroutine(CallCoroutine("balanceOf", address, (result) => {
        var balance = DecodeVariable<BigInteger>("balanceOf", result);
        onSucceed?.Invoke(UnitConversion.Convert.FromWei(balance, _decimals));
    }, (exception) => {
        onFailed?.Invoke(exception);
    }));
}
```