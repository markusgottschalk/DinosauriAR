using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionsScreen : UIScreen
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

    public void OnTendaguruExpeditionClicked()
    {
        UIController.Expeditions_TendaguruExpedition();
    }

    public void ShowExpeditionMultiplayerAddition_Tendaguru(bool active)
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
