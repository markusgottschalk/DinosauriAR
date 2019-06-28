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



using GoogleARCore.Examples.CloudAnchors;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

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
    /// The Lobby Screen to see Available Rooms or create a new one.
    /// </summary>
    //public Canvas LobbyScreen;

    /// <summary>
    /// The snackbar text.
    /// </summary>
    //public Text SnackbarText;

    /// <summary>
    /// The Label showing the current active room.
    /// </summary>
    //public GameObject CurrentRoomLabel;

    /// <summary>
    /// The Cloud Anchors Example Controller.
    /// </summary>
    public ARCoreController ARCoreController;

    /// <summary>
    /// The Panel containing the list of available rooms to join.
    /// </summary>
    //public GameObject RoomListPanel;

    /// <summary>
    /// Text indicating that no previous rooms exist.
    /// </summary>
    //public Text NoPreviousRoomsText;

    /// <summary>
    /// The prefab for a row in the available rooms list.
    /// </summary>
    //public GameObject JoinRoomListRowPrefab;

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

    public GameManager GameManager;

    /// <summary>
    /// The current room number.
    /// </summary>
    //private string m_CurrentRoomNumber;

    /// <summary>
    /// The Join Room buttons.
    /// </summary>
    //private List<GameObject> m_JoinRoomButtonsPool = new List<GameObject>();
    private (bool, NetworkID) roomHosted = (false, NetworkID.Invalid);

    /// <summary>
    /// The Unity Awake() method.
    /// </summary>
    public void Awake()
    {
        // Initialize the pool of Join Room buttons.
        //for (int i = 0; i < k_MatchPageSize; i++)
        //{
        //    GameObject button = Instantiate(JoinRoomListRowPrefab);
        //    button.transform.SetParent(RoomListPanel.transform, false);
        //    button.GetComponent<RectTransform>().anchoredPosition =
        //        new Vector2(0, -100 - (200 * i));
        //    button.SetActive(true);
        //    button.GetComponentInChildren<Text>().text = string.Empty;
        //    m_JoinRoomButtonsPool.Add(button);
        //}

#pragma warning disable 618
        MainNetworkManager = GetComponent<MainNetworkManager>();
#pragma warning restore 618


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
        //_ChangeLobbyUIVisibility(true);
    }


    /// <summary>
    /// Handles the user intent to create a new room.
    /// </summary>
    public void CreateRoom(string matchName)
    {
        MainNetworkManager.matchMaker.CreateMatch(matchName, MainNetworkManager.matchSize,
                                        true, string.Empty, string.Empty, string.Empty,
                                        0, 0,
#pragma warning disable 618
                                        (bool success, string extendedInfo, MatchInfo matchInfo) => { _OnMatchCreate(success, extendedInfo, matchInfo, matchName); }
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
#pragma warning disable 618
    private void _OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo, string matchName)
#pragma warning restore 618
    {
        MainNetworkManager.OnMatchCreate(success, extendedInfo, matchInfo);
        UIController.OnMatchCreate(success, extendedInfo, matchName);

        if (success)
        {
            roomHosted = (true, matchInfo.networkId);
            GameManager.StartExpedition(matchName);
        }

        //m_CurrentRoomNumber = _GetRoomNumberFromNetworkId(matchInfo.networkId);
        //_ChangeLobbyUIVisibility(false);
        //SnackbarText.text = "Find a plane, tap to create a Cloud Anchor.";
        //CurrentRoomLabel.GetComponentInChildren<Text>().text = "Room: " + m_CurrentRoomNumber;
    }

    public bool IsRoomHosted()
    {
        return roomHosted.Item1;
    }

    /// <summary>
    /// When a room is destroyed (e.g. by clicking on the back button while an own room is open).
    /// </summary>
    public void DestroyRoom()
    {
        if (!roomHosted.Item1)
        {
            Debug.Log("This is not your room. You can't destroy it.");
            _OnMatchDestroy(false, "This is not your room.");
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
    /// Handles the user intent to refresh the room list.
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
#pragma warning disable 618
    private void _OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches, string expeditionName)
#pragma warning restore 618
    {
        Dictionary<string, string> hostANDexpeditionNames = new Dictionary<string, string>();

        MainNetworkManager.OnMatchList(success, extendedInfo, matches);
        if(matches != null)
        {
#pragma warning disable 618
            foreach (MatchInfoSnapshot match in matches)
#pragma warning restore 618
            {
                hostANDexpeditionNames.Add(match.hostNodeId.ToString(), match.name); //TODO: get PlayerName from NodeId... (at CreateRoom?)
            }
        }

        UIController.OnMatchList(success, extendedInfo, expeditionName, hostANDexpeditionNames, matches);

//            if (m_Manager.matches != null)
//            {
//                // Reset all buttons in the pool.
//                foreach (GameObject button in m_JoinRoomButtonsPool)
//                {
//                    button.SetActive(false);
//                    button.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
//                    button.GetComponentInChildren<Text>().text = string.Empty;
//                }

//                NoPreviousRoomsText.gameObject.SetActive(m_Manager.matches.Count == 0);

//                // Add buttons for each existing match.
//                int i = 0;
//#pragma warning disable 618
//                foreach (var match in m_Manager.matches)
//#pragma warning restore 618
//                {
//                    if (i >= k_MatchPageSize)
//                    {
//                        break;
//                    }

//                    var text = "Room " + _GetRoomNumberFromNetworkId(match.networkId);
//                    GameObject button = m_JoinRoomButtonsPool[i++];
//                    button.GetComponentInChildren<Text>().text = text;
//                    button.GetComponentInChildren<Button>().onClick.AddListener(() =>
//                        _OnJoinRoomClicked(match));
//                    button.SetActive(true);
//                }
//            }
    }

    ///// <summary>
    ///// Callback indicating that the Cloud Anchor was instantiated and the host request was
    ///// made.
    ///// </summary>
    ///// <param name="isHost">Indicates whether this player is the host.</param>
    //public void OnAnchorInstantiated(bool isHost)
    //{
    //    if (isHost)
    //    {
    //        //SnackbarText.text = "Hosting Cloud Anchor...";
    //    }
    //    else
    //    {
    //        //SnackbarText.text =
    //        //    "Cloud Anchor added to session! Attempting to resolve anchor...";
    //    }
    //}

    ///// <summary>
    ///// Callback indicating that the Cloud Anchor was hosted.
    ///// </summary>
    ///// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was hosted
    ///// successfully.</param>
    ///// <param name="response">The response string received.</param>
    //public void OnAnchorHosted(bool success, string response)
    //{
    //    if (success)
    //    {
    //        //SnackbarText.text = "Cloud Anchor successfully hosted! Tap to place more stars.";
    //    }
    //    else
    //    {
    //        //SnackbarText.text = "Cloud Anchor could not be hosted. " + response;
    //    }
    //}

    ///// <summary>
    ///// Callback indicating that the Cloud Anchor was resolved.
    ///// </summary>
    ///// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was resolved
    ///// successfully.</param>
    ///// <param name="response">The response string received.</param>
    //public void OnAnchorResolved(bool success, string response)
    //{
    //    if (success)
    //    {
    //        //SnackbarText.text = "Cloud Anchor successfully resolved! Tap to place more stars.";
    //    }
    //    else
    //    {
    //        //SnackbarText.text =
    //        //    "Cloud Anchor could not be resolved. Will attempt again. " + response;
    //    }
    //}

    ///// <summary>
    ///// Use the snackbar to display the error message.
    ///// </summary>
    ///// <param name="errorMessage">The error message to be displayed on the snackbar.</param>
    //public void ShowErrorMessage(string errorMessage)
    //{
    //    //SnackbarText.text = errorMessage;
    //}

    /// <summary>
    /// Handles the user intent to join the room associated with the button clicked.
    /// </summary>
    /// <param name="match">The information about the match that the user intents to
    /// join.</param>
#pragma warning disable 618
    public void OnJoinRoomClicked(MatchInfoSnapshot match)
#pragma warning restore 618
    {
        MainNetworkManager.matchName = match.name;
        MainNetworkManager.matchMaker.JoinMatch(match.networkId, string.Empty, string.Empty,
                                        string.Empty, 0, 0, _OnMatchJoined);
        //CloudAnchorsExampleController.OnEnterResolvingModeClick();
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
        //if (!success)
        //{
        //    SnackbarText.text = "Could not join to match: " + extendedInfo;
        //    return;
        //}

        //m_CurrentRoomNumber = _GetRoomNumberFromNetworkId(matchInfo.networkId);
        //ChangeLobbyUIVisibility(false);
        //SnackbarText.text = "Waiting for Cloud Anchor to be hosted...";
        //CurrentRoomLabel.GetComponentInChildren<Text>().text = "Room: " + m_CurrentRoomNumber;
    }

    ///// <summary>
    ///// Changes the lobby UI Visibility by showing or hiding the buttons.
    ///// </summary>
    ///// <param name="visible">If set to <c>true</c> the lobby UI will be visible. It will be
    ///// hidden otherwise.</param>
    //private void _ChangeLobbyUIVisibility(bool visible)
    //{
    //    LobbyScreen.gameObject.SetActive(visible);
    //    //CurrentRoomLabel.gameObject.SetActive(!visible);
    //    foreach (GameObject button in m_JoinRoomButtonsPool)
    //    {
    //        bool active = visible && button.GetComponentInChildren<Text>().text != string.Empty;
    //        button.SetActive(active);
    //    }
    //}
        
    //public bool IsClientConnected()
    //{
    //    return MainNetworkManager.IsClientConnected();
    //}



    //private string _GetRoomNumberFromNetworkId(NetworkID networkID)
    //{
    //    return (System.Convert.ToInt64(networkID.ToString()) % 10000).ToString();
    //}

}

