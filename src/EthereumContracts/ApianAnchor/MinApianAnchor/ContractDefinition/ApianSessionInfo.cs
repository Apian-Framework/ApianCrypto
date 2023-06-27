using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace ApianAnchor.Contracts.MinApianAnchor.ContractDefinition
{
    public partial class ApianSessionInfo : ApianSessionInfoBase { }

    public class ApianSessionInfoBase 
    {
        [Parameter("string", "id", 1)]
        public virtual string Id { get; set; }
        [Parameter("string", "name", 2)]
        public virtual string Name { get; set; }
        [Parameter("address", "creator", 3)]
        public virtual string Creator { get; set; }
        [Parameter("string", "apianGroupType", 4)]
        public virtual string ApianGroupType { get; set; }
    }
}
