using System;
using System.Numerics;
using System.Text;
using UniLog;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;


#if !SINGLE_THREADED
using System.Threading.Tasks;
#endif

namespace ApianCrypto
{
    public class EthForApian : IApianCrypto
    {

        protected Web3 web3;
        protected Account ethAccount;
        protected EthereumMessageSigner ethSigner;
        protected IApianCryptoClient callbackClient; // only necessary for single-threaded operation

#if UNITY_2019_1_OR_NEWER
        protected EthForApianUnityHelper unityHelper;
#endif

        public bool IsConnected => web3 != null;
        public string CurrentAccountAddress => ethAccount?.Address;

        public static EthForApian Create()
        {
            return new EthForApian();
        }

        protected EthForApian()
        {
            ethSigner = new EthereumMessageSigner();

#if UNITY_2019_1_OR_NEWER
            unityHelper = EthForApianUnityHelper.Create();
#endif

        }

        // Connection
        public void Connect(string providerURL, IApianCryptoClient client=null)
        {
            callbackClient = client;
            web3 = new Web3(providerURL);
#if UNITY_2019_1_OR_NEWER
            unityHelper.SetupConnection(providerURL, client);
#endif
        }

        public void Connect(Object provider, IApianCryptoClient client = null)
        {
            callbackClient = client;
            web3 = provider as Web3;
#if UNITY_2019_1_OR_NEWER
            unityHelper.SetupConnection(providerURL, client);
#endif
        }

        public void Disconnect()
        {
            web3 = null;
            callbackClient = null;
#if UNITY_2019_1_OR_NEWER
            unityHelper.SetupConnection(null, null);
#endif
        }

        public void GetChainId()
        {
            if (callbackClient == null)
                throw new Exception("No IApianCryptoClient specified");
#if UNITY_2019_1_OR_NEWER
            unityHelper.DoGetChainId();
#elif !SINGLE_THREADED
            Task.Run( async () =>
                {
                    BigInteger bi =  await web3.Eth.ChainId.SendRequestAsync();
                    callbackClient.OnChainId((int)bi);
                });
#else
            #warning Single-threaded non-Unity GetChainId() not implmented
            throw new Exception("Single-threaded non-Unity GetChainId() not implmented");
#endif

        }

        public void GetBlockNumber()
        {
            if (callbackClient == null)
                throw new Exception("No IApianCryptoClient specified");
#if UNITY_2019_1_OR_NEWER
            unityHelper.DoGetBlockNumber();
#endif
        }

        public void GetBalance(string addr)
        {
            if (callbackClient == null)
                throw new Exception("No IApianCryptoClient specified");
#if UNITY_2019_1_OR_NEWER
            unityHelper.DoGetBalance(addr);
#endif
        }

#if !SINGLE_THREADED
        public async Task<int> GetChainIdAsync()
        {
            BigInteger bi =   await web3.Eth.ChainId.SendRequestAsync();
            return (int)bi;
        }

        public async Task<int> GetBlockNumberAsync()
        {
            BigInteger bi = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            return (int)bi;
        }

        public async Task<int> GetBalanceAsync(string addr)
        {
            BigInteger bi = await web3.Eth.GetBalance.SendRequestAsync(addr);
            return (int)bi;
        }

#endif


        // Account
        public string SetNewAccount()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            ethAccount = new Account(ecKey);
            return CurrentAccountAddress;
        }

        public string SetAccountFromKey(byte[] privateKeyBytes)
        {
            string pkStr = System.Text.Encoding.UTF8.GetString(privateKeyBytes);
            ethAccount = new Account(pkStr);
            return CurrentAccountAddress;
        }

        public string SetAccountFromJson(string password, string acctJson)
        {
            var keyStoreService = new Nethereum.KeyStore.KeyStoreScryptService();
            var key = keyStoreService.DecryptKeyStoreFromJson(password, acctJson);
            ethAccount = new Account(key);
            return CurrentAccountAddress;
        }

        public string JsonForCurrentAccount(string password)
        {
            var keyStoreService = new Nethereum.KeyStore.KeyStoreScryptService();
            var scryptParams = new Nethereum.KeyStore.Model.ScryptParams{Dklen = 32, N = 262144, R = 1, P = 8};
            var ecKey = new EthECKey(ethAccount.PrivateKey);
            var keyStore = keyStoreService.EncryptAndGenerateKeyStore(password, ecKey.GetPrivateKeyAsBytes(), ecKey.GetPublicAddress(), scryptParams);
            var json = keyStoreService.SerializeKeyStoreToJson(keyStore);
            return json;
        }

        public (string, string) JsonForNewAccount(string password)
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
    }





}