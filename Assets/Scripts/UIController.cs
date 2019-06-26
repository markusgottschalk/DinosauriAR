using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore.Examples.CloudAnchors;

public class UIController : MonoBehaviour
{
    public ARCoreController CloudAnchorsController;
    public NetworkManagerController NetworkManagerController;
    public GameManager GameManager;

    public GeneralAppScreen GeneralAppScreen;
    public SettingsScreen SettingsScreen;
    public StartingAppScreen StartingAppScreen;
    public ExpeditionsScreen ExpeditionsScreen;
    public TengaduruExpeditionScreen TengaduruExpeditionScreen;
    public ExpeditionsLobbyScreen ExpeditionsLobbyScreen;
    //public ExpeditionLobbyScreen ExpeditionLobbyScreen;

    private Stack<Screen> previousScreens;
    private Screen activeScreen;


    void Start()
    {
        previousScreens = new Stack<Screen>();

        StartingAppScreen.ShowScreen();
        activeScreen = StartingAppScreen;
        GeneralAppScreen.HideBackButton(); //TODO: evtl. nicht so manuell festlegen, sondern immer bei Änderung des Stacks überprüfen, ob Stack leer ist...

        ExpeditionsScreen.HideScreen();
        TengaduruExpeditionScreen.HideScreen();
        ExpeditionsLobbyScreen.HideScreen();
        //ExpeditionLobbyScreen.HideScreen();

        TengaduruExpeditionScreen.ChangeJoinExpeditionButton(false);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            BackButtonWasClicked();
        }
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
    /// Transition from expeditions screen to Tengaduru expedition screen when the Tengaduru expedition button has been clicked.
    /// </summary>
    public void Expeditions_TengaduruExpedition()
    {
        ExpeditionsScreen.HideScreen();
        previousScreens.Push(ExpeditionsScreen);
        TengaduruExpeditionScreen.ShowScreen();
        activeScreen = TengaduruExpeditionScreen;
    }


    ///// <summary>
    ///// Transition from Tengaduru expedition screen to specific expedition lobby screen when the prepare expedition button has been clicked.
    ///// </summary>
    //public void TengaduruExpedition_ExpeditionLobby()
    //{
    //    TengaduruExpeditionScreen.HideScreen();
    //    previousScreens.Push(TengaduruExpeditionScreen);
    //    ExpeditionLobbyScreen.ShowScreen("TengaduruExpedition");
    //    activeScreen = ExpeditionLobbyScreen;     
    //}

    /// <summary>
    /// Start Tengaduru Expedition as Host
    /// </summary>
    public void StartTengaduruExpedition()
    {
        TengaduruExpeditionScreen.HideScreen();
        GeneralAppScreen.HideBackButton();              //TODO: erstmal Settings&Back-Button ausblenden
        GeneralAppScreen.ShowSettingsButton(false);
        GameManager.StartExpedition("TengaduruExpedition");
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
    public void OnMatchCreate(bool success, string extendedInfo, string matchName)
    {
        if (!success)
        {
            GeneralAppScreen.ChangeSnackbarText("Fehler: Es konnte keine Expedition erstellt werden. " + extendedInfo);
            ShowSnackbarForSeconds(5);
            return;
        }
        else
        {
            switch (matchName)
            {
                case "TengaduruExpedition":
                    StartTengaduruExpedition();
                    //TengaduruExpedition_ExpeditionLobby();
                    //ExpeditionLobbyScreen.ChangeLeaderName(GameManager.PlayerName);
                    break;
                default: Debug.Log("Error: No expedition with " + matchName + " was found.");
                    break;

            }
        }
        GeneralAppScreen.ShowSettingsButton(false); //TODO: erstmal ohne Namensänderung wenn Match erstellt wurde
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
            GeneralAppScreen.ChangeSnackbarText("Fehler: Die Expedition konnte nicht beendet werden! " + extendedInfo);
            ShowSnackbarForSeconds(5);
        }
        else
        {
            Screen previousScreen = previousScreens.Pop();
            previousScreen.ShowScreen();
        }
        GeneralAppScreen.ShowSettingsButton(true); //TODO...  (siehe OnMatchCreate...)
    }

