using System;
using UniLog;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Signer;

namespace ApianCrypto
{
    public class EthForApian : IApianCrypto
    {

        protected Web3 web3;
        protected Account ethAccount;

        public bool IsConnected => web3 != null;
        public string AccountAddress => ethAccount.Address;

        public static EthForApian Create()
        {
            return new EthForApian();
        }

        protected EthForApian()
        {
        }

        // Connection
        public void Connect(string providerURL)
        {
            web3 = new Web3(providerURL);
        }


        // Account
        public string CreateAccount()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            ethAccount = new Account(ecKey);
            return AccountAddress;
        }

        public string CreateAccountForKey(byte[] privateKeyBytes)
        {
            string pkStr = System.Text.Encoding.UTF8.GetString(privateKeyBytes);
            ethAccount = new Account(pkStr);
            return AccountAddress;
        }

        public string CreateAccountFromJson(string password, string acctJson)
        {
            var keyStoreService = new Nethereum.KeyStore.KeyStoreScryptService();
            var key = keyStoreService.DecryptKeyStoreFromJson(password, acctJson);
            ethAccount = new Account(key);
            return AccountAddress;
        }

        public string GetJsonForAccount(string password)
        {
            var keyStoreService = new Nethereum.KeyStore.KeyStoreScryptService();
            var scryptParams = new Nethereum.KeyStore.Model.ScryptParams{Dklen = 32, N = 262144, R = 1, P = 8};
            var ecKey = new EthECKey(ethAccount.PrivateKey);
            var keyStore = keyStoreService.EncryptAndGenerateKeyStore(password, ecKey.GetPrivateKeyAsBytes(), ecKey.GetPublicAddress(), scryptParams);
            var json = keyStoreService.SerializeKeyStoreToJson(keyStore);
            return json;
        }

        // Sign/recover text message
        public string EncodeUTF8AndSign(string msg)
        {
            var  signer = new EthereumMessageSigner(); // TODO: Maybe create this when account is created and keep it?
            return signer.EncodeUTF8AndSign(msg,  new EthECKey(ethAccount.PrivateKey));

        }
        public string EncodeUTF8AndEcRecover(string msg, string signature)
        {
            var  signer = new EthereumMessageSigner();
            return signer.EncodeUTF8AndEcRecover(msg, signature);
        }



    }

}