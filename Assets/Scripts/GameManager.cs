using GoogleARCore.Examples.CloudAnchors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private string playerName;
    public string PlayerName
    {
        get
        {
            playerName = PlayerPrefs.GetString("PlayerName");
            return playerName;
        }

        set
        {
            playerName = value;
            Debug.Log("PlayerName: " + playerName);
            PlayerPrefs.SetString("PlayerName", playerName);
        }
    }

    public List<Expedition> expeditions;
    public GameObject UICamera;
    public ARCoreController ARCoreController;
    public UIController UIController;
    public NetworkManagerController NetworkManagerController;

    public Dictionary<ToolType, List<BlockMaterial>> toolsForBlocks;


    void Start()
    {
        if (!PlayerPrefs.HasKey("PlayerName"))
        {
            PlayerName = "Dein Name";
        }
        expeditions = new List<Expedition>();
        Expedition expedition1 = new Expedition("TendaguruExpedition");

        expeditions.Add(expedition1);
        UICamera.SetActive(true);

        toolsForBlocks = new Dictionary<ToolType, List<BlockMaterial>>();
        toolsForBlocks.Add(ToolType.ROCKHAMMER, new List<BlockMaterial>() { BlockMaterial.ROCK });
        toolsForBlocks.Add(ToolType.SHOVEL, new List<BlockMaterial>() { BlockMaterial.SAND });
        toolsForBlocks.Add(ToolType.SENSOR, new List<BlockMaterial>() { BlockMaterial.NONE });
        toolsForBlocks.Add(ToolType.BRUSH, new List<BlockMaterial>() { BlockMaterial.SAND, BlockMaterial.ROCK });
    }

    void Update()
    {
        //if the back button of the smartphone was pressed
        if (Input.GetKey(KeyCode.Escape))
        {
            //and the Application is in AR-mode (Hosting)
            if(ARCoreController.getApplicationMode() == ARCoreController.ApplicationMode.Hosting || ARCoreController.getApplicationMode() == ARCoreController.ApplicationMode.Resolving)
            {
                ARCoreController.QuitARMode();
                UIController.BackButtonWasClicked();    //Hide and activate screens
                UICamera.SetActive(true);
                NetworkManagerController.DestroyRoom();
            }

        }
    }

    public void DeactivateAllExpeditions()
    {
        foreach(Expedition expedition in expeditions)
        {
            expedition.active = false;
        }
    }

    public void StartExpedition(string expeditionName/*besser wäre: Expedition expedition*/)
    {
        UICamera.SetActive(false);
        ARCoreController.OnEnterHostingModeClick();
    }

    public void JoinExpedition(string expeditionName/*besser wäre: Expedition expedition*/)
    {
        UICamera.SetActive(false);
        ARCoreController.OnEnterResolvingModeClick();
    }

    public void QuitExpedition()
    {
        ARCoreController.QuitARMode();
        UICamera.SetActive(true);
    }

    public void LeaveApp()
    {
        Application.Quit();//TODO: besser nur Spiel und AR-modus zu beenden?!
    }
}
