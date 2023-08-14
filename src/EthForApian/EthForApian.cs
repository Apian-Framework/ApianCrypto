using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using UniLog;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;

using Newtonsoft.Json;

#if UNITY_WEBGL && !UNITY_EDITOR
using Nethereum.Unity.Rpc;
#endif

using ApianAnchor.Contracts.MinApianAnchor;
using ApianAnchor.Contracts.MinApianAnchor.ContractDefinition;

using System.Threading.Tasks;

namespace ApianCrypto
{
    public class EthForApian : IApianCrypto
    {
        protected Web3 web3;
        protected Account ethAccount;
        protected string providerURL;
        protected EthereumMessageSigner ethSigner;
        protected IApianCryptoClient callbackClient; // only necessary for single-threaded operation
        protected Dictionary<string, MinApianAnchorService> anchorsByContractAddr;
        protected Dictionary<string, MinApianAnchorService> anchorsBySessionId;
        protected Dictionary<string, string> contractAddrsBySessionId;

        public UniLogger logger;

        public bool IsConnected => web3 != null;
        public string CurrentAccountAddress => ethAccount?.Address;

        public static EthForApian Create()
        {
            return new EthForApian();
        }

        protected EthForApian()
        {
            logger = UniLogger.GetLogger("ApianCrypto");
            ResetContractAnchors();
            ethSigner = new EthereumMessageSigner();

        }

        protected void ResetContractAnchors()
        {
            anchorsByContractAddr = new Dictionary<string, MinApianAnchorService>();
            anchorsBySessionId = new Dictionary<string, MinApianAnchorService>();
            contractAddrsBySessionId = new Dictionary<string, string>();
        }

        // Connection
        public void Connect(string _providerURL, long chainId, IApianCryptoClient client=null)
        {
            providerURL = _providerURL;
            callbackClient = client;
            if (ethAccount != null)
            {
                BigInteger biChainId = new BigInteger(chainId);
                ethAccount = new Account(ethAccount.PrivateKey, biChainId); // FIXME: THis is a stupid way to do this
#if UNITY_WEBGL && !UNITY_EDITOR
                web3 = new Web3(ethAccount, new UnityWebRequestRpcTaskClient(new Uri(providerURL)));
#else
                web3 = new Web3( ethAccount, providerURL );
#endif
            } else {
                throw new Exception("No account loaded");
            }

        }

        public void Connect(Object provider,  IApianCryptoClient client = null)
        {
            callbackClient = client;
            web3 = provider as Web3;
            throw new Exception("Why is this firing?");
        }

        public void Disconnect()
        {
            web3 = null;
            callbackClient = null;
            ResetContractAnchors();
        }

        public void GetChainId()
        {
            throw new Exception("Single-threaded non-Unity GetChainId() not implmented");
        }

        public void GetBlockNumber()
        {
            throw new Exception("Single-threaded GetBlockNumber() not implmented");
        }

        public void GetBalance(string addr)
        {
            throw new Exception("Single-threaded GetBalance() not implmented");

        }

        public async Task<int> GetChainIdAsync()
        {
            if (web3 == null)
                throw new Exception("GetChainIdAsync() - Not connected to chain");
            BigInteger bi =   await web3.Eth.ChainId.SendRequestAsync();
            return (int)bi;
        }

        public async Task<int> GetBlockNumberAsync()
        {
            if (web3 == null)
                throw new Exception("GetBlockNumberAsync() - Not connected to chain");
            BigInteger bi = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            return (int)bi;
        }

        public async Task<int> GetBalanceAsync(string addr)
        {
            if (web3 == null)
                throw new Exception("GetBalanceAsync() - Not connected to chain");
            BigInteger bi = await web3.Eth.GetBalance.SendRequestAsync(addr);
            return (int)bi;
        }

        // Account
        public string SetNewAccount()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            ethAccount = new Account(ecKey);
            return CurrentAccountAddress;
        }

        public string SetAccountFromKey(string privateKeyHex)
        {
            ethAccount = new Account(privateKeyHex);
            return CurrentAccountAddress;
        }

        public string SetAccountFromKey(byte[] privateKeyBytes)
        {
            string pkStr =  BitConverter.ToString(privateKeyBytes).Replace("-", "") ; // System.Text.Encoding.UTF8.GetString(privateKeyBytes);
            return SetAccountFromKey(pkStr);
        }

