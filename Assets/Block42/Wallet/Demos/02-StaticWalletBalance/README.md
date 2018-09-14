![Block42](http://assets.block42.world/images/icons/block42_logo_200.png)

# Block42 Unity Wallet Demo 1 - Static Wallet Balance
The demo shows the ETH balance of a given wallet address, and aim to demonstrate two different ways archiving it by using Web3 or EthGetGalanceUnityRequest.

## Demo Scene
Open [StaticWallBalanceDemo.unity](StaticWalletBalanceDemo.unity) scene, a [StaticWallBalanceDemo.cs](StaticWalletBalanceDemo.cs) is attached at DemoScript:
![Settings](/Documents/Demo-02-StaticWalletBalance/01_settings.png)

Here you can put the wallet address you're interested in. Play the scene and you can see the ETH balance of it:
![Screenshot](/Documents/Demo-02-StaticWalletBalance/02_screenshot.png)

## Scripts Overview
Open [StaticWallBalanceDemo.cs](StaticWalletBalanceDemo.cs), here you can see two methods using `Web3` and `EthGetGalanceUnityRequest` to get the wallet's ETH balance respectively:

**EthGetGalanceUnityRequest**:
```
// Send balance request using EthGetGalanceUnityRequest, note that it uses coroutine
private IEnumerator GetBalanceCoroutine(string address, UnityAction<decimal> onSucceed, UnityAction<System.Exception> onFailed)
{
    var balanceRequest = new EthGetBalanceUnityRequest(WalletSettings.current.networkUrl);
    yield return balanceRequest.SendRequest(address, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

    if (balanceRequest.Exception == null)
    {
        var balance = balanceRequest.Result.Value;
        onSucceed?.Invoke(UnitConversion.Convert.FromWei(balance, 18));
    }
    else
    {
        onFailed?.Invoke(balanceRequest.Exception);
    }
}
```

**Web3**:
```
// Send balance request using Web3, note that Web3 uses Task and await
private async Task GetBalanceByWeb3(string address, UnityAction<decimal> callback)f
{
    var web3 = new Nethereum.Web3.Web3(WalletSettings.current.networkUrl);
    var balance = await web3.Eth.GetBalance.SendRequestAsync(address);
    callback(Nethereum.Util.UnitConversion.Convert.FromWei(balance, 18));
}
```

The most difference here is that `EthGetGalanceUnityRequest` and other [`UnityRPCRequests`](https://github.com/Nethereum/Nethereum/blob/master/src/Nethereum.Unity/UnityRPCRequests.cs) are using Unity's [`Coroutines`](https://docs.unity3d.com/Manual/Coroutines.html), while `Web3` is using [`Task`](https://msdn.microsoft.com/en-us/library/system.threading.tasks.task(v=vs.110).aspx). The syntax on how to using those will not be covered here. Furthermore, the rest demos will be using `UnityRPCRequests` since `Web3` tutorials and demos can be found in many other places.
