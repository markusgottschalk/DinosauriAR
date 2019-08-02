using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartingAppScreen : UIScreen
{
    [SerializeField]
    private Canvas startinAppScreen = default;

    public UIController UIController;

    void Start()
    {
    }

    public override void ShowScreen()
    {
        startinAppScreen.enabled = true;
    }

    public override void HideScreen()
    {
        startinAppScreen.enabled = false;
    }

    public void OnExpeditionsButtonClicked()
    {
        UIController.StartingApp_Expeditions();
    }

    public void OnLeavingAppButtonClicked()
    {
        UIController.LeavingApp();
    }
}
