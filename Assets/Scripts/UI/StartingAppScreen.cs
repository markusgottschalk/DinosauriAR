using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartingAppScreen : UIScreen
{
    [SerializeField]
    private Canvas startinAppScreen = default;

    //[SerializeField]
    //private GameObject m_ExpeditionsButton;
    public UIController UIController;

    void Start()
    {
        //startinAppScreen = gameObject.GetComponent<Canvas>();
        //m_ExpeditionsButton.GetComponent<Button>().onClick.AddListener(_OnExpeditionsButtonClicked);
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
