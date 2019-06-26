using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TengaduruExpeditionScreen : Screen
{
    [SerializeField]
    private Canvas tengaduruExpeditionScreen = default;

    [SerializeField]
    private string expeditionName = default;

    public UIController UIController;

    [SerializeField]
    private Button joinExpedition = default;


    // Start is called before the first frame update
    void Start()
    {
        //tengaduruExpeditionScreen = gameObject.GetComponent<Canvas>();
    }

    public override void ShowScreen()
    {
        tengaduruExpeditionScreen.enabled = true;
    }

    public override void HideScreen()
    {
        tengaduruExpeditionScreen.enabled = false;
    }

    public void OnTengaduruExpeditionPrepareClicked()
    {
        UIController.CreateMatch(expeditionName);
    }

    public void OnTengaduruExpeditionJoinClicked()
    {
        UIController.TengaduruExpedition_ExpeditionsLobby();
    }

    public void ChangeJoinExpeditionButton(bool interactable)
    {
        joinExpedition.interactable = interactable;
    }

    public void OnTengaduruExpeditionRefreshClicked()
    {
        UIController.RefreshExpeditionRoomList(expeditionName);
    }
}
