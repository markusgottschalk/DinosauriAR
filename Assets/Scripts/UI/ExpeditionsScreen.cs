using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionsScreen : UIScreen
{
    [SerializeField]
    private Canvas expeditionsScreen = default;
    public List<GameObject> expeditions = new List<GameObject>();

    public UIController UIController;

    /// <summary>
    /// Show a information message when the user activates the screen for the first time.
    /// </summary>
    private bool infoMessageWasShown;

    [SerializeField]
    private List<Sprite> refreshIconsToRotate = null;
    [SerializeField]
    private GameObject refreshGameObject = null;

    void Start()
    {
        infoMessageWasShown = false;
    }

    public override void ShowScreen()
    {
        expeditionsScreen.enabled = true;
        if (!infoMessageWasShown)
        {
            UIController.ShowMessage("Hier kannst du verschiedene Expeditionen auswählen, die du gerne durchführen würdest. Wenn eine Expedition bereits aktiv ist, wird dies auch angezeigt.", 5);
            infoMessageWasShown = true;
        }
        UIController.RefreshExpeditionRoomList(string.Empty);
    }

    public override void HideScreen()
    {
        expeditionsScreen.enabled = false;
        UIController.HideMessage();
    }

    public void OnTendaguruExpeditionClicked()
    {
        UIController.Expeditions_TendaguruExpedition();
    }

    /// <summary>
    /// Shows the addition on a expedition when at least one expedition is hosted.
    /// </summary>
    /// <param name="active">If the expedition is active or not</param>
    public void ShowExpeditionMultiplayerAddition_Tendaguru(bool active)
    {
        expeditions[0].SetActive(active);
    }

    public void OnExpeditionsRefreshClicked()
    {
        StartCoroutine(rotateRefreshIcon(2));
        UIController.RefreshExpeditionRoomList(string.Empty);
    }

    /// <summary>
    /// Show the addition on all expedition when they are hosted or not.
    /// </summary>
    /// <param name="active">The state of the expeditions</param>
    public void ShowExpeditionMultiplayerAddition_All(bool active)
    {
        foreach(GameObject go in expeditions)
        {
            go.SetActive(active);
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
