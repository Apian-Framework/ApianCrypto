
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
#if !SINGLE_THREADED
using System.Threading.Tasks;
#endif

using ApianCrypto;
using ApianAnchor.Contracts.MinApianAnchor.ContractDefinition;

namespace ApianAnchor.Contracts.MinApianAnchor
{

#if !SINGLE_THREADED
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

        public async Task<AnchorSessionEpoch> GetSessionEpochAsync(string sessionId, long epochNum)
        {
            var queryRes = await GetSessionEpochQueryAsync(sessionId, (BigInteger)epochNum);
            return queryRes.Epoch.ToApian();
        }

        public async Task<string> RegisterSessionAsync( AnchorSessionInfo sessInfo)
        {
             // Waits for TX to be submittd, returns TX hash - does NOT wait for receipt
            var txHash =  await RegisterSessionRequestAsync( ApianSessionInfo.FromApian(sessInfo));
            return txHash;
        }

        public async Task<string> ReportEpochAsync( AnchorSessionEpoch apEpoch)
        {
           // Waits for TX to be submittd, returns TX hash - does NOT wait for receipt
            var txHash =  await ReportEpochRequestAsync( ApianEpoch.FromApian(apEpoch));

            return txHash;
        }

    }
#endif // !SINGLE_THREADED
}