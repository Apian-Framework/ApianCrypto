using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace ApianAnchor.Contracts.MinApianAnchor.ContractDefinition
{
    public partial class SessionInfo : SessionInfoBase { }

    public class SessionInfoBase 
    {
        [Parameter("string", "id", 1)]
        public virtual string Id { get; set; }
        [Parameter("string", "name", 2)]
        public virtual string Name { get; set; }
        [Parameter("address", "creator", 3)]
        public virtual string Creator { get; set; }
        [Parameter("string", "apianGroupType", 4)]
        public virtual string ApianGroupType { get; set; }
        [Parameter("string", "genesisHash", 5)]
        public virtual string GenesisHash { get; set; }
    }
}