        public string SetAccountFromKeystore(string password, string keyStoreJson)
        {
            var keyStoreService = new Nethereum.KeyStore.KeyStoreScryptService();
            var key = keyStoreService.DecryptKeyStoreFromJson(password, keyStoreJson);
            ethAccount = new Account(key);
            return CurrentAccountAddress;
        }

        public string KeystoreForCurrentAccount(string password)
        {
            var keyStoreService = new Nethereum.KeyStore.KeyStoreScryptService();
            var scryptParams = new Nethereum.KeyStore.Model.ScryptParams{Dklen = 32, N = 262144, R = 1, P = 8};
            var ecKey = new EthECKey(ethAccount.PrivateKey);
            var keyStore = keyStoreService.EncryptAndGenerateKeyStore(password, ecKey.GetPrivateKeyAsBytes(), ecKey.GetPublicAddress(), scryptParams);
            var json = keyStoreService.SerializeKeyStoreToJson(keyStore);
            return json;
        }

        public (string, string) KeystoreForNewAccount(string password)
        {
           var keyStoreService = new Nethereum.KeyStore.KeyStoreScryptService();
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var scryptParams = new Nethereum.KeyStore.Model.ScryptParams{Dklen = 32, N = 262144, R = 1, P = 8};
            var keyStore = keyStoreService.EncryptAndGenerateKeyStore(password, ecKey.GetPrivateKeyAsBytes(), ecKey.GetPublicAddress(), scryptParams);
            var json = keyStoreService.SerializeKeyStoreToJson(keyStore);
            return (ecKey.GetPublicAddress(), json);
        }

        // Sign/recover text message
        public string EncodeUTF8AndSign(string addr, string msg)
        {
            if (addr.ToUpper() != ethAccount.Address.ToUpper())
                throw new Exception("EncodeUTF8AndSign() address {addr} is not loaded account: {ethAccount.Address}");

            return ethSigner.EncodeUTF8AndSign(msg,  new EthECKey(ethAccount.PrivateKey));

        }
        public string EncodeUTF8AndEcRecover(string msg, string signature)
        {
            //var  signer = new EthereumMessageSigner();
            return ethSigner.EncodeUTF8AndEcRecover(msg, signature);
        }

        // Hash without eth prefix
        public string HashString(string str)
        {
            return ethSigner.Hash(Encoding.UTF8.GetBytes(str)).ToHex();
        }

        // Session Anchor stuff (ISessionAnchor)

        public void AddSessionAnchorService(string sessionId, string contractAddr)
        {

            if (web3 == null)
                throw new Exception("AddSessionAnchorService() - Not connected to chain");

            if (string.IsNullOrEmpty(contractAddr))
            {
                throw new Exception("AddSessionAnchorService(): ContractAddr cannot be null or empty");
            }

            // No session ID is a special case for when you want to do requests from contracts that you aren.t talking to.

            if ( string.IsNullOrEmpty(sessionId) )
            {
                if (!anchorsByContractAddr.ContainsKey(contractAddr))
                {
                    logger.Info($"AddSessionAnchorService(): Creating sessionless anchor: {contractAddr}");
                    anchorsByContractAddr[contractAddr]  = new MinApianAnchorService(web3, contractAddr);
                    }
                else
                    logger.Info($"AddSessionAnchorService(): Sessionless anchor exists: {contractAddr}");
                return;
            }

            // Normal case with session Id
            if (anchorsBySessionId.ContainsKey(sessionId))
            {
                logger.Warn($"AddSessionAnchorService(): Anchor for session: {sessionId} alreadtexists");
                return;
            }

            if (!anchorsByContractAddr.ContainsKey(contractAddr))
            {
                // Create one for this contract
                anchorsByContractAddr[contractAddr]  = new MinApianAnchorService(web3, contractAddr);
            }

            anchorsBySessionId[sessionId] = anchorsByContractAddr[contractAddr];
            contractAddrsBySessionId[sessionId] = contractAddr;
            logger.Verbose($"AddSessionAnchorService(): Anchor created for (session, contract): ({sessionId}, {contractAddr})");
        }

        public void RegisterSession(string sessionId, AnchorSessionInfo sessInfo)
        {

            throw new Exception("No Single-threaded RegisterSession()");

        }


