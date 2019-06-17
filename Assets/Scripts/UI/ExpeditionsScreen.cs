using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionsScreen : Screen
{
    private Canvas m_ExpeditionsScreen;
    public UIController UIController;

    void Start()
    {
        m_ExpeditionsScreen = gameObject.GetComponent<Canvas>();
    }

    public override void ShowScreen()
    {
        m_ExpeditionsScreen.enabled = true;
    }

    public override void HideScreen()
    {
        m_ExpeditionsScreen.enabled = false;
    }

    public void OnTengaduruExpeditionClicked()
    {
        UIController.Expeditions_TengaduruExpedition();
    }
}
