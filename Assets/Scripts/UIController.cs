using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GoogleARCore.Examples.CloudAnchors.CloudAnchorsController CloudAnchorsController;

    public GeneralAppScreen GeneralAppScreen;
    public SettingsScreen SettingsScreen;
    public StartingAppScreen StartingAppScreen;
    public ExpeditionsScreen ExpeditionsScreen;
    public TengaduruExpeditionScreen TengaduruExpeditionScreen;
    public ExpeditionsLobbyScreen ExpeditionsLobbyScreen;
    public ExpeditionLobbyScreen ExpeditionLobbyScreen;

    private Stack<Screen> previousScreens;
    private Screen activeScreen;

    private string playerName;
    public string PlayerName
    {
        get
        {
            playerName = PlayerPrefs.GetString("PlayerName");
            return playerName;
        }

        set
        {
            playerName = value;
            PlayerPrefs.SetString("PlayerName", value);
        }
    }

    void Start()
    {
        PlayerName = "Dein Name"; //TODO > nur einmal bei Start festlegen, aber in richtigem Controller...

        previousScreens = new Stack<Screen>();

        StartingAppScreen.ShowScreen();
        activeScreen = StartingAppScreen;
        GeneralAppScreen.HideBackButton(); //TODO: evtl. nicht so manuell festlegen, sondern immer bei Änderung des Stacks überprüfen, ob Stack leer ist...
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


    /// <summary>
    /// Transition from Tengaduru expedition screen to specific expedition lobby screen when the prepare expedition button has been clicked.
    /// </summary>
    public void TengaduruExpedition_ExpeditionLobby()
    {
        TengaduruExpeditionScreen.HideScreen();
        previousScreens.Push(TengaduruExpeditionScreen);
        ExpeditionLobbyScreen.ShowScreen();
        activeScreen = ExpeditionLobbyScreen;
    }

    /// <summary>
    /// Transition from Tengaduru expedition screen to expeditions lobby screen when the join expedition button has been clicked.
    /// </summary>
    public void TengaduruExpedition_ExpeditionsLobby()
    {
        TengaduruExpeditionScreen.HideScreen();
        previousScreens.Push(TengaduruExpeditionScreen);
        ExpeditionsLobbyScreen.ShowScreen();
        activeScreen = ExpeditionLobbyScreen;
    }


    /// <summary>
    /// Transition from expeditions lobby screen to specific expedition lobby screen when the join expedition button has been clicked.
    /// </summary>
    public void ExpeditionsLobby_ExpeditionLobby()
    {
        ExpeditionsLobbyScreen.HideScreen();
        previousScreens.Push(ExpeditionsLobbyScreen);
        ExpeditionLobbyScreen.ShowScreen();
        activeScreen = ExpeditionLobbyScreen;
    }

    public void ExpeditionLobby_StartExpedition()
    {
        //TODO -> start AR...übergeben an AR Controller... start AR Screen (AR Discovery Guide?)
    }

    /// <summary>
    /// Transition from the active screen to the settings screen when the settings button has been clicked.
    /// </summary>
    public void OnSettingsButtonClicked()
    {
        activeScreen.HideScreen();
        SettingsScreen.ShowScreen();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackButtonWasClicked();
        }     
    }

    public void BackButtonWasClicked()
    {
        activeScreen.HideScreen();

        if (previousScreens.Count != 0)
        {
            Screen previousScreen = previousScreens.Pop();
            previousScreen.ShowScreen();
            if (previousScreens.Count == 0)
            {
                GeneralAppScreen.HideBackButton(); GeneralAppScreen.HideBackButton(); //TODO: evtl. nicht so manuell festlegen, sondern immer bei Änderung des Stacks überprüfen, ob Stack leer ist...
            }
        }
        else
        {
            //TODO: Quit is in CloudAnchorsController...?
            //CloudAnchorsController._DoQuit(); or isQuitting = true...
            Application.Quit();
        }
    }
}
