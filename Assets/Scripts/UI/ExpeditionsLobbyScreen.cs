using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionsLobbyScreen : Screen
{
    public UIController UIController;
    private Canvas expeditionsLobbyScreen;


    void Start()
    {
        expeditionsLobbyScreen = gameObject.GetComponent<Canvas>();
    }

    public override void ShowScreen()
    {
        expeditionsLobbyScreen.enabled = true;
    }

    public override void HideScreen()
    {
        expeditionsLobbyScreen.enabled = false;
    }

    public void OnJoinExpeditionClicked()
    {
        UIController.ExpeditionsLobby_ExpeditionLobby();
    }

}
