
using System;
using System.Collections.Generic;
using System.Linq;
using Nethereum.Hex.HexConvertors.Extensions;
// using System.Threading.Tasks;
// using System.Collections.Generic;
// using System.Numerics;
// using Nethereum.Hex.HexTypes;
// using Nethereum.ABI.FunctionEncoding.Attributes;

using ApianCrypto;

namespace ApianAnchor.Contracts.MinApianAnchor.ContractDefinition
{
    public partial class ApianEpoch
    {

        public static ApianEpoch FromApian(AnchorSessionEpoch se)
        {
            var epoch = new ApianEpoch();
            epoch.SessionId = se.SessionId;
            epoch.EpochNum  = se.EpochNum;
            epoch.EndApianTime = se.EndApianTime;
            epoch.EndCmdSeqNumber = se.EndCmdSeqNumber;
            epoch.EndStateHash = se.EndStateHash;
            epoch.ProxyAddrs = se.ProxyAddrs;
            epoch.ProxyFlags = se.ProxyFlags;
            epoch.ProxySigs = se.ProxySigs.Select(s => s.HexToByteArray()).ToList();


            return epoch;
        }

        public AnchorSessionEpoch ToApian()
        {
            return new AnchorSessionEpoch( SessionId, (long)EpochNum, (long)EndApianTime, (long)EndCmdSeqNumber,
                EndStateHash, ProxyAddrs, ProxyFlags, ProxySigs.Select( (ba) =>  BitConverter.ToString(ba).Replace("-", "") ).ToList() );
        }

    }



}