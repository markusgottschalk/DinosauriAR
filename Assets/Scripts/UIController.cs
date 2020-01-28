using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public NetworkManagerController NetworkManagerController;
    public GameManager GameManager;

    //All screens of the UI
    public GeneralAppScreen GeneralAppScreen;
    public SettingsScreen SettingsScreen;
    public StartingAppScreen StartingAppScreen;
    public ExpeditionsScreen ExpeditionsScreen;
    public TendaguruExpeditionScreen TendaguruExpeditionScreen;
    public TendaguruExpeditionEndScreen TendaguruExpeditionEndScreen;
    public ExpeditionsLobbyScreen ExpeditionsLobbyScreen;

    /// <summary>
    /// The stack of all previous clicked screens to show again when the back button was clicked.
    /// </summary>
    private Stack<UIScreen> previousScreens;

    /// <summary>
    /// The active screen.
    /// </summary>
    private UIScreen activeScreen;

    /// <summary>
    /// The coroutine for showing a message.
    /// </summary>
    private IEnumerator messageCoroutine = null;

    public bool ExpeditionEnded { get; private set; } = false;

    /// <summary>
    /// The Start method. Hide all screens except the starting and general app screen.
    /// </summary>
    void Start()
    {
        previousScreens = new Stack<UIScreen>();

        StartingAppScreen.ShowScreen();
        activeScreen = StartingAppScreen;
        GeneralAppScreen.ShowScreen();
        GeneralAppScreen.HideBackButton();

        ExpeditionsScreen.HideScreen();
        TendaguruExpeditionScreen.HideScreen();
        TendaguruExpeditionEndScreen.HideScreen();
        ExpeditionsLobbyScreen.HideScreen();

        TendaguruExpeditionScreen.ChangeJoinExpeditionButton(false);
    }


    /// <summary>
    /// Transition from starting screen to expeditions screen when the expeditions button has been clicked.
    /// </summary>
    public void StartingApp_Expeditions()
    {
        StartingAppScreen.HideScreen();
        previousScreens.Push(StartingAppScreen);
        ExpeditionsScreen.ShowScreen();
        activeScreen = ExpeditionsScreen;
        GeneralAppScreen.ShowBackButton();
    }

    /// <summary>
    /// Transition from expeditions screen to Tendaguru expedition screen when the Tendaguru expedition button has been clicked.
    /// </summary>
    public void Expeditions_TendaguruExpedition()
    {
        ExpeditionsScreen.HideScreen();
        previousScreens.Push(ExpeditionsScreen);
        TendaguruExpeditionScreen.ShowScreen();
        activeScreen = TendaguruExpeditionScreen;
    }

    /// <summary>
    /// Start Tendaguru Expedition as Host
    /// </summary>
    public void StartTendaguruExpedition()
    {
        TendaguruExpeditionScreen.HideScreen();
        previousScreens.Push(TendaguruExpeditionScreen);
        GeneralAppScreen.ShowSettingsButton(false);
        activeScreen = null;                        //null means that the app is in AR mode
    }


    /// <summary>
    /// Delegates  match creation to NetworkController.
    /// </summary>
    /// <param name="matchName">The name of the match</param>
    public void CreateMatch(string matchName)
    {
        NetworkManagerController.CreateRoom(matchName);
    }

    /// <summary>
    /// Callback when a match was created or not. Change the UI accordingly.
    /// </summary>
    /// <param name="success">Indicates if the request succeeded.</param>
    /// <param name="extendedInfo">A text description for the error if success is false.</param>
    /// <param name="matchANDhostName">The created matchname which consists of the name of the expedition and the name of the host.</param>
    public void OnMatchCreate(bool success, string extendedInfo, string matchANDhostName)
    {
        string matchName = matchANDhostName.Split('-')[0];
        if (!success)
        {
            ShowMessage("Fehler: Es konnte keine Expedition erstellt werden. Verbinde dich mit dem Internet und versuche es erneut. ", 5);
            Debug.LogError("Error, expedition could not be created. " + extendedInfo);
        }
        else
        {
            switch (matchName)
            {
                case "TendaguruExpedition":
                    StartTendaguruExpedition();
                    break;
                default: Debug.LogError("Error: No expedition with " + matchName + " was found.");
                    break;

            }
        }
    }

    /// <summary>
    /// Callback when a match was destroyed when the back button was clicked. Change the UI accordingly.
    /// </summary>
    /// <param name="success">Indicates if the request succeeded.</param>
    /// <param name="extendedInfo">A text description for the error if success is false.</param>
    public void OnMatchDestroy(bool success, string extendedInfo)
    {
        if (!success)
        {
            Debug.LogError("Error: The expedition could not be exited. " + extendedInfo);
            //ShowMessage("Fehler: Die Expedition konnte nicht beendet werden! " + extendedInfo, 5);
        }
    }

    /// <summary>
    /// Delegates refreshing list to NetworkManagerController.
    /// </summary>
    /// <param name="expeditionName">The name of the specific expeditions. Use string.empty for all expeditions</param>
    public void RefreshExpeditionRoomList(string expeditionName)
    {
        NetworkManagerController.RefreshRoomList(expeditionName);
    }

    /// <summary>
    /// Callback when list of matches were refreshed. Change the UI accordingly.
    /// </summary>
    /// <param name="success">Indicates if the request succeeded.</param>
    /// <param name="extendedInfo">A text description for the error if success is false.</param>
    /// <param name="expeditionNameFilter">For which expedition was the list refreshed. String.empty when whole list was refreshed.</param>
    /// <param name="matches">The matches of active expeditions for <paramref name="expeditionNameFilter"/>.</param>
