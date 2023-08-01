using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using ApianAnchor.Contracts.MinApianAnchor.ContractDefinition;

namespace ApianAnchor.Contracts.MinApianAnchor
{
    public partial class MinApianAnchorService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, MinApianAnchorDeployment minApianAnchorDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<MinApianAnchorDeployment>().SendRequestAndWaitForReceiptAsync(minApianAnchorDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, MinApianAnchorDeployment minApianAnchorDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<MinApianAnchorDeployment>().SendRequestAsync(minApianAnchorDeployment);
        }

        public static async Task<MinApianAnchorService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, MinApianAnchorDeployment minApianAnchorDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, minApianAnchorDeployment, cancellationTokenSource);
            return new MinApianAnchorService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.IWeb3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public MinApianAnchorService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public MinApianAnchorService(Nethereum.Web3.IWeb3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<BigInteger> GetSessionCountQueryAsync(GetSessionCountFunction getSessionCountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetSessionCountFunction, BigInteger>(getSessionCountFunction, blockParameter);
        }

        
        public Task<BigInteger> GetSessionCountQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetSessionCountFunction, BigInteger>(null, blockParameter);
        }

        public Task<GetSessionEpochOutputDTO> GetSessionEpochQueryAsync(GetSessionEpochFunction getSessionEpochFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetSessionEpochFunction, GetSessionEpochOutputDTO>(getSessionEpochFunction, blockParameter);
        }

        public Task<GetSessionEpochOutputDTO> GetSessionEpochQueryAsync(string sessionId, BigInteger epochNum, BlockParameter blockParameter = null)
        {
            var getSessionEpochFunction = new GetSessionEpochFunction();
                getSessionEpochFunction.SessionId = sessionId;
                getSessionEpochFunction.EpochNum = epochNum;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetSessionEpochFunction, GetSessionEpochOutputDTO>(getSessionEpochFunction, blockParameter);
        }

        public Task<List<string>> GetSessionIdsQueryAsync(GetSessionIdsFunction getSessionIdsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetSessionIdsFunction, List<string>>(getSessionIdsFunction, blockParameter);
        }

        
        public Task<List<string>> GetSessionIdsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetSessionIdsFunction, List<string>>(null, blockParameter);
        }

        public Task<GetSessionInfoOutputDTO> GetSessionInfoQueryAsync(GetSessionInfoFunction getSessionInfoFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetSessionInfoFunction, GetSessionInfoOutputDTO>(getSessionInfoFunction, blockParameter);
        }

        public Task<GetSessionInfoOutputDTO> GetSessionInfoQueryAsync(string sessionId, BlockParameter blockParameter = null)
        {
            var getSessionInfoFunction = new GetSessionInfoFunction();
                getSessionInfoFunction.SessionId = sessionId;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetSessionInfoFunction, GetSessionInfoOutputDTO>(getSessionInfoFunction, blockParameter);
        }

        public Task<string> RegisterSessionRequestAsync(RegisterSessionFunction registerSessionFunction)
        {
             return ContractHandler.SendRequestAsync(registerSessionFunction);
        }

        public Task<TransactionReceipt> RegisterSessionRequestAndWaitForReceiptAsync(RegisterSessionFunction registerSessionFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(registerSessionFunction, cancellationToken);
        }

        public Task<string> RegisterSessionRequestAsync(SessionInfo sessInfo)
        {
            var registerSessionFunction = new RegisterSessionFunction();
                registerSessionFunction.SessInfo = sessInfo;
            
             return ContractHandler.SendRequestAsync(registerSessionFunction);
        }

        public Task<TransactionReceipt> RegisterSessionRequestAndWaitForReceiptAsync(SessionInfo sessInfo, CancellationTokenSource cancellationToken = null)
        {
            var registerSessionFunction = new RegisterSessionFunction();
                registerSessionFunction.SessInfo = sessInfo;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(registerSessionFunction, cancellationToken);
        }

        public Task<string> ReportEpochRequestAsync(ReportEpochFunction reportEpochFunction)
        {
             return ContractHandler.SendRequestAsync(reportEpochFunction);
        }

        public Task<TransactionReceipt> ReportEpochRequestAndWaitForReceiptAsync(ReportEpochFunction reportEpochFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(reportEpochFunction, cancellationToken);
        }

        public Task<string> ReportEpochRequestAsync(ApianEpoch epoch)
        {
            var reportEpochFunction = new ReportEpochFunction();
                reportEpochFunction.Epoch = epoch;
            
             return ContractHandler.SendRequestAsync(reportEpochFunction);
        }

        public Task<TransactionReceipt> ReportEpochRequestAndWaitForReceiptAsync(ApianEpoch epoch, CancellationTokenSource cancellationToken = null)
        {
            var reportEpochFunction = new ReportEpochFunction();
                reportEpochFunction.Epoch = epoch;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(reportEpochFunction, cancellationToken);
        }

        public Task<string> VersionQueryAsync(VersionFunction versionFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VersionFunction, string>(versionFunction, blockParameter);
        }

        
        public Task<string> VersionQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VersionFunction, string>(null, blockParameter);
        }
    }
}
