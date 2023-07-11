using System;
#if !SINGLE_THREADED
using System.Threading.Tasks;
#endif

namespace ApianCrypto
{

    public interface IApianCryptoClient
    {
        void OnChainId(int chainId);
        void OnBlockNumber(int blockNumber);
        void OnBalance(string addr, int balance);
    }


    public interface IApianCrypto
    {

        string CurrentAccountAddress {get; }
        string SetNewAccount(); // creates and sets internally
        string SetAccountFromKey(string privateKeyHex);

        string SetAccountFromKeystore(string password, string ksJson);
        string KeystoreForCurrentAccount(string password);
        (string,string) KeystoreForNewAccount(string password); // ( address, json)

        string HashString(string data); // addr must match currently loaded acct
        string EncodeUTF8AndSign(string addr, string msg); // w/ eth prefix
        string EncodeUTF8AndEcRecover(string message, string signature);

        bool IsConnected {get;}
        void Connect(string providerString, IApianCryptoClient client = null); // provider is probably a url
        void Connect(Object provider, IApianCryptoClient client = null);
        void Disconnect();

        // SINGLE_THREADED API

        void GetChainId();
        void GetBlockNumber();
        void GetBalance(string addr);

#if !SINGLE_THREADED
        Task<int> GetChainIdAsync();
        Task<int> GetBlockNumberAsync();
        Task<int> GetBalanceAsync(string addr);
#endif

    }

}