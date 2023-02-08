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

        string AccountAddress {get; }
        string CreateAccount();
        string CreateAccountForKey(byte[] privateKeyBytes);
        string CreateAccountFromJson(string password, string acctJson);
        string GetJsonForAccount(string password);

        string HashString(string data);
        string EncodeUTF8AndSign(string msg); // w/ eth prefix
        string EncodeUTF8AndEcRecover(string message, string signature);

        bool IsConnected {get;}
        void Connect(string provider, IApianCryptoClient client = null); // provider is probably a url
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