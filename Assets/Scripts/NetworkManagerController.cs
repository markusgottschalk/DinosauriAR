//-----------------------------------------------------------------------
// <copyright file="NetworkManagerUIController.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//THIS SCRIPT WAS MODIFIED.
//-----------------------------------------------------------------------



using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

/// <summary>
/// Controller managing the network.
/// </summary>
#pragma warning disable 618
[RequireComponent(typeof(NetworkManager))]
public class NetworkManagerController : MonoBehaviour
#pragma warning restore 618
{
    public UIController UIController;

    /// <summary>
    /// The Cloud Anchors Example Controller.
    /// </summary>
    public ARCoreController ARCoreController;

    /// <summary>
    /// The number of matches that will be shown.
    /// </summary>
    private const int k_MatchPageSize = 5;

    /// <summary>
    /// The Network Manager.
    /// </summary>
#pragma warning disable 618
    private MainNetworkManager MainNetworkManager;
#pragma warning restore 618

    /// <summary>
    /// The Game Manager.
    /// </summary>
    public GameManager GameManager;

    /// <summary>
    /// A tuple describing the hosted room with network ID.
    /// </summary>
    private (bool, NetworkID) roomHosted = (false, NetworkID.Invalid);

    /// <summary>
    /// The Unity Awake() method.
    /// </summary>
    public void Awake()
    {
#pragma warning disable 618
        MainNetworkManager = GetComponent<MainNetworkManager>();
#pragma warning restore 618

        //Start the Matchmaker and list all available matches
        MainNetworkManager.StartMatchMaker();
        MainNetworkManager.matchMaker.ListMatches(
            startPageNumber: 0,
            resultPageSize: k_MatchPageSize,
            matchNameFilter: string.Empty,
            filterOutPrivateMatchesFromResults: false,
            eloScoreTarget: 0,
            requestDomain: 0,
#pragma warning disable 618
            callback: (bool success, string extendedInfo, List<MatchInfoSnapshot> matches) => { _OnMatchList(success, extendedInfo, matches, string.Empty); }
#pragma warning restore 618
            );
    }

    private void Start()
    {
        MainNetworkManager.OnClientConnected += ARCoreController.OnConnectedToServer;
        MainNetworkManager.OnClientDisconnected += UIController.OnDisconnectedFromServer;
    }


    /// <summary>
    /// Handles the user intent to create a new room with the name of the expedition and the name of the host so it can be shown on the list of available rooms.
    /// </summary>
    /// <param name="matchName">The name of the expedition</param>
    public void CreateRoom(string matchName)
    {
        string matchANDhostName = matchName + "-" + GameManager.PlayerName;
        MainNetworkManager.matchMaker.CreateMatch(matchANDhostName, MainNetworkManager.matchSize,
                                        true, string.Empty, string.Empty, string.Empty,
                                        0, 0,
#pragma warning disable 618
                                        (bool success, string extendedInfo, MatchInfo matchInfo) => { _OnMatchCreate(success, extendedInfo, matchInfo, matchANDhostName); }
#pragma warning restore 618
                                        );
    }


    /// <summary>
    /// Callback that happens when a <see cref="NetworkMatch.CreateMatch"/> request has been
    /// processed on the server.
    /// </summary>
    /// <param name="success">Indicates if the request succeeded.</param>
    /// <param name="extendedInfo">A text description for the error if success is false.</param>
    /// <param name="matchInfo">The information about the newly created match.</param>
    /// <param name="matchANDhostName">The name of the expedition and the name of the host.</param>
#pragma warning disable 618
    private void _OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo, string matchANDhostName)