#pragma warning disable 618
    public void OnMatchList(bool success, string extendedInfo, string expeditionNameFilter, List<UnityEngine.Networking.Match.MatchInfoSnapshot> matches)
#pragma warning restore 618
    {
        if (!success)
        {
            ShowMessage("Fehler: Die laufenden Expeditionen konnten nicht identifiziert werden! Verbinde dich mit dem Internet und versuche es erneut. ", 5);
            Debug.LogError("Error, the active expeditions could not be retrieved. " + extendedInfo);
        }
        else
        {
            if (matches.Count == 0) //no matches found
            {
                if (expeditionNameFilter == string.Empty) //searched for ALL matches -> all UI can be disabled
                {
                    TendaguruExpeditionScreen.ChangeJoinExpeditionButton(false);
                    ExpeditionsScreen.ShowExpeditionMultiplayerAddition_All(false);
                    GameManager.DeactivateAllExpeditions();
                    return;
                }
                else
                {
                    deactivateExpeditionInRoom(expeditionNameFilter);
                    ExpeditionsLobbyScreen.RefreshLobby(matches);
                    return;
                }
                
            }

#pragma warning disable 618
            foreach (UnityEngine.Networking.Match.MatchInfoSnapshot match in matches)
#pragma warning restore 618
            {
                //check if at least one expedition is (already) active
                foreach (Expedition expedition in GameManager.expeditions)
                {
                    if (match.name.Split('-')[0] == expedition.Name && expedition.active == false)
                    {
                        expedition.active = true;
                        activateExpeditionInRoom(match.name.Split('-')[0]);
                    }
                }
            }

            //only refresh the expedition which was asked for
            ExpeditionsLobbyScreen.RefreshLobby(matches);
        }
    }

    /// <summary>
    /// Activate the UI for when a specific expedition is hosted.
    /// </summary>
    /// <param name="expeditionName">The name of the expedition</param>
    private void activateExpeditionInRoom(string expeditionName)
    {
        if(expeditionName == "TendaguruExpedition")
        {
            TendaguruExpeditionScreen.ChangeJoinExpeditionButton(true);
            ExpeditionsScreen.ShowExpeditionMultiplayerAddition_Tendaguru(true);
            return;
        }

        Debug.LogError("Error: No expeditions with " + expeditionName + " were found.");
    }

    /// <summary>
    /// Deactivate the UI for when a specific expedition is hosted.
    /// </summary>
    /// <param name="expeditionName">The name of the expedition</param>
    private void deactivateExpeditionInRoom(string expeditionName)
    {
        foreach (Expedition expedition in GameManager.expeditions)
        {
            if(expedition.Name == expeditionName)
            {
                expedition.active = false;
            }
        }

        if (expeditionName == "TendaguruExpedition")
        {
            TendaguruExpeditionScreen.ChangeJoinExpeditionButton(false);
            ExpeditionsScreen.ShowExpeditionMultiplayerAddition_Tendaguru(false);
            return;
        }

        Debug.LogError("Error: No expeditions with " + expeditionName + " were found.");
    }

    /// <summary>
    /// Delegates the request to join a specific room to NetworkManagerController.
    /// </summary>
    /// <param name="match">The specific match</param>
#pragma warning disable 618
    public void JoinRoom(UnityEngine.Networking.Match.MatchInfoSnapshot match)
