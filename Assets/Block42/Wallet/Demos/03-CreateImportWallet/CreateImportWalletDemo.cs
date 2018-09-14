using UnityEngine;
using UnityEngine.UI;

namespace Block42
{
	public class CreateImportWalletDemo : MyWalletDemo
	{

		[SerializeField] private InputField _createPasswordInputField;
		[SerializeField] protected InputField _createPublicKeyInputField, _createPrivateKeyInputField, _createEncryptedJsonInputField;

		[SerializeField] private InputField _importEncryptedJsonInputField, _importPasswordInputField;
		[SerializeField] protected InputField _importPublicKeyInputField, _importPrivateKeyInputField;

		private void Awake()
		{
			WalletManager.walletCreated += DisplayCurrentWalletAddress;
		}

		public void OnCreateWalletClick()
		{
			OnResetWalletClick(); // We only demo the first wallet, so reset it first before creating
			WalletManager.CreateWallet("Dummy Account Name", _createPasswordInputField.text);

			// Print out some wallet parameters after creation
			_createPublicKeyInputField.text = WalletManager.CurrentWallet.address;
			_createPrivateKeyInputField.text = WalletManager.CurrentWallet.privateKey;
			_createEncryptedJsonInputField.text = WalletManager.CurrentWallet.encryptedJson;
		}

		public void OnResetWalletClick()
		{
			WalletManager.DeleteWalletDataFile();
			WalletManager.LoadWallets();
			DisplayCurrentWalletAddress(); // Mannually reset the address in UI
		}

		public void OnImportWalletClick()
		{
			OnResetWalletClick(); // We only demo the first wallet, so reset it first before importing

			try
			{
				WalletManager.ImportWallet("Dummy Account Name", _importPasswordInputField.text, _importEncryptedJsonInputField.text);
			}
			catch (System.Exception e)
			{
				Debug.LogError("Import wallet failed - password mismatch.");
				_importPublicKeyInputField.text = string.Empty;
				_importPrivateKeyInputField.text = string.Empty;
			}

			// Print out some wallet parameters after import
			_importPublicKeyInputField.text = WalletManager.CurrentWalletAddress;
			_importPrivateKeyInputField.text = WalletManager.CurrentWalletPrivateKey;
		}

	}

}