#pragma warning restore 618
    {
        string matchName = matchANDhostName.Split('-')[0];
        MainNetworkManager.OnMatchCreate(success, extendedInfo, matchInfo);
        UIController.OnMatchCreate(success, extendedInfo, matchANDhostName);

        if (success)
        {
            roomHosted = (true, matchInfo.networkId);
            GameManager.StartExpedition(matchName);
        }
    }

    public bool IsRoomHosted()
    {
        return roomHosted.Item1;
    }

    /// <summary>
    /// When a room is destroyed (e.g. by clicking on the back button while an own room is open).
    /// </summary>
    public void QuitMatch()
    {
        if (!roomHosted.Item1)
        {
            //Debug.Log("This is not your room. You can't destroy it.");
            //_OnMatchDestroy(false, "This is not your room.");
            return;
        }

        MainNetworkManager.matchMaker.DestroyMatch(roomHosted.Item2, 0, _OnMatchDestroy);
    }

    private void _OnMatchDestroy(bool success, string extendedInfo)
    {
        MainNetworkManager.OnDestroyMatch(success, extendedInfo);
        UIController.OnMatchDestroy(success, extendedInfo);
        if (success)
        {
            roomHosted = (false, NetworkID.Invalid);
        }
    }


    /// <summary>
    /// Handles the user intent to refresh the room list. The matchNameFilter is either <paramref name="expeditionName"/> or string.Empty. But the filter is a partial wildcard so it will find all matches containing the whole string (even when the matchName is "expeditionName-hostName").
    /// </summary>
    /// <param name="expeditionName">Refreshes specific expedition lists. String.empty refreshes whole list.</param>
    public void RefreshRoomList(string expeditionName)
    {
        MainNetworkManager.matchMaker.ListMatches(
            startPageNumber: 0,
            resultPageSize: k_MatchPageSize,
            matchNameFilter: expeditionName,
            filterOutPrivateMatchesFromResults: false,
            eloScoreTarget: 0,
            requestDomain: 0,
#pragma warning disable 618
            callback: (bool success, string extendedInfo, List<MatchInfoSnapshot> matches) => { _OnMatchList(success, extendedInfo, matches, expeditionName); }
#pragma warning restore 618
            );
    }

    /// <summary>
    /// Callback that happens when a <see cref="NetworkMatch.ListMatches"/> request has been
    /// processed on the server.
    /// </summary>
    /// <param name="success">Indicates if the request succeeded.</param>
    /// <param name="extendedInfo">A text description for the error if success is false.</param>
    /// <param name="matches">A list of matches corresponding to the filters set in the initial
    /// list request.</param>
    /// <param name="expeditionNameSearchedFor">The original match filter.</param>
#pragma warning disable 618
    private void _OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches, string expeditionNameSearchedFor)
#pragma warning restore 618
    {
        MainNetworkManager.OnMatchList(success, extendedInfo, matches);
        UIController.OnMatchList(success, extendedInfo, expeditionNameSearchedFor, matches);
    }

    /// <summary>
    /// Handles the user intent to join the room associated with the button clicked.
    /// </summary>
    /// <param name="match">The information about the match that the user intents to
    /// join.</param>
#pragma warning disable 618
    public void OnJoinRoomClicked(MatchInfoSnapshot match)
#pragma warning restore 618
    {
        MainNetworkManager.matchName = match.name.Split('-')[0];
        MainNetworkManager.matchMaker.JoinMatch(match.networkId, string.Empty, string.Empty,
                                        string.Empty, 0, 0, _OnMatchJoined);
    }

    /// <summary>
    /// Callback that happens when a <see cref="NetworkMatch.JoinMatch"/> request has been
    /// processed on the server.
    /// </summary>
    /// <param name="success">Indicates if the request succeeded.</param>
    /// <param name="extendedInfo">A text description for the error if success is false.</param>
    /// <param name="matchInfo">The info for the newly joined match.</param>
#pragma warning disable 618
    private void _OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
#pragma warning restore 618
    {
        MainNetworkManager.OnMatchJoined(success, extendedInfo, matchInfo);
        UIController.OnMatchJoined(success, extendedInfo);

        if (success)
        {
            GameManager.JoinExpedition(MainNetworkManager.matchName);
        }
    }

    public void StartMatchMaker()
    {
        MainNetworkManager.StartMatchMaker();
    }
}