#pragma warning restore 618
    {
        NetworkManagerController.OnJoinRoomClicked(match);
    }

    /// <summary>
    /// Callback when a match was joined. Change the UI accordingly.
    /// </summary>
    /// <param name="success">Indicates if the request succeeded.</param>
    /// <param name="extendedInfo">A text description for the error if success is false.</param>
    public void OnMatchJoined(bool success, string extendedInfo)
    {
        if (!success)
        {
            ShowMessage("Fehler: Der Expedition konnte nicht beigetreten werden. Verbinde dich mit dem Internet und versuche es erneut. ", 5);
            Debug.LogError("Error, expedition could not be joined. " + extendedInfo);
            return;
        }
        ExpeditionsLobbyScreen.HideScreen();
        GeneralAppScreen.ShowSettingsButton(false);
        activeScreen = null;

        ShowMessage("Warte auf die Erstellung der Expedition...", 5);
    }

    /// <summary>
    /// Transition from Tendaguru expedition screen to expeditions lobby screen when the join expedition button has been clicked.
    /// </summary>
    public void TendaguruExpedition_ExpeditionsLobby()
    {
        TendaguruExpeditionScreen.HideScreen();
        previousScreens.Push(TendaguruExpeditionScreen);
        ExpeditionsLobbyScreen.ShowScreen("TendaguruExpedition");
        activeScreen = ExpeditionsLobbyScreen;
    }

    /// <summary>
    /// Transition from ar screen to tengaduru expedition end screen when the expedition exit button has been clicked.
    /// </summary>
    public void ARScreen_TengaduruExpeditionEnd()
    {
        ExitExpedition();
        TendaguruExpeditionEndScreen.ShowScreen();
        activeScreen = TendaguruExpeditionEndScreen;
    }

    /// <summary>
    /// Transition from Tendaguru expedition end screen to expeditions screen when the expedition overview button or the exit expedition button (when the expedition was unsuccessful) has been clicked.
    /// </summary>
    public void TendaguruExpeditionEnd_Expeditions()
    {
        ExpeditionEnded = false;
        TendaguruExpeditionEndScreen.HideScreen();
        ExpeditionsScreen.ShowScreen();
        GeneralAppScreen.ShowSettingsButton(true);
        GeneralAppScreen.ShowBackButton();

        //delete all Screens on stack after expeditions overview
        while(previousScreens.Peek() != ExpeditionsScreen)
        {
            previousScreens.Pop();
        }
        previousScreens.Pop();

        activeScreen = ExpeditionsScreen;
    }

    /// <summary>
    /// Transition from the active screen to the settings screen when the settings button has been clicked.
    /// </summary>
    public void OnSettingsButtonClicked()
    {
        if (!GeneralAppScreen.BackButtonIsActive())
        {
            GeneralAppScreen.ShowBackButton();
        }
        activeScreen.HideScreen();
        previousScreens.Push(activeScreen);
        activeScreen = SettingsScreen;
        SettingsScreen.ShowScreen();
        GeneralAppScreen.ShowSettingsButton(false);
    }

    /// <summary>
    /// Go back 1 screen when the back button was clicked.
    /// </summary>
    public void BackButtonWasClicked()
    {
        HideMessage();
        //if the active screen is UI, hide this screen
        if(activeScreen != null)
        {
            activeScreen.HideScreen();
        }
        //else if the active screen is AR, go back to UI
        else
        {
            GameManager.QuitExpedition();
        }

        if(activeScreen == SettingsScreen)
        {
            GeneralAppScreen.ShowSettingsButton(true);
        }

        //if there are previous screens
        if (previousScreens.Count != 0)
        {
            UIScreen previousScreen = previousScreens.Pop();
            previousScreen.ShowScreen();
            activeScreen = previousScreen;

            //hides the back button when there are no previous screens
            if (previousScreens.Count == 0)
            {
                GeneralAppScreen.HideBackButton();
            }
        }
        else //if there are no previous screens, the app will be closed
        {
            LeavingApp();
        }
    }

    public void LeavingApp()
    {
        GameManager.LeaveApp();
    }

    /// <summary>
    /// Change the name of the player.
    /// </summary>
    /// <param name="newName">The new name of the player</param>
    public void ChangePlayerName(string newName)
    {
        GameManager.PlayerName = newName;
        ShowMessage("Der Name wurde erfolgreich gespeichert.", 5);
    }

    /// <summary>
    /// Show the snackbar for <paramref name="delay"/> seconds. 
    /// </summary>
    /// <param name="delay">Seconds to show the snackbar</param>
    /// <returns></returns>
    private IEnumerator ShowSnackbarForSeconds(float delay)
    {
        GeneralAppScreen.ShowSnackbar();
        yield return new WaitForSeconds(delay);
        GeneralAppScreen.HideSnackbar();
    }

    /// <summary>
    /// Show a specific message for <paramref name="delay"/> in the snackbar. If a message is already shown, hide it and show the new one.
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="delay">Seconds to show the message</param>
    public void ShowMessage(string message, int delay)
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }
        GeneralAppScreen.ChangeSnackbarText(message);
        messageCoroutine = ShowSnackbarForSeconds(delay);
        StartCoroutine(messageCoroutine);
    }

    /// <summary>
    /// Hide the currently active message in the snackbar.
    /// </summary>
    public void HideMessage()
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }
        GeneralAppScreen.HideSnackbar();
    }

    /// <summary>
    /// Show the hand animation.
    /// </summary>
    /// <param name="seconds">Seconds to show the animation></param>
    public void ShowHandAnimation(float seconds)
    {
        GeneralAppScreen.ShowHandAnimation(seconds);
    }

    public string GetPlayerName()
    {
        return GameManager.PlayerName;
    }

    /// <summary>
    /// Callback indicating that the Cloud Anchor was hosted.
    /// </summary>
    /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was hosted
    /// successfully.</param>
    /// <param name="response">The response string received.</param>
    public void OnAnchorHosted(bool success, string response)
    {
        if (success)
        {
            ShowMessage("Die Expedition wurde erfolgreich erstellt. Aktiviere nun ein Werkzeug indem du es mit der Kamera aus verschiedenen Richtungen anschaust während es still liegt. ", 10);
        }
        else
        {
            ShowMessage("Die Expedition konnte nicht erstellt werden. Bitte versuche es erneut." + response, 5);
        }
    }

    /// <summary>
    /// Callback indicating that the Cloud Anchor was resolved.
    /// </summary>
    /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was resolved
    /// successfully.</param>
    /// <param name="response">The response string received.</param>
    public void OnAnchorResolved(bool success, string response)
    {
        if (success)
        {
            ShowMessage("Die Expedition wird dir jetzt angezeigt. Aktiviere nun ein Werkzeug indem du es mit der Kamera aus verschiedenen Richtungen anschaust während es still liegt.", 10);
        }
        else
        {
            ShowMessage("Die Expedition kann dir nicht angezeigt werden. Es wird erneut versucht... " + response, 5);
        }
    }

    /// <summary>
    /// Callback indicating that the Cloud Anchor was instantiated and the host request was
    /// made.
    /// </summary>
    /// <param name="isHost">Indicates whether this player is the host.</param>
    public void OnAnchorInstantiated(bool isHost)
    {
        if (isHost)
        {
            ShowMessage("Die Expedition wird nun an alle Teilnehmenden verbreitet...", 10);
        }
        else
        {
            ShowMessage("Die Expedition wurde erfolgreich von der Expeditionsleitung erstellt. Im Folgenden wird versucht sie an alle Teilnehmenden zu verbreiten...", 10);
        }
    }

    /// <summary>
    /// Callback when the expedition ends and all bones were found or destroyed.
    /// </summary>
    /// <param name="success">Indicates the success of the excavation.</param>
    public void EndExpedition(bool success)
    {
        if (success)
        {
            ShowMessage("Herzlichen Glückwunsch. Ihr habt das Skelett erfolgreich ausgegraben und viele Informationen gesammelt. Wenn ihr bereit seid könnt ihr die Expedition abschließen und sehen, wie der Dinosaurier mal ausgesehen haben könnte.", 10);
        }
        else
        {
            ShowMessage("Ihr habt es geschafft die Expedition zu beenden. Leider konntet ihr nicht so viele Informationen sammeln um das komplette Skelett zu rekonstruieren. Versucht es doch ein weiteres Mal und startet die Expedition erneut.", 10);
        }

        ExpeditionEnded = true;
        GeneralAppScreen.ShowEndExpeditionButton(true, success);
        GeneralAppScreen.HideBackButton();
    }


    /// <summary>
    /// Callback that happens when the client disconnected from the server.
    /// </summary>
    public void OnDisconnectedFromServer()
    {
        NetworkManagerController.StartMatchMaker();
        //if client is not in AR-mode, do nothing
        if(activeScreen != null)
        {
            return;
        }

        //if it is still in AR-mode and the expedition ended (e.g. because host exits the expedition and shuts down the match), exit the expedition as well
        if (ExpeditionEnded)
        {
            GeneralAppScreen.ExitExpedition();
            return;
        }

        //if another error happens
        ShowMessage("Es ist ein Fehler mit dem Netzwerk aufgetreten. Bitte starte die Expedition erneut.", 5);
        Debug.LogError("Network session disconnected!");

        Invoke("BackButtonWasClicked", 5.0f);
    }


    public void ExitExpedition()
    {
        GameManager.QuitExpedition();
    }
}
