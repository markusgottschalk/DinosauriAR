using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExpeditionLobbyScreen : Screen
{
    public UIController UIController;

    [SerializeField]
    private Canvas expeditionLobbyScreen = default;
    [SerializeField]
    private TextMeshProUGUI leaderName = default;
    [SerializeField]
    private GameObject title = default;
    private string titleText;
    private string expeditionName;

    void Start()
    {
        //expeditionLobbyScreen = gameObject.GetComponent<Canvas>();
        titleText = title.GetComponent<TextMeshProUGUI>().text;
    }


    public override void ShowScreen()
    {
        expeditionLobbyScreen.enabled = true;
    }

    public void ShowScreen(string expeditionName)
    {
        this.expeditionName = expeditionName;
        changeTitle(expeditionName);
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

    public void ChangeLeaderName(string newName)
    {
        leaderName.text = newName;
    }

    public void OnExpeditionLobbyRefreshClicked()
    {
        UIController.RefreshExpeditionRoomList(expeditionName);
    }

    private void changeTitle(string newTitle)
    {
        switch (newTitle)
        {
            case "TengaduruExpedition":
                titleText = "Tengaduru-Expeditionen";
                break;
            default:
                titleText = "no expedition found";
                break;
        }
    }
}
