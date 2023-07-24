// SPDX-License-Identifier: UNLICENSED

// For gas testing lock this down as a single compiler version
pragma solidity ^0.8.18;

uint8 constant PROXY_PLAYER = 1; // acct is a player
uint8 constant PROXY_VALIDATOR = 0; // acct is a validator  player/validator is exclusive
uint8 constant PROXY_JOINED = 2; // acct is new in this epoch
uint8 constant PROXY_LEFT = 4; // acct left during this epoch

struct ApianSessionInfo {
    string id;
    string name;
    address creator; // is a proxy account
    string apianGroupType;
    string genesisHash;
}

struct ApianEpoch {
    string sessionId;
    uint256 epochNum;
    uint256 endApianTime;
    uint256 endCmdSeqNumber;
    string endStateHash;
    address[] proxyAddrs; // ascending addr order
    uint8[] proxyFlags; // same order
    bytes[] proxySigs; //  ''
}

struct ApianSession {
    ApianSessionInfo info;
    uint256[] epochNums; // so we can iterate over the mapping
    mapping( uint256 => ApianEpoch) epochMapByNum;
}

contract MinApianAnchor  {
    error SessionAlreadyRegistered(string sessionId, address creator);
    error SessionNotRegistered(string sessionId);
    error EpochAlreadyReported(string sessionId, uint256 epochNum);
    error EpochNotReported(string sessionId, uint256 epochNum);

    event SessionRegistered( string sessionId, address registeredBy); // Event
    event EpochReported( string sessionId, uint256 epochNum, address reportedBy); // Event

    string public constant version = "1.1.0";

    string[] sessionIds; // use to iterate over session map or check how many.
    mapping(string => ApianSession) sessionMapById;

    constructor() {}

    function registerSession(ApianSessionInfo calldata sessInfo ) external {

        ApianSession storage newSess = sessionMapById[sessInfo.id];
        if (newSess.info.creator != address(0)) {
            revert SessionAlreadyRegistered({sessionId: sessInfo.id, creator: sessInfo.creator});
        }
        sessionIds.push(sessInfo.id); // new one

        ApianSessionInfo memory newSessInfo = ApianSessionInfo({
            id: sessInfo.id,
            name: sessInfo.name,
            creator: sessInfo.creator,
            apianGroupType: sessInfo.apianGroupType,
            genesisHash: sessInfo.genesisHash
        });
        newSess.info = newSessInfo;

        emit SessionRegistered(sessInfo.id, msg.sender); // Event
    }

    function reportEpoch(ApianEpoch calldata epoch) external {

        // fetch or create session
        ApianSession storage sess = sessionMapById[epoch.sessionId];
        if (sess.info.creator == address(0)) {
            revert SessionNotRegistered({sessionId: epoch.sessionId});
        }

        if (bytes(sess.epochMapByNum[epoch.epochNum ].sessionId).length != 0) {  // enpty storage string test: bytes(str).length == 0
            revert EpochAlreadyReported({sessionId: epoch.sessionId, epochNum: epoch.epochNum});
        }

        //  verify signatures?
        //      Note: or not - maybe we just log the signatures unless someone expressly asks for verification.

        // Post report
        sess.epochNums.push(epoch.epochNum);
        sess.epochMapByNum[epoch.epochNum] = epoch;

        emit EpochReported(epoch.sessionId, epoch.epochNum, msg.sender); // Event
    }

    function getSessionCount() external view returns (uint256)
    {
        return sessionIds.length;
    }

    function getSessionIds() external view returns (string[] memory)
    {
        return sessionIds;
    }

    function getSessionInfo(string calldata sessionId)
        external view
        returns (ApianSessionInfo memory info, uint256[] memory epochNums)
    {
        ApianSession storage sess = sessionMapById[sessionId];
        if (sess.info.creator == address(0)) {
            revert SessionNotRegistered({sessionId: sessionId});
        }
        return ( sess.info, sess.epochNums );
     }

    function getSessionEpoch(string calldata sessionId, uint256 epochNum)
        external view
        returns (ApianEpoch memory epoch)
    {
        ApianSession storage sess = sessionMapById[sessionId];
        if (sess.info.creator == address(0)) {
            revert SessionNotRegistered({sessionId: sessionId});
        }

        epoch = sess.epochMapByNum[epochNum];
        if (bytes(sess.epochMapByNum[epoch.epochNum ].sessionId).length == 0) {  // empty storage string test: bytes(str).length == 0
            revert EpochNotReported({sessionId: sessionId, epochNum: epochNum});
        }
        return (epoch);
     }

}
