using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

namespace Block42
{

    public class TransferTokenDemo : MonoBehaviour
    {

        [SerializeField] private string _fromAdress = "0x10b30630929cbc9cc8142394c3dcf65262619329";
        [SerializeField] private InputField _toAdressInputField;
        [SerializeField] private InputField _toAmountInputField;

        private StandardToken _token;

        void Start()
        {
            _token = new StandardToken();
        }

        public async void OnTransferClick()
        {
			await _token.UnlockAcct(WalletManager.CurrentWalletAddress, "poanode123");

            BigInteger amount;
            if (BigInteger.TryParse(_toAmountInputField.text, out amount))
            {
				await _token.Transfer(_toAdressInputField.text, amount, WalletManager.CurrentWalletAddress);
                Debug.LogFormat("TransferTokenDemo - Start transfering {0}", _toAmountInputField.text);
            }
            else
            {
                Debug.LogError("TransferTokenDemo - Please enter amount as integer");
            }
        }

    }

}