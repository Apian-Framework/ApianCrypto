using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ApianCrypto
{

    public class BlockchainInfo
    {
        public string RpcUrl;
        public int ChainId;
        public string Currency; // "ETH", "xDAI", etc. This assumes that "decimals" for the chain is always 18.

        public BlockchainInfo() {} // default ctor for NewtonSoft
    }



    // TODO: This might ought to go away if there doesn't turn out to be any implementation-independent
    // common code

    public abstract class ApianCryptoBase : IApianCrypto
    {   public abstract string CurrentAccountAddress {get; }
        public abstract string SetNewAccount();
        public abstract string SetAccountFromKey(string privateKeyHex);
        public abstract string SetAccountFromKeystore(string password, string ksJson);
        public abstract string KeystoreForCurrentAccount(string password);
        public abstract (string,string) KeystoreForNewAccount(string password);
        public abstract string EncodeUTF8AndSign(string adddr, string msg);
        public abstract string EncodeUTF8AndEcRecover(string message, string signature);
        public abstract string HashString(string data);

        public abstract bool IsConnected {get;}
        public abstract void Connect(string provider, long chainId, IApianCryptoClient client=null);
        public abstract void Connect(Object provider, IApianCryptoClient client = null);

        public abstract void Disconnect();

        // TODO: do we need these syncronous/callback versions?
        // I'm not even sure they work.
        public abstract void GetChainId();
        public abstract void GetBlockNumber();
        public abstract void GetBalance(string addr);
        public void ValidateAnchorVersion(string contractAddr) { throw new NotImplementedException(); }
        public void AddSessionAnchorService( string sessionId, string contractAddr) { throw new NotImplementedException(); }
        public void RegisterSession(string sessionId, AnchorSessionInfo sessInfo) { throw new NotImplementedException(); }
        public void ReportEpoch(string sessionId, ApianEpochReport epoch) { throw new NotImplementedException(); }

        public abstract Task<int> GetChainIdAsync();
        public abstract Task<int> GetBlockNumberAsync();
        public abstract Task<int> GetBalanceAsync(string addr);

        // ISessionANchor
        public Task<(bool,string)> ValidateAnchorVersionAsync(string contractAddr) { throw new NotImplementedException(); }
        public Task<long> GetContractSessionCountAsync(string sessionId, string contractAddr) {throw new NotImplementedException();  }
        public Task<List<string>> GetContractSessionIdsAsync(string sessionId, string contractAddr) { throw new NotImplementedException(); }
        public Task<(AnchorSessionInfo, IList<long>)> GetSessionDataAsync(string sessionId)  { throw new NotImplementedException(); }
        public Task<ApianEpochReport> GetSessionEpochAsync( string sessionId, long epochNum) { throw new NotImplementedException(); }
        public Task<string> RegisterSessionAsync(string sessionId, AnchorSessionInfo sessInfo) { throw new NotImplementedException(); }
        public Task<string> ReportEpochAsync(string sessionId, ApianEpochReport epoch) { throw new NotImplementedException(); }


    }
}