        public void ReportEpoch(string sessionId, ApianEpochReport epoch)
        {

            throw new Exception("No Single-threaded ReportEpoch() ");

        }


        public async Task<long> GetContractSessionCountAsync(string sessionId, string contractAddr = null)
        {
            if (web3 == null)
                throw new Exception("GetContractSessionCountAsync() - Not connected to chain");

            // If contract addr is non-null then it is queried, regardless of session Id
            // Otherwise the session's contract is queried
            // There DOES already need to be an entry for he contract addr.
            if ( !string.IsNullOrEmpty(contractAddr) ) {
                if (anchorsByContractAddr.ContainsKey(contractAddr)) {
                    return await anchorsByContractAddr[contractAddr].GetContractSessionCountAsync();
                } else {
                    logger.Error($"No anchor for contract: {contractAddr}");
                }
            } else {
                if (anchorsBySessionId.ContainsKey(sessionId)) {
                    return await anchorsBySessionId[sessionId].GetContractSessionCountAsync();
                } else {
                    logger.Error($"No anchor for session: {sessionId}");
                }
            }
            return 0; // TODO: Define ApianCrypto exceptions and handle 'em
        }

        public async Task<List<string>> GetContractSessionIdsAsync(string sessionId, string contractAddr = null)
        {
            if (web3 == null)
                throw new Exception("GetContractSessionIdsAsync() - Not connected to chain");

            // Another contract-wide query. See above for details
            if ( !string.IsNullOrEmpty(contractAddr) ) {
                if (anchorsByContractAddr.ContainsKey(contractAddr)) {
                    return await anchorsByContractAddr[contractAddr].GetContractSessionIdsAsync();
                } else {
                    logger.Error($"No anchor for contract: {contractAddr}");
                }
            } else {
                if (anchorsBySessionId.ContainsKey(sessionId)) {
                    return await anchorsBySessionId[sessionId].GetContractSessionIdsAsync();
                } else {
                    logger.Error($"No anchor for session: {sessionId}");
                }
            }
            return null; // TODO: Define ApianCrypto exceptions and handle 'em
        }

        public async Task<(AnchorSessionInfo, IList<long>)> GetSessionDataAsync(string sessionId)
        {
            if (web3 == null)
                throw new Exception("GetSessionDataAsync() - Not connected to chain");

            // returns a (sessionInfo, List<long> epochNums) tuple
            if (anchorsBySessionId.ContainsKey(sessionId)) {
                return await anchorsBySessionId[sessionId].GetSessionDataAsync(sessionId);
            } else {
                logger.Error($"No anchor for session: {sessionId}");
            }
            return (null, null);
        }

        public async Task<ApianEpochReport> GetSessionEpochAsync( string sessionId, long epochNum)
        {
            if (web3 == null)
                throw new Exception("GetSessionEpochAsync() - Not connected to chain");

            if (anchorsBySessionId.ContainsKey(sessionId)) {
                return await anchorsBySessionId[sessionId].GetSessionEpochAsync(sessionId, epochNum);
            } else {
                logger.Error($"No anchor for session: {sessionId}");
            }
            return null;
        }

        public async Task<string> RegisterSessionAsync(string sessionId, AnchorSessionInfo sessInfo)
        {
            if (web3 == null)
                throw new Exception("RegisterSessionAsync() - Not connected to chain");

            if (anchorsBySessionId.ContainsKey(sessionId)) {
                return await anchorsBySessionId[sessionId].RegisterSessionAsync( sessInfo);
            } else {
                logger.Error($"No anchor for session: {sessionId}");
            }
            return null;
        }

        public async Task<string> ReportEpochAsync(string sessionId, ApianEpochReport epoch)
        {
            if (web3 == null)
                throw new Exception("ReportEpochAsync() - Not connected to chain");

            if (anchorsBySessionId.ContainsKey(sessionId)) {

                string js = JsonConvert.SerializeObject(epoch, Formatting.Indented);
                logger.Info($"Reporting epoch {epoch.EpochNum}:\n{js}");

                return await anchorsBySessionId[sessionId].ReportEpochAsync(epoch);
            } else {
                logger.Error($"No anchor for session: {sessionId}");
            }
            return null;
        }

    }

}