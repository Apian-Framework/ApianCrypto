

const ProxyFlags = {
    PROXY_PLAYER: 1, // acct is a player
    PROXY_VALIDATOR: 0, // acct is a validator  player/validator is exclusive
    PROXY_JOINED: 2, // acct is new in this epoch
    PROXY_LEFT: 4 // acct left during this epoch
};

function ApianSessionInfo(id, name, creator, type)
{
    this.id = id;
    this.name = name;
    this.creator = creator;
    this.apianGroupType = type;
};

function ApianEpoch(sessionId, epochNum, endApianTime, endCmdSeqNumber, endStateHash, proxyAddrs, proxyFlags, proxySigs)
{
    this.sessionId = sessionId;
    this.epochNum = epochNum;
    this.endApianTime = endApianTime;
    this.endCmdSeqNumber = endCmdSeqNumber;
    this.endStateHash = endStateHash;
    this.proxyAddrs = proxyAddrs;
    this.proxyFlags = proxyFlags;
    this.proxySigs = proxySigs;
};

const SessionAId = "ApricotSessionId";

const session_A_TestData =
{


    SessionInfo: new ApianSessionInfo(

        SessionAId, // virtual string Id { get; set; }
        "ApricotSessionName", //  public virtual string Name { get; set; }
        "0x1234567890", // public virtual string Creator { get; set; }
        "CreatorSez" // string ApianGroupType { get; set; }
    ),

    RegSessionParms:
    {
        epoch: 1, apianTime: 100, cmdNum: 0, stateHash: "sessAGenesisHash"
    },

    Epochs:
    [
        new ApianEpoch( SessionAId, 2, 200, 20, "sessAEp2Hash",
           ["0x1234", "0x2345"], [ProxyFlags.PROXY_JOINED|ProxyFlags.PROXY_PLAYER, ProxyFlags.PROXY_JOINED|ProxyFlags.PROXY_PLAYER],
           ["0x460f074934f1b5bee8d4c6ee83230f3ba1d9fb32043bd5b900e2519430607e44907919b233ec7ebc5372bc1a55b2f9245e90db1bcdb433aead99ab35a797ee1e1b",
            "0xe2bfa185c7400fa8c12bcb04ee330a0c8ede74583722141013c0e9994fd717c426d9c263c45148c9c41ad3d87be38ed9b6ecf4da7a1dfeff4e1090c1bbe3c2171c"] ),
        new ApianEpoch( SessionAId, 3, 300, 30, "sessAEp3Hash",
            ["0x1234", "0x2345"], [ProxyFlags.PROXY_PLAYER, ProxyFlags.PROXY_PLAYER],
            ["0xb59159b4592bae171f4e22af1c57a1c78792868c369d2f576d2a5987831112bfc1e8d18aa4774dca92eae569a0ae7e330cd7675d4d47a17d2fda77fbbacf9ff11c",
             "0x4943d9733f23a01fc87878d6a2668a638064f0f0cc5dd8b69d189c89024d6766cb88819f36b5f482839966956a1085e3378ce069a8714c5f8afec170812017721b"] ),
        new ApianEpoch( SessionAId, 4, 400, 40, "sessAEp4Hash",
            ["0x1234", "0x2345"], [ProxyFlags.PROXY_PLAYER|ProxyFlags.PROXY_LEFT, ProxyFlags.PROXY_PLAYER],
            ["0x00",
             "0xd10f4e2eb22f339a8a260a31f2224abb9167800ef1788c08ddaf2203bf209dacfdf5afa2fd5bdb4991bbb30d86a8d02465c942108d155900a7b33edceb824cd01c"] )
    ]
};

const session_B_TestData =
{
    SessionInfo: new ApianSessionInfo(

        "BananaSessionId", // virtual string Id { get; set; }
        "BananaSessionName", //  public virtual string Name { get; set; }
        "0x1234567890", // public virtual string Creator { get; set; }
        "LeaderSez" // string ApianGroupType { get; set; }
    ),

    RegSessionParms:
    {
        epoch: 1, apianTime: 300, cmdNum: 0, stateHash: "sessBGenesisHash"
    }

    // Epochs:
    // [
    //     { SessionId: this.ApianSessionInfo.Id, EpochNum:2, EndApianTime:200, EndCmdSeqNumber:20, EndStateHash:"sessAEp2Hash",
    //             ProxyAddrs:["0x1234", "0x2345"], ProxyFlags:[PROXY_JOINED|PROXY_PLAYER, PROXY_JOINED|PROXY_PLAYER], ProxySigs:["sAe2sig123","sAe2sig234"] },
    //     { SessionId:ApianSessionInfo.Id, EpochNum:3, EndApianTime:300, EndCmdSeqNumber:30, EndStateHash:"sessAEp3Hash",
    //             ProxyAddrs:["0x1234", "0x2345"], ProxyFlags:[PROXY_PLAYER, PROXY_PLAYER], ProxySigs:["sAe3sig123","sAe3sig234"] },
    //     { SessionId:ApianSessionInfo.Id, EpochNum:4, EndApianTime:400, EndCmdSeqNumber:40, EndStateHash:"sessAEp4Hash",
    //             ProxyAddrs:["0x1234", "0x2345"], ProxyFlags:[PROXY_PLAYER|PROXY_LEFT, PROXY_PLAYER], ProxySigs:["","sAe4sig234"] }

    // ]
};


module.exports = {
    ProxyFlags,
    ApianSessionInfo,
    ApianEpoch,
    session_A_TestData,
    session_B_TestData
};