    /// <summary>
    /// Delegates refreshing list to NetworkController.
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
    /// <param name="hostANDexpeditionsNames">The expedition names and host names of active expeditions for expeditionNameFilter.</param>
    public void OnMatchList(bool success, string extendedInfo, string expeditionNameFilter, Dictionary<string, string> hostANDexpeditionsNames, List<UnityEngine.Networking.Match.MatchInfoSnapshot> matches)
    {
        if (!success)
        {
            GeneralAppScreen.ChangeSnackbarText("Fehler: Die laufenden Expeditionen konnten nicht identifiziert werden! " + extendedInfo);
            ShowSnackbarForSeconds(5);
        }
        else
        {
            //GeneralAppScreen.ChangeSnackbarText("Die verfügbaren Spiele werden nun angezeigt.");
            //StartCoroutine(ShowSnackbarForSeconds(5));
            if (hostANDexpeditionsNames.Count == 0) //no matches found
            {
                if (expeditionNameFilter == string.Empty) //searched for ALL matches -> all UI can be disabled
                {
                    TengaduruExpeditionScreen.ChangeJoinExpeditionButton(false); //"TODO:" Buttons von allen spezifischen ExpeditionScreens deaktivieren
                    ExpeditionsScreen.ShowExpeditionMultiplayerAddition_All(false);
                    GameManager.deactivateAllExpeditions();
                    //GeneralAppScreen.ChangeSnackbarText("Es konnten keine laufenden Expeditionen gefunden werden");
                    //ShowSnackbarForSeconds(5);
                    return;
                }
                else
                {
                    deactivateExpeditionInRoom(expeditionNameFilter);
                    ExpeditionsLobbyScreen.RefreshLobby(hostANDexpeditionsNames, matches);
                    return;
                }
                
            }
            

            foreach(KeyValuePair<string, string> match in hostANDexpeditionsNames)
            {
                //check if at least one expedition is (already) active
                foreach (Expedition expedition in GameManager.expeditions)
                {
                    if (match.Value == expedition.Name && expedition.active == false)
                    {
                        expedition.active = true;
                        activateExpeditionInRoom(match.Value);
                    }
                }
            }

            //only refresh the expedition which was asked for
            ExpeditionsLobbyScreen.RefreshLobby(hostANDexpeditionsNames, matches);
        }
    }

    private void activateExpeditionInRoom(string expeditionName)
    {
        if(expeditionName == "TengaduruExpedition")
        {
            TengaduruExpeditionScreen.ChangeJoinExpeditionButton(true);
            ExpeditionsScreen.ShowExpeditionMultiplayerAddition_Tengaduru(true);
            return;
        }

        Debug.Log("Error: No expeditions with " + expeditionName + " were found.");
    }

    private void deactivateExpeditionInRoom(string expeditionName)
    {
        foreach (Expedition expedition in GameManager.expeditions)
        {
            if(expedition.Name == expeditionName)
            {
                expedition.active = false;
            }
        }

        if (expeditionName == "TengaduruExpedition")
        {
            TengaduruExpeditionScreen.ChangeJoinExpeditionButton(false);
            ExpeditionsScreen.ShowExpeditionMultiplayerAddition_Tengaduru(false);
            //todo... mehr machen, was passiert im expeditionsLobbyScreen... -> "leerer Screen"
            return;
        }

        Debug.Log("Error: No expeditions with " + expeditionName + " were found.");
    }

    public void JoinRoom(UnityEngine.Networking.Match.MatchInfoSnapshot match)
    {
        NetworkManagerController.OnJoinRoomClicked(match);
    }

    public void OnMatchJoined(bool success, string extendedInfo)
    {
        if (!success)
        {
            GeneralAppScreen.ChangeSnackbarText("Fehler: Der Expedition konnte nicht beigetreten werden. " + extendedInfo);
            ShowSnackbarForSeconds(5);
            return;
        }
        ExpeditionsLobbyScreen.HideScreen();
        GeneralAppScreen.HideBackButton();              //TODO: erstmal Settings&Back-Button ausblenden
        GeneralAppScreen.ShowSettingsButton(false);
        GameManager.JoinExpedition("TengaduruExpedition");

        GeneralAppScreen.ChangeSnackbarText("Warte auf das Hosten des Cloud Anchors...");
        ShowSnackbarForSeconds(5);
    }

    /// <summary>
    /// Transition from Tengaduru expedition screen to expeditions lobby screen when the join expedition button has been clicked.
    /// </summary>
    public void TengaduruExpedition_ExpeditionsLobby()
    {
        TengaduruExpeditionScreen.HideScreen();
        previousScreens.Push(TengaduruExpeditionScreen);
        ExpeditionsLobbyScreen.ShowScreen("TengaduruExpedition");
        activeScreen = ExpeditionsLobbyScreen;
    }


