using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace ApianAnchor.Contracts.MinApianAnchor.ContractDefinition
{
    public partial class ApianEpoch : ApianEpochBase { }

    public class ApianEpochBase 
    {
        [Parameter("string", "sessionId", 1)]
        public virtual string SessionId { get; set; }
        [Parameter("uint256", "epochNum", 2)]
        public virtual BigInteger EpochNum { get; set; }
        [Parameter("uint256", "endApianTime", 3)]
        public virtual BigInteger EndApianTime { get; set; }
        [Parameter("uint256", "endCmdSeqNumber", 4)]
        public virtual BigInteger EndCmdSeqNumber { get; set; }
        [Parameter("string", "endStateHash", 5)]
        public virtual string EndStateHash { get; set; }
        [Parameter("address[]", "proxyAddrs", 6)]
        public virtual List<string> ProxyAddrs { get; set; }
        [Parameter("uint8[]", "proxyFlags", 7)]
        public virtual List<byte> ProxyFlags { get; set; }
        [Parameter("bytes[]", "proxySigs", 8)]
        public virtual List<byte[]> ProxySigs { get; set; }
    }
}
