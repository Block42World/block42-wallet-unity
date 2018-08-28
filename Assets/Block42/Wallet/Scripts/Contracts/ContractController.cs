using System.Collections;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.JsonRpc.UnityClient;
using UnityEngine;
using UnityEngine.Events;

namespace Block42
{

    // A abstract base class that interact with Ethereum contract, all contracts should inherent from this calss.
    public abstract class ContractController : MonoBehaviour
    {

        [Tooltip("The ABI of the contract we are going to use. On way to get this is by going to remix IDE and view contract details.")]
        [SerializeField] protected string _abi;
        
        [Tooltip("Contract address, found that when deploying the contract or check existing contract on Etherescan.")]
        [SerializeField] protected string _address;

        [SerializeField] protected bool _debugLog;

        private Contract _contract;

        protected virtual void Awake()
        {
            _contract = new Contract(null, _abi, _address);
			Debug.Log(_contract);
        }

		protected Function GetFunction(string name)
        {
            return _contract.GetFunction(name);
        }

		protected CallInput CreateCallInput(string name, params object[] functionInput)
        {
            return GetFunction(name)?.CreateCallInput(functionInput);
        }

		protected IEnumerator CallCoroutine(string variable, object functionInput1, UnityAction<string> onSuccess = null, UnityAction<System.Exception> onFailed = null)
		{
			var callInput = CreateCallInput(variable, functionInput1);
			yield return StartCoroutine(CallCoroutine(callInput, onSuccess, onFailed));
		}

		protected IEnumerator CallCoroutine(string variable, object functionInput1, object functionInput2, UnityAction<string> onSuccess = null, UnityAction<System.Exception> onFailed = null)
		{
			var callInput = CreateCallInput(variable, functionInput1, functionInput2);
			yield return StartCoroutine(CallCoroutine(callInput, onSuccess, onFailed));
		}

		protected IEnumerator CallCoroutine(string variable, object functionInput1, object functionInput2, object functionInput3, UnityAction<string> onSuccess = null, UnityAction<System.Exception> onFailed = null)
		{
			var callInput = CreateCallInput(variable, functionInput1, functionInput2, functionInput3);
			yield return StartCoroutine(CallCoroutine(callInput, onSuccess, onFailed));
		}

		protected IEnumerator CallCoroutine(string variable, object functionInput1, object functionInput2, object functionInput3, object functionInput4, UnityAction<string> onSuccess = null, UnityAction<System.Exception> onFailed = null)
		{
			var callInput = CreateCallInput(variable, functionInput1, functionInput2, functionInput3, functionInput4);
			yield return StartCoroutine(CallCoroutine(callInput, onSuccess, onFailed));
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

        protected T DecodeVariable<T>(string variableName, string result)
        {
            var function = GetFunction(variableName);
            try
            {
                return function.DecodeSimpleTypeOutput<T>(result); // this results in an error if BigInteger is 0
            }
            catch
            {
                return default(T);
            }
        }

    }

}