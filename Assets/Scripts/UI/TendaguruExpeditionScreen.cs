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

    /// <summary>
    /// The refresh icons which will be shown when the refresh button has been clicked.
    /// </summary>
    [SerializeField]
    private List<Sprite> refreshIconsToRotate = null;
    [SerializeField]
    private GameObject refreshGameObject = null;

    public UIController UIController;

    [SerializeField]
    private Button joinExpedition = default;

    void Start()
    {
        
    }

    public override void ShowScreen()
    {
        tendaguruExpeditionScreen.enabled = true;
    }

    public override void HideScreen()
    {
        tendaguruExpeditionScreen.enabled = false;
    }

    public void OnTendaguruExpeditionStartClicked()
    {
        UIController.CreateMatch(expeditionName);
    }

    public void OnTendaguruExpeditionJoinClicked()
    {
        UIController.TendaguruExpedition_ExpeditionsLobby();
    }

    /// <summary>
    /// Change the state of the join expedition button.
    /// </summary>
    /// <param name="interactable">The state of the button</param>
    public void ChangeJoinExpeditionButton(bool interactable)
    {
        joinExpedition.interactable = interactable;
    }

    /// <summary>
    /// When refresh button has been clicked, "rotate" the refresh button to give feedback that the click was successful, even when no new expeditions are shown.
    /// </summary>
    public void OnTendaguruExpeditionRefreshClicked()
    {
        StartCoroutine(rotateRefreshIcon(2));
        UIController.RefreshExpeditionRoomList(expeditionName);
    }

    private IEnumerator rotateRefreshIcon(float seconds)
    {
        int i = 0;
        while(seconds > 0)
        {
            refreshGameObject.GetComponent<Image>().sprite = refreshIconsToRotate[i];
            i++;
            if(i >= refreshIconsToRotate.Count)
            {
                i = 0;
            }
            seconds -= 0.5f;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
