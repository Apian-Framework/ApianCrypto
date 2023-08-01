
using System;
using System.Collections.Generic;
#if !SINGLE_THREADED
using System.Threading.Tasks;
#endif

namespace ApianCrypto
{

    // Session Anchor Contract structs
   public class AnchorSessionInfo
    {
        public virtual string Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Creator { get; set; }
        public virtual string ApianGroupType { get; set; }
        public virtual string GenesisHash { get; set; }

        public AnchorSessionInfo(string id, string name, string creator, string apianGroupType, string genesisHash)
        {
            Id = id;
            Name = name;
            Creator = creator;
            ApianGroupType = apianGroupType;
            GenesisHash = genesisHash;
        }
    }

    public class ApianEpochReport
    {
        public const int  PROXY_PLAYER = 1; // acct is a player
        public const int  PROXY_VALIDATOR = 0; // acct is a validator  player/validator is exclusive
        public const int  PROXY_JOINED = 2; // acct is new in this epoch
        public const int  PROXY_LEFT = 4; // acct left during this epoch

        public string SessionId { get; set; }
        public long EpochNum { get; set; }
        public long EndApianTime { get; set; }
        public long EndCmdSeqNumber { get; set; }
        public string EndStateHash { get; set; }
        public List<string> ProxyAddrs { get; set; }
        public List<byte> ProxyFlags { get; set; }
        public List<string> ProxySigs { get; set; }

        public ApianEpochReport(string sessionId, long epochNum, long endApianTime, long endCmdSeqNumber,
            string endStateHash, List<string> proxyAddrs, List<byte> proxyFlags, List<string> proxySigs)
        {
            SessionId = sessionId;
            EpochNum = epochNum;
            EndApianTime = endApianTime;
            EndCmdSeqNumber = endCmdSeqNumber;
            EndStateHash = endStateHash;
            ProxyAddrs = new List<string>(proxyAddrs);
            ProxyFlags = new List<byte>(proxyFlags);
            ProxySigs = new List<string>(proxySigs);
        }

    }


}