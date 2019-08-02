using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TendaguruExpeditionEndScreen : UIScreen
{
    [SerializeField]
    private Canvas tendaguruExpeditionEndScreen = default;

    public UIController UIController;


    public override void ShowScreen()
    {
        tendaguruExpeditionEndScreen.enabled = true;
    }

    public override void HideScreen()
    {
        tendaguruExpeditionEndScreen.enabled = false;
    }

    public void OnExpeditionOverviewButtonClicked()
    {
        UIController.TendaguruExpeditionEnd_Expeditions();
    }
}
