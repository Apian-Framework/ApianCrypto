using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;
using Nethereum.XUnitEthereumClients;

using ApianCrypto;

namespace ApianCryptoTests
{
    public class TestFixtureBase
    {
        protected EthereumClientIntegrationFixture _ethereumClientIntegrationFixture  = new EthereumClientIntegrationFixture();
    }


    [TestFixture]
    public class EthForApianTests : TestFixtureBase
    {
        [Test]
        public void SignAndRecoverMessage()
        {

            IApianCrypto cryptoThing = EthForApian.Create();

            string addr = cryptoThing.SetNewAccount();
            Assert.NotNull(addr, "Null address");

            string msg = "Ya Ya! Ya Ya ya.";
            string sig = cryptoThing.EncodeUTF8AndSign(addr, msg);

            string recAddr = cryptoThing.EncodeUTF8AndEcRecover(msg, sig);

            Assert.AreEqual(recAddr,addr, $"Recovered address does not match original address");
        }

        [Test]
        public async Task ConnectToChain()
        {
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();
            IApianCrypto cryptoThing = EthForApian.Create();

            cryptoThing.Connect(web3);

            int chainId = await cryptoThing.GetChainIdAsync();

            Assert.AreEqual(chainId, 1);

        }


    //     [Test]
    //     public void P2pNetChannelPeer_NoSync_Ctor()
    //     {
    //         P2pNetPeer peer = new P2pNetPeer(defLocalPeerId);

    //         // public P2pNetChannelPeer(P2pNetPeer peer, P2pNetChannel channel)
    //         P2pNetChannelPeer chp = new P2pNetChannelPeer(peer, CreateChannel(chInfoTracking()));
    //         Assert.That(chp.HaveHeardFrom, Is.False);
    //         Assert.That(chp.HasTimedOut, Is.False);
    //         Assert.That(chp.IsMissing, Is.False);
    //         Assert.That(chp.ClockNeedsSync, Is.False); // rest was checked above
    //     }

    //     [Test]
    //     public void P2pNetChannelPeer_NoTracking_Ctor()
    //     {
    //         P2pNetPeer peer = new P2pNetPeer(defLocalPeerId);

    //         // public P2pNetChannelPeer(P2pNetPeer peer, P2pNetChannel channel)
    //         P2pNetChannelPeer chp = new P2pNetChannelPeer(peer, CreateChannel(chInfoNoTracking()));

    //         Assert.That(chp.HasTimedOut, Is.False);
    //         Assert.That(chp.WeShouldSendHello, Is.False);
    //         Assert.That(chp.NeedsPing, Is.False);
    //         Assert.That(chp.ClockNeedsSync, Is.False);
    //     }

    // }

    // [TestFixture]
    // public class P2pNetChannelPeerCollectionTests : TestFixtureBase
    // {
    //     [Test]
    //     public void P2pNetChannelPeerCollection_ConstructorWorks()
    //     {
    //         // public P2pNetChannelPeer(P2pNetPeer peer, P2pNetChannel channel)
    //         P2pNetChannelPeerPairings coll = new P2pNetChannelPeerPairings();
    //         Assert.That(coll, Is.Not.Null);
    //         Assert.That(coll.Channels, Is.Not.Null);
    //         Assert.That(coll.PeersById, Is.Not.Null);
    //         Assert.That(coll.PeersByAddress, Is.Not.Null);
    //         Assert.That(coll.ChannelPeers, Is.Not.Null);
    //     }

    //     [Test]
    //     public void CPC_AddMainChannel()
    //     {
    //         P2pNetChannelInfo chInfo = chInfoTracking();
    //         P2pNetChannel mainChan = CreateChannel(chInfo); // defaultPeerData

    //         // public P2pNetChannelPeer(P2pNetPeer peer, P2pNetChannel channel)
    //         P2pNetChannelPeerPairings coll = new P2pNetChannelPeerPairings();
    //         Assert.That(coll, Is.Not.Null);

    //         coll.SetMainChannel(mainChan);
    //         Assert.That(coll.MainChannel.Info, Is.EqualTo(chInfo));
    //         Assert.That(coll.MainChannel.LocalHelloData, Is.EqualTo(defLocalHelloData));

    //     }

    }


}