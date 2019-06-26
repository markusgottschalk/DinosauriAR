using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionsLobbyScreen : Screen
{
    public UIController UIController;
    [SerializeField]
    private Canvas expeditionsLobbyScreen = default;
    [SerializeField]
    private GameObject title = default;
    private string titleText;
    private string expeditionName;

    [SerializeField]
    private GameObject expeditionList = default;
    public GameObject ExpeditionRoomPrefab;

    private List<GameObject> joinButtons;

    void Start()
    {
        //expeditionsLobbyScreen = gameObject.GetComponent<Canvas>();
        titleText = title.GetComponent<TextMeshProUGUI>().text;
        joinButtons = new List<GameObject>();
    }

    public override void ShowScreen()
    {
        expeditionsLobbyScreen.enabled = true;
    }

    public void ShowScreen(string expeditionName)
    {
        this.expeditionName = expeditionName;
        changeTitle(expeditionName);
        expeditionsLobbyScreen.enabled = true;
    }

    public override void HideScreen()
    {
        expeditionsLobbyScreen.enabled = false;
    }

    public void OnJoinExpeditionClicked()
    {
        //TODO -> evtl gleich beim Klicken auf Expedition joinen...
        //UIController.ExpeditionsLobby_ExpeditionLobby(expeditionName);
    }

    public void OnExpeditionsLobbyRefreshClicked()
    {
        UIController.RefreshExpeditionRoomList(expeditionName);
    }

    public void RefreshLobby(Dictionary<string, string> expeditionNamesANDHosts, List<UnityEngine.Networking.Match.MatchInfoSnapshot> matches)
    {
        foreach(GameObject go in joinButtons)
        {
            Destroy(go);
        }

        if(expeditionNamesANDHosts.Count == 0) //no matches
        {
            return;
        }

        int i = 0;
        foreach(KeyValuePair<string, string> match in expeditionNamesANDHosts)
        {
            GameObject expedition = Instantiate(ExpeditionRoomPrefab, expeditionList.transform);
            expedition.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = match.Value; //write leader name
            joinButtons.Add(expedition);
            expedition.GetComponent<Button>().onClick.AddListener(() => UIController.JoinRoom(matches[i]));
            i++;
        }
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
