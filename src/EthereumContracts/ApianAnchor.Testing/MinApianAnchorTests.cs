using System.Numerics;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using Nethereum.XUnitEthereumClients;
using Xunit;
using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Nethereum.BlockchainProcessing.ProgressRepositories;

using ApianAnchor.Contracts.MinApianAnchor;
using ApianAnchor.Contracts.MinApianAnchor.ContractDefinition;

namespace ApianEthContracts.Testing
{
    [Collection(EthereumClientIntegrationFixture.ETHEREUM_CLIENT_COLLECTION_DEFAULT)]
    public class ApianAnchorTests
    {
        private readonly EthereumClientIntegrationFixture _ethereumClientIntegrationFixture;

        public ApianAnchorTests(EthereumClientIntegrationFixture ethereumClientIntegrationFixture)
        {
            _ethereumClientIntegrationFixture = ethereumClientIntegrationFixture;
        }


        [Fact]
        public async void ShouldDeploy()
        {

            //You can connect to Infura directly if wanted using GetInfuraWeb3
            //var web3 = _ethereumClientIntegrationFixture.GetInfuraWeb3(InfuraNetwork.Ropsten);
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

            // //Example of using Legacy instead of 1559
            // if(_ethereumClientIntegrationFixture.EthereumClient == EthereumClient.Ganache)
            {
                web3.TransactionManager.UseLegacyAsDefault = true;
            }

            var anchorDeployment = new MinApianAnchorDeployment();

            //Deploy our custom token
            var anchorDeploymentReceipt = await MinApianAnchorService.DeployContractAndWaitForReceiptAsync(web3, anchorDeployment);

            //Creating a new service
            var anchorService = new MinApianAnchorService(web3, anchorDeploymentReceipt.ContractAddress);

            //validate that there's no sessions
            var sessCnt = await anchorService.GetSessionCountQueryAsync();
            Assert.Equal(0, sessCnt);

            // Should also be an empty list
            var sessIdList = await anchorService.GetSessionIdsQueryAsync();
            Assert.Equal( new List<string>(), sessIdList);
        }

      public async void ShouldAllowSessionRegistration()
        {
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();
            web3.TransactionManager.UseLegacyAsDefault = true;

            var anchorDeployment = new MinApianAnchorDeployment();
            var anchorDeploymentReceipt = await MinApianAnchorService.DeployContractAndWaitForReceiptAsync(web3, anchorDeployment);
            var anchorService = new MinApianAnchorService(web3, anchorDeploymentReceipt.ContractAddress);


            var regARcpt = await RegisterSessionRequestAndWaitForReceiptAsync(
                TestSessionData.sessionAInfo,
                sessionAInit.epoch, sessionAInit.time, sessionAInit.cmd, sessionAInit.hash);





        // [Fact]
        // public async void ShouldGetTransferEventLogs()
        // {

        //     var destinationAddress = "0x6C547791C3573c2093d81b919350DB1094707011";
        //     //Using ropsten infura if wanted for only a single test, as opposed to configuration
        //     //var web3 = _ethereumClientIntegrationFixture.GetInfuraWeb3(InfuraNetwork.Ropsten);
        //     var web3 = _ethereumClientIntegrationFixture.GetWeb3();

        //     //Example of using Legacy instead of 1559
        //     if (_ethereumClientIntegrationFixture.EthereumClient == EthereumClient.Ganache)
        //     {
        //         web3.TransactionManager.UseLegacyAsDefault = true;
        //     }

        //     var erc20TokenDeployment = new ERC20TokenDeployment() { DecimalUnits = 18, TokenName = "TST", TokenSymbol = "TST", InitialAmount = Web3.Convert.ToWei(10000) };

        //     //Deploy our custom token
        //     var tokenDeploymentReceipt = await ERC20TokenService.DeployContractAndWaitForReceiptAsync(web3, erc20TokenDeployment);

        //     //Creating a new service
        //     var tokenService = new ERC20TokenService(web3, tokenDeploymentReceipt.ContractAddress);

        //     //using Web3.Convert.ToWei as it has 18 decimal places (default)
        //     var transferReceipt1 = await tokenService.TransferRequestAndWaitForReceiptAsync(destinationAddress, Web3.Convert.ToWei(10, 18));
        //     var transferReceipt2 = await tokenService.TransferRequestAndWaitForReceiptAsync(destinationAddress, Web3.Convert.ToWei(10, 18));

        //     var transferEvent = web3.Eth.GetEvent<TransferEventDTO>();
        //     var transferFilter = transferEvent.GetFilterBuilder().AddTopic(x => x.To, destinationAddress).Build(tokenService.ContractHandler.ContractAddress,
        //         new BlockRange(transferReceipt1.BlockNumber, transferReceipt2.BlockNumber));

        //     var transferEvents = await transferEvent.GetAllChangesAsync(transferFilter);

        //     Assert.Equal(2, transferEvents.Count);

        }


