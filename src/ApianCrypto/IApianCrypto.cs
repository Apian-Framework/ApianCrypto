using System;

namespace ApianCrypto
{
    public interface IApianCrypto
    {
        bool IsConnected {get;}
        void Connect(string provider); // probably a url

        string AccountAddress {get; }
        string CreateAccount();
        string CreateAccountForKey(byte[] privateKeyBytes);
        string CreateAccountFromJson(string password, string acctJson);
        string GetJsonForAccount(string password);

        string EncodeUTF8AndSign(string msg);
        string EncodeUTF8AndEcRecover(string message, string signature);

    }

}