const {
  time,
  loadFixture,
} = require("@nomicfoundation/hardhat-toolbox/network-helpers");
const { anyValue } = require("@nomicfoundation/hardhat-chai-matchers/withArgs");
const { expect } = require("chai");

const { testPrivKeys } = require("./TestPrivKeys.js");
const { ProxyFlags,
  ApianSessionInfo,
  ApianEpoch,
  session_A_TestData: sda,
  session_B_TestData: sdb
} = require("./TestSessionData.js");

// Need this to be able to serialice a BigInt or anything that contains one
Object.defineProperties(BigInt.prototype, {
  toJSON: {
    value: function () {
      return this.toString();
    },
  },
});


describe("MinApianAnchor Tests", function () {

  async function setupContractsAndAccts() {

    const fundedAccounts = await ethers.getSigners();
    const emptyAccounts = [];
    testPrivKeys.forEach((privKey) => {
      emptyAccounts.push( new ethers.Wallet(privKey, ethers.provider));
    });

    // deploy contract
    const ApianAnchorFactory = await ethers.getContractFactory("MinApianAnchor");
    const apianAnchor = await ApianAnchorFactory.deploy();

    // Fixtures can return anything you consider useful for your tests
    return { apianAnchor, fundedAccounts, emptyAccounts };
  }


  describe("Deployment", function () {

    it("Should deploy without error", async function () {
      const { apianAnchor, fundedAccounts, emptyAccounts }  = await loadFixture(setupContractsAndAccts);

      expect(await apianAnchor.getSessionCount()).to.equal(0, "Session count should be 0");
      expect(await apianAnchor.getSessionIds()).to.deep.equal(Array(), "Session ids should be empty array");

    });

  });

  describe("Session Registration", function () {

    it("Should register a session", async function () {

      const { apianAnchor, fundedAccounts, emptyAccounts }  = await loadFixture(setupContractsAndAccts);

       // function registerSession(
      //     ApianSessionInfo calldata sessInfo,
      //     uint64 epochNum, // genesis epoch data (probably zeroes except for state hash)
      //     uint64 apianTime,
      //     uint64 cmdSeqNumber,
      //     string calldata stateHash
      // )
      var reporterAcct = fundedAccounts[1];
      var creatorAcct = emptyAccounts[0];
      var rsp = sda.RegSessionParms;

      sda.SessionInfo.creator = creatorAcct.address;

      expect( ethers.isAddress(sda.SessionInfo.creator) ).to.equal(true, "Creator should be a valid address");

      await expect( apianAnchor.connect(reporterAcct).registerSession( sda.SessionInfo, rsp.epoch, rsp.apianTime, rsp.cmdNum, rsp.stateHash))
        .to.emit(apianAnchor, "SessionRegistered")
        .withArgs( sda.SessionInfo.id , reporterAcct.address);

      expect(await apianAnchor.getSessionCount()).to.equal(1, "Session count should be 1");
      expect(await apianAnchor.getSessionIds()).to.deep.equal([sda.SessionInfo.id], "Session ids should have one entry");

      // getSessionInfo(tring sessionId)
       var gsiRes = await apianAnchor.getSessionInfo(sda.SessionInfo.id);

       //console.log(JSON.stringify(gsiRes[0]));

      var fetchedSessInfo = new ApianSessionInfo(...gsiRes[0]);
      var fetchedEpochIds = gsiRes[1];

      expect(fetchedSessInfo).to.deep.equal(sda.SessionInfo, "Fetched session info should match")
      expect(fetchedEpochIds.length).to.equal(1, "Epoch count should be 1");

      // getSessionEpoch(string sessionId, uint256 epochNum)
      var gseRes = await apianAnchor.getSessionEpoch(sda.SessionInfo.id, rsp.epoch);
      var fetchedEpoch = new ApianEpoch(...gseRes);
      var testEpoch = new ApianEpoch(sda.SessionInfo.id, rsp.epoch, rsp.apianTime, rsp.cmdNum,
        rsp.stateHash, [], [], []);

      expect(fetchedEpoch).to.deep.equal(testEpoch, "Fetched epoch info should match")

    });

    it("Should register 2 sessions", async function () {

      const { apianAnchor, fundedAccounts, emptyAccounts }  = await loadFixture(setupContractsAndAccts);

      var reporterAcctA = fundedAccounts[1];
      var reporterAcctB = fundedAccounts[2];

      var creatorAcctA = emptyAccounts[0];
      var creatorAcctB = emptyAccounts[1];

      var rspA = sda.RegSessionParms;
      sda.SessionInfo.creator = creatorAcctA.address;

      var rspB = sdb.RegSessionParms;
      sdb.SessionInfo.creator = creatorAcctB.address;

      expect( ethers.isAddress(sda.SessionInfo.creator) ).to.equal(true, "Creator A should be a valid address");
      expect( ethers.isAddress(sdb.SessionInfo.creator) ).to.equal(true, "Creator B should be a valid address");

      await apianAnchor.connect(reporterAcctA).registerSession( sda.SessionInfo, rspA.epoch, rspA.apianTime, rspA.cmdNum, rspA.stateHash);
      await apianAnchor.connect(reporterAcctB).registerSession( sdb.SessionInfo, rspB.epoch, rspB.apianTime, rspB.cmdNum, rspB.stateHash);

      expect(await apianAnchor.getSessionCount()).to.equal(2, "Session count should be 2");
      expect(await apianAnchor.getSessionIds()).to.deep.equal([sda.SessionInfo.id, sdb.SessionInfo.id], "Session ids should have two entries");

    });

    it("Should revert on registering duplicate sessions", async function () {

      const { apianAnchor, fundedAccounts, emptyAccounts }  = await loadFixture(setupContractsAndAccts);

      var reporterAcctA = fundedAccounts[1];
      var reporterAcctB = fundedAccounts[2];

      var creatorAcctA = emptyAccounts[0];

      var rspA = sda.RegSessionParms;
      sda.SessionInfo.creator = creatorAcctA.address;

      await apianAnchor.connect(reporterAcctA).registerSession( sda.SessionInfo, rspA.epoch, rspA.apianTime, rspA.cmdNum, rspA.stateHash);

      await expect( apianAnchor.connect(reporterAcctB)
        .registerSession( sda.SessionInfo, rspA.epoch, rspA.apianTime, rspA.cmdNum, rspA.stateHash) )
        .to.be.revertedWithCustomError(apianAnchor, "SessionAlreadyRegistered")
        .withArgs( sda.SessionInfo.id , creatorAcctA.address);
    });

  });   // Session registration tests

  describe("Epoch Reporting", function () {

    it("Should report some epochs", async function () {

      const { apianAnchor, fundedAccounts, emptyAccounts }  = await loadFixture(setupContractsAndAccts);

      var reporterAcctA = fundedAccounts[1];
      var reporterAcctB = fundedAccounts[2];

      var creatorAcctA = emptyAccounts[0];

      var rspA = sda.RegSessionParms;
      sda.SessionInfo.creator = creatorAcctA.address; // insert real address

      sda.Epochs[0].proxyAddrs = [reporterAcctB.address, reporterAcctA.address];
      sda.Epochs[1].proxyAddrs = [reporterAcctB.address, reporterAcctA.address];
      sda.Epochs[2].proxyAddrs = [reporterAcctB.address, reporterAcctA.address];

      await apianAnchor.connect(reporterAcctA).registerSession( sda.SessionInfo, rspA.epoch, rspA.apianTime, rspA.cmdNum, rspA.stateHash);

      await apianAnchor.connect(reporterAcctB).reportEpoch( sda.Epochs[0]);
      await apianAnchor.connect(reporterAcctA).reportEpoch( sda.Epochs[1]);
      await apianAnchor.connect(reporterAcctB).reportEpoch( sda.Epochs[2]);

      var gsiRes = await apianAnchor.getSessionInfo(sda.SessionInfo.id);

       //console.log(JSON.stringify(gsiRes[0]));

      var fetchedSessInfo = new ApianSessionInfo(...gsiRes[0]);
      var fetchedEpochIds = gsiRes[1];

      //console.log(fetchedEpochIds);
      expect(fetchedEpochIds).to.deep.equal([rspA.epoch,sda.Epochs[0].epochNum,sda.Epochs[1].epochNum,sda.Epochs[2].epochNum]);

      var testEpochNum = sda.Epochs[1].epochNum;

      var gseRes = await apianAnchor.getSessionEpoch(sda.SessionInfo.id,testEpochNum);
      var fetchedEpoch = new ApianEpoch(...gseRes);

      expect(fetchedEpoch).to.deep.equal(sda.Epochs[1], "Fetched epoch info should match")

    });

    it("Should revert if reported epoch's session doesn't exist", async function () {

      const { apianAnchor, fundedAccounts, emptyAccounts }  = await loadFixture(setupContractsAndAccts);

      var reporterAcctA = fundedAccounts[1];
      var reporterAcctB = fundedAccounts[2];

      var creatorAcctA = emptyAccounts[0];

      var rspA = sda.RegSessionParms;

      sda.Epochs[0].proxyAddrs = [reporterAcctB.address, reporterAcctA.address];

      await expect( apianAnchor.connect(reporterAcctB)
        .reportEpoch( sda.Epochs[0]) )
        .to.be.revertedWithCustomError(apianAnchor, "SessionNotRegistered")
        .withArgs( sda.SessionInfo.id );

    });

    it("Should revert on duplicate epoch reports", async function () {

      const { apianAnchor, fundedAccounts, emptyAccounts }  = await loadFixture(setupContractsAndAccts);

      var reporterAcctA = fundedAccounts[1];
      var reporterAcctB = fundedAccounts[2];

      var creatorAcctA = emptyAccounts[0];

      var rspA = sda.RegSessionParms;
      sda.SessionInfo.creator = creatorAcctA.address; // insert real address

      sda.Epochs[0].proxyAddrs = [reporterAcctB.address, reporterAcctA.address];
      sda.Epochs[1].proxyAddrs = [reporterAcctB.address, reporterAcctA.address];
      sda.Epochs[2].proxyAddrs = [reporterAcctB.address, reporterAcctA.address];

      await apianAnchor.connect(reporterAcctA).registerSession( sda.SessionInfo, rspA.epoch, rspA.apianTime, rspA.cmdNum, rspA.stateHash);

      await apianAnchor.connect(reporterAcctB).reportEpoch( sda.Epochs[0]);

      await expect( apianAnchor.connect(reporterAcctB)
        .reportEpoch( sda.Epochs[0]) )
        .to.be.revertedWithCustomError(apianAnchor, "EpochAlreadyReported")
        .withArgs( sda.SessionInfo.id,  sda.Epochs[0].epochNum );
    });
  });   // Epoch reports tests

  describe("Information Getter failures", function () {

    it("getSessionInfo() should revert on missing session", async function () {
      const { apianAnchor, fundedAccounts, emptyAccounts }  = await loadFixture(setupContractsAndAccts);
      await expect( apianAnchor.getSessionInfo(sda.SessionInfo.id) )
      .to.be.revertedWithCustomError(apianAnchor, "SessionNotRegistered")
      .withArgs( sda.SessionInfo.id );
    });

    it("getSessionEpoch() should revert on missing session", async function () {
      const { apianAnchor, fundedAccounts, emptyAccounts }  = await loadFixture(setupContractsAndAccts);

      await expect( apianAnchor.getSessionEpoch(sda.SessionInfo.id, sda.Epochs[1].epochNum))
      .to.be.revertedWithCustomError(apianAnchor, "SessionNotRegistered")
      .withArgs( sda.SessionInfo.id );
    });

    it("getSessionEpoch() should revert on missing epoch", async function () {
      const { apianAnchor, fundedAccounts, emptyAccounts }  = await loadFixture(setupContractsAndAccts);
      var reporterAcctA = fundedAccounts[1];
      var creatorAcctA = emptyAccounts[0];
      var rspA = sda.RegSessionParms;
      sda.SessionInfo.creator = creatorAcctA.address; // insert real address

      await apianAnchor.connect(reporterAcctA).registerSession( sda.SessionInfo, rspA.epoch, rspA.apianTime, rspA.cmdNum, rspA.stateHash);

      await expect( apianAnchor.getSessionEpoch(sda.SessionInfo.id, sda.Epochs[1].epochNum))
      .to.be.revertedWithCustomError(apianAnchor, "EpochNotReported")
      .withArgs( sda.SessionInfo.id, sda.Epochs[1].epochNum );
    });

  });

});
