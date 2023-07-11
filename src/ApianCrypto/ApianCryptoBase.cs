using System;
#if !SINGLE_THREADED
using System.Threading.Tasks;
#endif

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
    {
        public abstract string CurrentAccountAddress {get; }
        public abstract string SetNewAccount();
        public abstract string SetAccountFromKey(string privateKeyHex);
        public abstract string SetAccountFromKeystore(string password, string ksJson);
        public abstract string KeystoreForCurrentAccount(string password);
        public abstract (string,string) KeystoreForNewAccount(string password);
        public abstract string EncodeUTF8AndSign(string adddr, string msg);
        public abstract string EncodeUTF8AndEcRecover(string message, string signature);
        public abstract string HashString(string data);

        public abstract bool IsConnected {get;}
        public abstract void Connect(string provider, IApianCryptoClient client=null);
        public abstract void Connect(Object provider, IApianCryptoClient client = null);

        public abstract void Disconnect();

        public abstract void GetChainId();
        public abstract void GetBlockNumber();
        public abstract void GetBalance(string addr);

#if !SINGLE_THREADED
        public abstract Task<int> GetChainIdAsync();
        public abstract Task<int> GetBlockNumberAsync();
        public abstract Task<int> GetBalanceAsync(string addr);
#endif

    }
}