using System;

namespace ApianCrypto
{
    // TODO: This might ought to go away if there doesn't turn out to be any implmentation-independent
    // common code

    public abstract class ApianCryptoBase : IApianCrypto
    {
        public abstract bool IsConnected {get;}
        public abstract void Connect(string provider);
        public abstract string AccountAddress {get; }
        public abstract string CreateAccount();
        public abstract string CreateAccountForKey(byte[] privateKeyBytes);
        public abstract string CreateAccountFromJson(string password, string acctJson);
        public abstract string GetJsonForAccount(string password);
        public abstract string EncodeUTF8AndSign(string msg);
        public abstract string EncodeUTF8AndEcRecover(string message, string signature);
    }
}