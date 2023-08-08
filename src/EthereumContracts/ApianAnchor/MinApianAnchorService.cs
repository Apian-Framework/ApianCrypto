
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ApianCrypto;
using ApianAnchor.Contracts.MinApianAnchor.ContractDefinition;

namespace ApianAnchor.Contracts.MinApianAnchor
{
    public partial class MinApianAnchorService
    {
        // This adds the ApianCrypto side of the Nethereum-generated class

        // Fetch total contract session count.
        public async Task<long> GetContractSessionCountAsync()
        {
            BigInteger bCount = await GetSessionCountQueryAsync();
            return (long)bCount;
        }
        public async Task<List<string>> GetContractSessionIdsAsync()
        {
            return await GetSessionIdsQueryAsync(); // doesn;t need any translating
        }

        public async Task<(AnchorSessionInfo, IList<long>)> GetSessionDataAsync(string sessionId)
        {
            // Gets a tuple of: (sessionInfo, [epochNums...])

            var queryRes = await GetSessionInfoQueryAsync(sessionId);
            return (queryRes.Info.ToApian(), queryRes.EpochNums.Select((bi) => (long)bi).ToList());
        }

        public async Task<ApianEpochReport> GetSessionEpochAsync(string sessionId, long epochNum)
        {
            var queryRes = await GetSessionEpochQueryAsync(sessionId, (BigInteger)epochNum);
            return queryRes.Epoch.ToApian();
        }

        public async Task<string> RegisterSessionAsync( AnchorSessionInfo sessInfo)
        {
             // Waits for TX to be submittd, returns TX hash - does NOT wait for receipt
            var txHash =  await RegisterSessionRequestAsync( SessionInfo.FromApian(sessInfo));
            return txHash;
        }

        public async Task<string> ReportEpochAsync( ApianEpochReport apEpoch)
        {
           // Waits for TX to be submittd, returns TX hash - does NOT wait for receipt
            ApianEpoch solEpoch =  ApianEpoch.FromApian(apEpoch);
            var txHash =  await ReportEpochRequestAsync(solEpoch);

            return txHash;
        }

    }
}