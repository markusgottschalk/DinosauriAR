using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TengaduruExpeditionScreen : Screen
{
    private Canvas tengaduruSxpeditionScreen;
    public UIController UIController;


    // Start is called before the first frame update
    void Start()
    {
        tengaduruSxpeditionScreen = gameObject.GetComponent<Canvas>();
    }

    public override void ShowScreen()
    {
        tengaduruSxpeditionScreen.enabled = true;
    }

    public override void HideScreen()
    {
        tengaduruSxpeditionScreen.enabled = false;
    }

    public void OnTengaduruExpeditionPrepareClicked()
    {
        UIController.TengaduruExpedition_ExpeditionLobby();
    }

    public void OnTengaduruExpeditionJoinClicked()
    {
        UIController.TengaduruExpedition_ExpeditionsLobby();
    }
}
