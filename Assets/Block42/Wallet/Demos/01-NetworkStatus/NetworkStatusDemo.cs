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
			WalletManager.GetBlockNumber((blockNumber) =>
			{
				_blockNumberText.text = blockNumber.ToString();
			});
		}

		private void UpdateGasPrice()
		{
			WalletManager.GetGasPrice((gasPrice) =>
			{
				_gasPriceText.text = gasPrice.ToString() + " GWEI";
			});
		}

		private void UpdateLatestBlock()
		{
			WalletManager.GetLatestBlock((block) =>
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
			});
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