        // [Fact]
        // public async void ShouldGetTransferEventLogsUsingProcessorAndStoreThem()
        // {

        //     var destinationAddress = "0x6C547791C3573c2093d81b919350DB1094707011";
        //     var web3 = _ethereumClientIntegrationFixture.GetWeb3();

        //     //Example of using Legacy instead of 1559
        //     if (_ethereumClientIntegrationFixture.EthereumClient == EthereumClient.Ganache)
        //     {
        //         web3.TransactionManager.UseLegacyAsDefault = true;
        //     }

        //     var erc20TokenDeployment = new ERC20TokenDeployment() { DecimalUnits = 18, TokenName = "TST", TokenSymbol = "TST", InitialAmount = Web3.Convert.ToWei(10000) };

        //     //Deploy our custom token
        //     var tokenDeploymentReceipt = await ERC20TokenService.DeployContractAndWaitForReceiptAsync(web3, erc20TokenDeployment);

        //     //Creating a new service
        //     var tokenService = new ERC20TokenService(web3, tokenDeploymentReceipt.ContractAddress);

        //     //using Web3.Convert.ToWei as it has 18 decimal places (default)
        //     var transferReceipt1 = await tokenService.TransferRequestAndWaitForReceiptAsync(destinationAddress, Web3.Convert.ToWei(10, 18));
        //     var transferReceipt2 = await tokenService.TransferRequestAndWaitForReceiptAsync(destinationAddress, Web3.Convert.ToWei(10, 18));


        //     //We are storing in a database the logs
        //     var storedMockedEvents = new List<EventLog<TransferEventDTO>>();
        //     //storage action mock
        //     Task StoreLogAsync(EventLog<TransferEventDTO> eventLog)
        //     {
        //         storedMockedEvents.Add(eventLog);
        //         return Task.CompletedTask;
        //     }

        //     //progress repository to restart processing (simple in memory one, use the other adapters for other storage possibilities)
        //     var blockProgressRepository = new InMemoryBlockchainProgressRepository(transferReceipt1.BlockNumber.Value - 1);

        //     //create our processor to retrieve transfers
        //     //restrict the processor to Transfers for a specific contract address
        //     var processor = web3.Processing.Logs.CreateProcessorForContract<TransferEventDTO>(
        //         tokenService.ContractHandler.ContractAddress, //the contract to monitor
        //         StoreLogAsync, //action to perform when a log is found
        //         minimumBlockConfirmations: 0,  // number of block confirmations to wait
        //         blockProgressRepository: blockProgressRepository //repository to track the progress
        //         );

        //     //if we need to stop the processor mid execution - call cancel on the token
        //     var cancellationToken = new CancellationToken();

        //     //crawl the required block range
        //     await processor.ExecuteAsync(
        //         cancellationToken: cancellationToken,
        //         toBlockNumber: transferReceipt2.BlockNumber.Value,
        //         startAtBlockNumberIfNotProcessed: transferReceipt1.BlockNumber.Value);

        //     Assert.Equal(2, storedMockedEvents.Count);

        // }

    }
}