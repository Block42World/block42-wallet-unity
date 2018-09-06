using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Util;

namespace Block42
{
	public class NetworkStatusDemo : MonoBehaviour
	{

		[SerializeField] protected Text _networkText, _blockNumberText, _gasPriceText, _authorText, _hashText, _difficultyText, _extraDataText, _gasLimitText, _gasUsedText, _logBloomText, _minerText, _nounText, _numberText, _parentHashText, _receiptsRootText, _sealFieldCountText, _sha3UnclesText, _sizeText, _stateRootText, _timestampText, _totalDifficultyText, _transactionRootText, _uncleCountText, _transactionCountText;

		private void Awake()
		{
			_networkText.text = WalletSettings.current.networkUrl;
		}

		private void Start()
		{
			UpdateBlockNumber();
			UpdateGasPrice();
			UpdateLatestBlock();
		}

		private void UpdateBlockNumber()
		{
			StartCoroutine(GetBlockNumber((blockNumber) =>
			{
				_blockNumberText.text = blockNumber.ToString();
			}));
		}

		private IEnumerator GetBlockNumber(UnityAction<int> callback)
		{
			// Use EthBlockNumberUnityRequest from the Nethereum lib to send block number request
			var blockNumberRequest = new EthBlockNumberUnityRequest(WalletSettings.current.networkUrl);
			yield return blockNumberRequest.SendRequest();

			if (blockNumberRequest.Exception == null)
			{
				callback((int)blockNumberRequest.Result.Value);
			}
			else
			{
				throw new System.InvalidOperationException("Block number request failed, exception=" + blockNumberRequest.Exception);
			}
		}

		private void UpdateGasPrice()
		{
			StartCoroutine(GetGasPrice((gasPrice) =>
			{
				_gasPriceText.text = gasPrice.ToString() + " GWEI";
			}));
		}

		private IEnumerator GetGasPrice(UnityAction<decimal> callback)
		{
			// Use EthGasPriceUnityRequest from the Nethereum lib to send gas price request
			var gasPriceRequest = new EthGasPriceUnityRequest(WalletSettings.current.networkUrl);
			yield return gasPriceRequest.SendRequest();

			if (gasPriceRequest.Exception == null)
			{
				callback(UnitConversion.Convert.FromWei(gasPriceRequest.Result.Value, 9));
			}
			else
			{
				throw new System.InvalidOperationException("Gas price request failed, exception=" + gasPriceRequest.Exception);
			}
		}

		private void UpdateLatestBlock()
		{
			StartCoroutine(GetLatestBlock((block) =>
			{
				_authorText.text = block.Author;
				_hashText.text = block.BlockHash;
				_difficultyText.text = block.Difficulty.Value.ToString();
				_extraDataText.text = block.ExtraData;
				_gasLimitText.text = UnitConversion.Convert.FromWei(block.GasLimit, 9).ToString() + " GWEI";
				_gasUsedText.text = UnitConversion.Convert.FromWei(block.GasUsed, 9).ToString() + " GWEI";
				_logBloomText.text = block.LogsBloom;
				_minerText.text = block.Miner;
				_nounText.text = block.Nonce;
				_numberText.text = block.Number.Value.ToString();
				_parentHashText.text = block.ParentHash;
				_receiptsRootText.text = block.ReceiptsRoot;
				_sealFieldCountText.text = block.SealFields == null ? "0" : block.SealFields.Length.ToString();
				_sha3UnclesText.text = block.Sha3Uncles;
				_sizeText.text = block.Size.Value.ToString();
				_stateRootText.text = block.StateRoot;
				_timestampText.text = (new System.DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds((double)block.Timestamp.Value).ToString();
				_totalDifficultyText.text = block.TotalDifficulty.Value.ToString();
				_transactionRootText.text = block.TransactionsRoot;
				_uncleCountText.text = block.Uncles == null ? "0" : block.Uncles.Length.ToString();
				_transactionCountText.text = block.Transactions == null ? "0" : block.Transactions.Length.ToString();
			}));
		}

		private IEnumerator GetLatestBlock(UnityAction<Nethereum.RPC.Eth.DTOs.BlockWithTransactions> callback)
		{
			// Use EthBlockNumberUnityRequest from the Nethereum lib to send block number request
			var blockNumberRequest = new EthBlockNumberUnityRequest(WalletSettings.current.networkUrl);
			yield return blockNumberRequest.SendRequest();

			if (blockNumberRequest.Exception == null)
			{
				// Use EthGetBlockWithTransactionsByNumberUnityRequest from the Nethereum lib to send get block request
				var getBlockByNumberRequest = new EthGetBlockWithTransactionsByNumberUnityRequest(WalletSettings.current.networkUrl);
				yield return getBlockByNumberRequest.SendRequest(new Nethereum.Hex.HexTypes.HexBigInteger(blockNumberRequest.Result.Value));

				if (getBlockByNumberRequest.Exception == null)
				{
					callback(getBlockByNumberRequest.Result);
				}
				else
				{
					throw new System.InvalidOperationException("Get block request failed, exception=" + getBlockByNumberRequest.Exception);
				}
			}
			else
			{
				throw new System.InvalidOperationException("Block number request failed, exception=" + blockNumberRequest.Exception);
			}
		}

		public void OnRefreshClick()
		{
			Debug.Log("Refreshing...");
			UpdateBlockNumber();
			UpdateLatestBlock();
			_blockNumberText.text = string.Empty;
			_authorText.text = string.Empty;
			_hashText.text = string.Empty;
			_difficultyText.text = string.Empty;
			_extraDataText.text = string.Empty;
			_gasLimitText.text = string.Empty;
			_gasUsedText.text = string.Empty;
			_logBloomText.text = string.Empty;
			_minerText.text = string.Empty;
			_nounText.text = string.Empty;
			_numberText.text = string.Empty;
			_parentHashText.text = string.Empty;
			_receiptsRootText.text = string.Empty;
			_sealFieldCountText.text = string.Empty;
			_sha3UnclesText.text = string.Empty;
			_sizeText.text = string.Empty;
			_stateRootText.text = string.Empty;
			_timestampText.text = string.Empty;
			_totalDifficultyText.text = string.Empty;
			_transactionRootText.text = string.Empty;
			_uncleCountText.text = string.Empty;
			_transactionCountText.text = string.Empty;
		}

	}

}