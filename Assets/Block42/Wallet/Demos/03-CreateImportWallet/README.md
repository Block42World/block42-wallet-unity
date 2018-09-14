![Block42](http://assets.block42.world/images/icons/block42_logo_200.png)

# Block42 Unity Wallet Demo 3 - Create / Import Wallet
The demo shows you how the wallet is created and imported locally, just like [MetaMask](https://metamask.io/).

## Demo Scene
Play [CreateImportWallet.unity](CreateImportWallet.unity) scene, you have 3 actions to choose: create, reset and import.
![Settings](/Documents/Demo-03-CreateImportWallet/01_screenshot.png)

- Create: Generate a public-private key pair is randomly, then the key encrypted by the password you choosen to a [keystore](https://nethereum.readthedocs.io/en/feat-noobs_4th_iteration/Nethereum.Docs/docs/introduction/keystore/).
- Reset: Remove the current wallet.
- Import: Given the keystore and password, recover the public-private key pair.

Once a wallet is created / imported, it saved as a serialized file locally. In therory, you only need to save the keystore and ask players to enter the password to retrive the public-private key pair, but encrypting and decrypting keystore takes few seconds, we may not want players to wait that long on every login, so we save the the public key and private key here too. You may want to use encryption plugin such as [Easy Save](https://docs.moodkie.com/easy-save-2/guides/encryption/) for the serialized file for extra security.

Players has to keep their keystore and rememer the password to retive their wallet, you can save it with password in your online account server, or simply keep it local and ask players to back it up by themselves.

Another option is skipping keystore generation and implementing [BIP39](https://github.com/bitcoinjs/bip39) and convert the private key into a 12-words mnemonic, similar to MetaMask.

## Scripts Overview
[CreateImportWalletDemo.cs](CreateImportWalletDemo.cs) calls `CreateWallet()` and `ImportWallet()` in **WalletManager**, which uses `Nethereum.Singer.EthECKey` for the public-private key pair and `Nthereum.KeyStore.KeyStoreService` for keystore:

```
public static void CreateWallet(string accountName, string password)
{
    // Create a new random key for public key and private key
    var ecKey = EthECKey.GenerateKey();

    var address = ecKey.GetPublicAddress();
    var privateKeyBytes = ecKey.GetPrivateKeyAsBytes();
    var privateKey = ecKey.GetPrivateKey();

    // Use key stroe service to generate a keystore
    var keystoreservice = new KeyStoreService();
    var encryptedJson = keystoreservice.EncryptAndGenerateDefaultKeyStoreAsJson(password, privateKeyBytes, address);

    // Created the wallet with given info
    AddWallet(accountName, ecKey.GetPublicAddress(), encryptedJson, ecKey.GetPrivateKey());
}
```
```
public static void ImportWallet(string accountName, string password, string encryptedJson)
{
    // Use key store service to re-generate the key
    var service = new KeyStoreService();
    byte[] bytes = service.DecryptKeyStoreFromJson(password, encryptedJson);
    var ecKey = new EthECKey(bytes, true);

    // Created the wallet with given info
    AddWallet(accountName, ecKey.GetPublicAddress(), encryptedJson, ecKey.GetPrivateKey());
}
```