    ///// <summary>
    ///// Transition from expeditions lobby screen to specific expedition lobby screen when the join expedition button has been clicked.
    ///// </summary>
    //public void ExpeditionsLobby_ExpeditionLobby(string expeditionName)
    //{
    //    ExpeditionsLobbyScreen.HideScreen();
    //    previousScreens.Push(ExpeditionsLobbyScreen);
    //    ExpeditionLobbyScreen.ShowScreen(expeditionName);
    //    activeScreen = ExpeditionLobbyScreen;
    //}

    public void ExpeditionLobby_StartExpedition()
    {
        //TODO -> start AR...übergeben an AR Controller... start AR Screen (AR Discovery Guide?)
        //Hide ExpeditionLobbyScreen
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
        activeScreen.HideScreen();
        if(activeScreen == SettingsScreen)
        {
            GeneralAppScreen.ShowSettingsButton(true);
        }

        if (previousScreens.Count != 0)
        {
            if (NetworkManagerController.IsRoomHosted())
            {
                NetworkManagerController.DestroyRoom();
                //TODO: check if in ARScreen -> then go back to TengaduruExpeditionScreen...
            }
            else
            {
                Screen previousScreen = previousScreens.Pop();
                previousScreen.ShowScreen();
                activeScreen = previousScreen;
            }

            if (previousScreens.Count == 0)
            {
                GeneralAppScreen.HideBackButton(); //TODO: evtl. nicht so manuell festlegen, sondern immer bei Änderung des Stacks überprüfen, ob Stack leer ist...
            }
        }
        else
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
        GeneralAppScreen.ChangeSnackbarText("Der Name wurde erfolgreich gespeichert.");
        StartCoroutine(ShowSnackbarForSeconds(5));
    }

    /// <summary>
    /// Show the snackbar for <paramref name="delay"/> seconds. 
    /// </summary>
    /// <param name="delay">Seconds to show the snackbar</param>
    /// <returns></returns>
    public IEnumerator ShowSnackbarForSeconds(float delay)
    {
        GeneralAppScreen.ShowSnackbar();
        yield return new WaitForSeconds(delay);
        GeneralAppScreen.HideSnackbar();
    }

    public void ShowMessage(string message)
    {
        GeneralAppScreen.ChangeSnackbarText(message);
        StartCoroutine(ShowSnackbarForSeconds(5));
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
            GeneralAppScreen.ChangeSnackbarText("Der Cloud Anchor wurde erfolgreich erstellt. Tappe auf den Bildschirm um Sterne zu erstellen!");
            StartCoroutine(ShowSnackbarForSeconds(5));
            //SnackbarText.text = "Cloud Anchor successfully hosted! Tap to place more stars.";
        }
        else
        {
            GeneralAppScreen.ChangeSnackbarText("Der Cloud Anchor konnte nicht erstellt werden. " + response);
            StartCoroutine(ShowSnackbarForSeconds(5));
            //SnackbarText.text = "Cloud Anchor could not be hosted. " + response;
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
            GeneralAppScreen.ChangeSnackbarText("Der Cloud Anchor wurde erfolgreich aufgelöst. Tappe auf den Bildschirm um Sterne zu erstellen!");
            StartCoroutine(ShowSnackbarForSeconds(5));
            //SnackbarText.text = "Cloud Anchor successfully resolved! Tap to place more stars.";
        }
        else
        {
            GeneralAppScreen.ChangeSnackbarText("Der Cloud Anchor konnte nicht aufgelöst werden. Es wird erneut versucht... " + response);
            StartCoroutine(ShowSnackbarForSeconds(5));
            //SnackbarText.text =
            //    "Cloud Anchor could not be resolved. Will attempt again. " + response;
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
            GeneralAppScreen.ChangeSnackbarText("Der Cloud Anchor wird gehostet...");
            StartCoroutine(ShowSnackbarForSeconds(5));
            //SnackbarText.text = "Hosting Cloud Anchor...";
        }
        else
        {
            GeneralAppScreen.ChangeSnackbarText("Der Cloud Anchor wurde erfolgreich vom Host hinzugefügt. Versuche nun ihn aufzulösen...");
            StartCoroutine(ShowSnackbarForSeconds(5));
            //SnackbarText.text =
            //    "Cloud Anchor added to session! Attempting to resolve anchor...";
        }
    }
}
