using System;
using System.Collections;
using System.Numerics;
using System.Text;
using UniLog;

﻿using Nethereum.Contracts;

using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Eth.DTOs; // for BlockParameter

using Newtonsoft.Json;

using ApianAnchor.Contracts.MinApianAnchor;
using ApianAnchor.Contracts.MinApianAnchor.ContractDefinition;

#if UNITY_2019_1_OR_NEWER

using UnityEngine;
using Nethereum.Unity.Rpc;


namespace ApianCrypto
{
    // This is a Unity-managed class (MonoBehavior-derived)
    // that implments coroutines to allow asynchronous call in single-threaded (WebGL)
    // code.
    // These all end up calling IApianCryptoClient callbacks.

    public class EthForApianUnityHelper : MonoBehaviour
    {
        public static EthForApianUnityHelper Create()
        {
            GameObject go = GameObject.Find("CoroutineHelper"); // needs to be in the scene
            go.AddComponent<EthForApianUnityHelper>();
            return go.GetComponent<EthForApianUnityHelper>();
        }


        public IApianCryptoClient CallbackClient { get; private set; }
        public string ProviderURL { get; private set; }

        protected Account ethAccount {get; private set;}

        protected UniLogger Logger;

        public  EthForApianUnityHelper()
        {
            Logger = new UniLogger("EthForApianUnityHelper");
        }

        public void SetupConnection(string providerURL, Account account, IApianCryptoClient client )
        {
            CallbackClient = client;
            ProviderURL = providerURL;
            ethAccount = account;
        }

        public void DoGetChainId()
        {
            StartCoroutine(GetChainIdCoRo());
        }

        public IEnumerator GetChainIdCoRo()
        {
            var chainIdReq = new EthChainIdUnityRequest(ProviderURL);
            yield return chainIdReq.SendRequest();

            if (chainIdReq.Exception != null) {
                UnityEngine.Debug.Log(chainIdReq.Exception.Message);
            }
            CallbackClient.OnChainId((int)chainIdReq.Result.Value);
        }

        public void DoGetBlockNumber()
        {
            StartCoroutine(GetBlockNumberCoRo());
        }

        public IEnumerator GetBlockNumberCoRo()
        {
            var blockNumReq = new EthBlockNumberUnityRequest(ProviderURL);
            yield return blockNumReq.SendRequest();

            if (blockNumReq.Exception != null)
                UnityEngine.Debug.Log(blockNumReq.Exception.Message);

            CallbackClient.OnBlockNumber((int)blockNumReq.Result.Value);
        }

        public void DoGetBalance(string acct)
        {
            StartCoroutine(GetBalanceCoRo(acct));
        }

        public IEnumerator GetBalanceCoRo(string acct)
        {
            var balanceRequest = new EthGetBalanceUnityRequest(ProviderURL);
            yield return balanceRequest.SendRequest(acct, BlockParameter.CreateLatest());

            if (balanceRequest.Exception != null)
                UnityEngine.Debug.Log(balanceRequest.Exception.Message);

            CallbackClient.OnBalance( acct, (int)balanceRequest.Result.Value);
        }

        public void DoRegisterSession(string contractAddr, AnchorSessionInfo sessInfo)
        {
            StartCoroutine(RegisterSessionCoRo(contractAddr, sessInfo));
        }

        public IEnumerator RegisterSessionCoRo(string contractAddr, AnchorSessionInfo sessInfo)
        {
            var transactionRequest = new TransactionSignedUnityRequest(ProviderURL, ethAccount.PrivateKey);

            transactionRequest.UseLegacyAsDefault = true;

            var transactionMessage = new RegisterSessionFunction
            {
                 SessInfo = ApianSessionInfo.FromApian(sessInfo)
            };

            yield return transactionRequest.SignAndSendTransaction(transactionMessage, contractAddr);

            var transactionHash = transactionRequest.Result;

            Logger.Verbose("RegisterSession txn hash:" + transactionHash);

            CallbackClient.OnSessionRegistered(sessInfo.Id, transactionHash);
        }


    }
}

#endif
