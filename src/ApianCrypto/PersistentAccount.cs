using System;
using Newtonsoft.Json;

namespace ApianCrypto
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PersistentAccount
    {
        // Mostly these contain encrypted Ethereum v3 keystores, but for testing there neesd to be some way
        // to use non-secure local-private-key accounts, too
        public enum AvailTypes { V3Keystore = 1, ClearPrivKey = 2}; // zero is invalid
        [JsonProperty]
        public AvailTypes Type { get; private set; }
        [JsonProperty]
        public string Address  { get; private set; }
        [JsonProperty]
        public string Data { get; private set; }  // either just clear text or json

        public PersistentAccount(AvailTypes type, string address, string data)   {
            Type = type;
            Address = address;
            Data = data;
        }

        public string ToJson() =>  JsonConvert.SerializeObject(this);
        public static PersistentAccount FromJson(string jsonString) => JsonConvert.DeserializeObject<PersistentAccount>(jsonString);

    }

}