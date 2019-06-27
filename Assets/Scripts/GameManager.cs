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
    public GameObject ARCoreCamera;
    public ARCoreController ARCoreController;
    public UIController UIController;
    public NetworkManagerController NetworkManagerController;


    void Start()
    {
        if (!PlayerPrefs.HasKey("PlayerName"))
        {
            PlayerName = "Dein Name";
        }
        expeditions = new List<Expedition>();
        Expedition expedition1 = new Expedition("TengaduruExpedition");

        expeditions.Add(expedition1);
        UICamera.SetActive(true);
        ARCoreCamera.SetActive(false);
        ARCoreController.gameObject.SetActive(false);
    }

    void Update()
    {
        //if the back button of the smartphone was pressed
        if (Input.GetKey(KeyCode.Escape))
        {
            //and the Application is in AR-mode (Hosting)
            if(ARCoreController.getApplicationMode() == ARCoreController.ApplicationMode.Hosting)
            {
                UIController.BackButtonWasClicked();    //Hide and activate screens
                UICamera.SetActive(true);
                NetworkManagerController.DestroyRoom();
                ARCoreCamera.SetActive(false);
                ARCoreController.QuitARMode();
                ARCoreController.gameObject.SetActive(false);
            }

        }
    }

    public void deactivateAllExpeditions()
    {
        foreach(Expedition expedition in expeditions)
        {
            expedition.active = false;
        }
    }

    public void StartExpedition(string expeditionName/*besser wäre: Expedition expedition*/)
    {
        UICamera.SetActive(false);
        ARCoreCamera.SetActive(true);
        ARCoreController.gameObject.SetActive(true);
        ARCoreController.OnEnterHostingModeClick();
    }

    public void JoinExpedition(string expeditionName/*besser wäre: Expedition expedition*/)
    {
        UICamera.SetActive(false);
        ARCoreCamera.SetActive(true);
        ARCoreController.gameObject.SetActive(true);
        ARCoreController.OnEnterResolvingModeClick();
    }

    public void LeaveApp()
    {
        Application.Quit();
    }
}
