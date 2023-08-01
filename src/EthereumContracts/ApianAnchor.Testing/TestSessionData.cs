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
    public class SessionStartData
    {
        public  ulong epoch { get; set; }
        public  ulong apianTime { get; set; }
        public  ulong cmdNum { get; set; }
        public  string stateHash { get; set; }
    }

    public static class TestSessionData
    {
        public const int  PROXY_PLAYER = 1; // acct is a player
        public const int  PROXY_VALIDATOR = 0; // acct is a validator  player/validator is exclusive
        public const int  PROXY_JOINED = 2; // acct is new in this epoch
        public const int  PROXY_LEFT = 4; // acct left during this epoch

        // Session A
        public static SessionInfo sessionAInfo = new ApianSessionInfo()
        {
            Id = "ApricotSessionId", // virtual string Id { get; set; }
            Name = "ApricotSessionName", //  public virtual string Name { get; set; }
            Creator = "0x1234567890", // public virtual string Creator { get; set; }
            ApianGroupType = "CreatorSez" // string ApianGroupType { get; set; }
        };

        public static SessionStartData sessionAStart = new SessionStartData()
        {
            epoch=1, apianTime=100, cmdNum=0, stateHash="sessAGenesisHash"
        };

        public static List<ApianEpoch> sessionAEpochs = new List<ApianEpoch>()
        {
            new ApianEpoch(){SessionId=sessionAInfo.Id, EpochNum=2, EndApianTime=200, EndCmdSeqNumber=20, EndStateHash="sessAEp2Hash",
                ProxyAddrs={"0x1234", "0x2345"}, ProxyFlags={PROXY_JOINED|PROXY_PLAYER, PROXY_JOINED|PROXY_PLAYER}, ProxySigs={"sAe2sig123","sAe2sig234"} },
            new ApianEpoch(){SessionId=sessionAInfo.Id, EpochNum=3, EndApianTime=300, EndCmdSeqNumber=30, EndStateHash="sessAEp3Hash",
                ProxyAddrs={"0x1234", "0x2345"}, ProxyFlags={PROXY_PLAYER, PROXY_PLAYER}, ProxySigs={"sAe3sig123","sAe3sig234"} },
            new ApianEpoch(){SessionId=sessionAInfo.Id, EpochNum=4, EndApianTime=400, EndCmdSeqNumber=40, EndStateHash="sessAEp4Hash",
                ProxyAddrs={"0x1234", "0x2345"}, ProxyFlags={PROXY_PLAYER|PROXY_LEFT, PROXY_PLAYER}, ProxySigs={"","sAe4sig234"} }

        };



        public static SessionInfo sessionBInfo = new ApianSessionInfo()
        {
            "BerrySessionId", // virtual string Id { get; set; }
            "BerrySessionName", //  public virtual string Name { get; set; }
            "0x1212121212", // public virtual string Creator { get; set; }
            "LeaderSez" // string ApianGroupType { get; set; }
        };

    public static SessionStartData sessionBStart = new SessionStartData()
        {
            10, 1000, 0, "sessBGenesisHash"
        };



    }
}