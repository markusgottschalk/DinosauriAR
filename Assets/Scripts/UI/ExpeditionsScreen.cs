using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionsScreen : Screen
{
    [SerializeField]
    private Canvas expeditionsScreen = default;
    public List<GameObject> expeditions = new List<GameObject>();


    public UIController UIController;


    void Start()
    {
        //expeditionsScreen = GetComponent<Canvas>();
    }

    public override void ShowScreen()
    {
        expeditionsScreen.enabled = true;
    }

    public override void HideScreen()
    {
        expeditionsScreen.enabled = false;
    }

    public void OnTengaduruExpeditionClicked()
    {
        UIController.Expeditions_TengaduruExpedition();
    }

    public void ShowExpeditionMultiplayerAddition_Tengaduru(bool active)
    {
        expeditions[0].SetActive(active);
    }

    public void OnExpeditionsRefreshClicked()
    {
        UIController.RefreshExpeditionRoomList(string.Empty);
    }

    public void ShowExpeditionMultiplayerAddition_All(bool active)
    {
        foreach(GameObject go in expeditions)
        {
            go.SetActive(active);
        }
    }

}
