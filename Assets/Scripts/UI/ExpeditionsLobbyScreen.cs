using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionsLobbyScreen : UIScreen
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

    /// <summary>
    /// The refresh Icons which will be shown when the refresh button is clicked.
    /// </summary>
    [SerializeField]
    private List<Sprite> refreshIconsToRotate = null;
    [SerializeField]
    private GameObject refreshGameObject = null;

    void Start()
    {
        titleText = title.GetComponent<TextMeshProUGUI>().text;
        joinButtons = new List<GameObject>();
    }

    public override void ShowScreen()
    {
        expeditionsLobbyScreen.enabled = true;
    }

    /// <summary>
    /// Show screen for specific expedition.
    /// </summary>
    /// <param name="expeditionName">The name of the expedition</param>
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

    /// <summary>
    /// When refresh button has been clicked, "rotate" the refresh button to give feedback that the click was successful, even when no new expeditions are shown.
    /// </summary>
    public void OnExpeditionsLobbyRefreshClicked()
    {
        StartCoroutine(rotateRefreshIcon(2));
        UIController.RefreshExpeditionRoomList(expeditionName);
    }

    /// <summary>
    /// Refresh the Lobby with available matches.
    /// </summary>
    /// <param name="matches">The available matches</param>
#pragma warning disable 618
    public void RefreshLobby(List<UnityEngine.Networking.Match.MatchInfoSnapshot> matches)
#pragma warning restore 618
    {
        foreach (GameObject go in joinButtons)
        {
            Destroy(go);
        }

        if(matches.Count == 0) //no matches
        {
            return;
        }

        //add join button for each match
#pragma warning disable 618
        foreach (UnityEngine.Networking.Match.MatchInfoSnapshot match in matches)
#pragma warning restore 618
        {
            GameObject expedition = Instantiate(ExpeditionRoomPrefab, expeditionList.transform);
            expedition.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = match.name.Split('-')[1];
            joinButtons.Add(expedition);
            expedition.GetComponent<Button>().onClick.AddListener(() => UIController.JoinRoom(match));
        }
    }

    /// <summary>
    /// Change the title of the screen.
    /// </summary>
    /// <param name="newTitle">The new title</param>
    private void changeTitle(string newTitle)
    {
        switch (newTitle)
        {
            case "TendaguruExpedition":
                titleText = "Tendaguru-Expeditionen";
                break;
            default:
                titleText = "no expedition found";
                break;
        }
    }

    private IEnumerator rotateRefreshIcon(float seconds)
    {
        int i = 0;
        while (seconds > 0)
        {
            refreshGameObject.GetComponent<Image>().sprite = refreshIconsToRotate[i];
            i++;
            if (i >= refreshIconsToRotate.Count)
            {
                i = 0;
            }
            seconds -= 0.5f;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
