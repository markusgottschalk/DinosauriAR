using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionLobbyScreen : Screen
{
    public UIController UIController;
    private Canvas expeditionLobbyScreen;

    void Start()
    {
        expeditionLobbyScreen = gameObject.GetComponent<Canvas>();
    }


    public override void ShowScreen()
    {
        expeditionLobbyScreen.enabled = true;
    }

    public override void HideScreen()
    {
        expeditionLobbyScreen.enabled = false;
    }

    public void OnStartExpeditionClicked()
    {
        UIController.ExpeditionLobby_StartExpedition();
    }
}
