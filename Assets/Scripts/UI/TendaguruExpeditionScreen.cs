using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TendaguruExpeditionScreen : UIScreen
{
    [SerializeField]
    private Canvas tendaguruExpeditionScreen = default;

    [SerializeField]
    private string expeditionName = default;

    public UIController UIController;

    [SerializeField]
    private Button joinExpedition = default;


    // Start is called before the first frame update
    void Start()
    {
        //tendaguruExpeditionScreen = gameObject.GetComponent<Canvas>();
    }

    public override void ShowScreen()
    {
        tendaguruExpeditionScreen.enabled = true;
    }

    public override void HideScreen()
    {
        tendaguruExpeditionScreen.enabled = false;
    }

    public void OnTendaguruExpeditionPrepareClicked()
    {
        UIController.CreateMatch(expeditionName);
    }

    public void OnTendaguruExpeditionJoinClicked()
    {
        UIController.TendaguruExpedition_ExpeditionsLobby();
    }

    public void ChangeJoinExpeditionButton(bool interactable)
    {
        joinExpedition.interactable = interactable;
    }

    public void OnTendaguruExpeditionRefreshClicked()
    {
        UIController.RefreshExpeditionRoomList(expeditionName);
    }
}
