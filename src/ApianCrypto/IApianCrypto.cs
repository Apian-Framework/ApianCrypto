using System;
using System.Collections.Generic;

#if !SINGLE_THREADED
using System.Threading.Tasks;
#endif

namespace ApianCrypto
{
    // Passing back any exceptions that occur is a pretty clean way to get around all of the messiness
    // involved with tasks and coroutines and all of the "how/where to catch 'em" questions. This
    // is expecially the case in Unity-platformed builds.
    public interface IApianCryptoClient
    {
        void OnChainId(int chainId, Exception ex);
        void OnBlockNumber(int blockNumber, Exception ex);
        void OnBalance(string addr, int balance, Exception ex);

        void OnSessionRegistered(string sessId, string txHash, Exception ex);
        void OnEpochReported(string sessId, long epochNum, string txHash, Exception ex);
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
        void Connect(string providerString, long chainId, IApianCryptoClient client = null); // provider is probably a url
        void Connect(Object provider, IApianCryptoClient client = null);
        void Disconnect();

        // TODO: get rid of the single-thread stuff
        // SINGLE_THREADED API
        //void GetChainId();
        //void GetBlockNumber();
        //void GetBalance(string addr);

        void AddSessionAnchorService(string sessionId, string contractAddr); // only 1 per session, but they can share a contract address

        //void RegisterSession(string sessionId, AnchorSessionInfo sessInfo); // not async - really just for Unity WebGL
        //void ReportEpoch(string sessionId, ApianEpochReport epoch);  // Also for Unity WebGL (single thread)

#if !SINGLE_THREADED
        Task<int> GetChainIdAsync();
        Task<int> GetBlockNumberAsync();
        Task<int> GetBalanceAsync(string addr);


        // Session anchor methods
        //
        // The idea is that this single ApianCrypto instance is using the same account to talk to multiple SessionAnchor contracts.
        // AddSessionAnchor() creates a new AnchorContractService each different contract and puts it a dictionary keyed by contract addr.
        // It also makes a sessionID -> ContractService dict so calls made with a session Id talk to the right contract.
        //
        Task<long> GetContractSessionCountAsync(string sessionId, string contractAddr = null);
        Task<List<string>> GetContractSessionIdsAsync(string sessionId, string contractAddr = null);
        Task<(AnchorSessionInfo, IList<long>)> GetSessionDataAsync(string sessionId);
        Task<ApianEpochReport> GetSessionEpochAsync( string sessionId, long epochNum);
        Task<string> RegisterSessionAsync(string sessionId, AnchorSessionInfo sessInfo);
        Task<string> ReportEpochAsync(string sessionId, ApianEpochReport epoch);
#endif


    }
}