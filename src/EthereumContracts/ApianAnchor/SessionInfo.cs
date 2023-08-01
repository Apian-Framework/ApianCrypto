
using ApianCrypto;

namespace ApianAnchor.Contracts.MinApianAnchor.ContractDefinition
{
    public partial class SessionInfo
    {
        public AnchorSessionInfo ToApian()
        {
            return new AnchorSessionInfo( Id , Name, Creator, ApianGroupType, GenesisHash);
        }

        public static SessionInfo FromApian(AnchorSessionInfo asi)
        {
            var solInfo =  new SessionInfo();
            solInfo.Id = asi.Id;
            solInfo.Name = asi.Name;
            solInfo.Creator = asi.Creator;
            solInfo.ApianGroupType = asi.ApianGroupType;
            solInfo.GenesisHash = asi.GenesisHash;
            return solInfo;
        }

    }

}