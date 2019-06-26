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
        ARCoreCamera.SetActive(true); //das eigentlich besser von ARCoreController setzen lassen?!
        ARCoreController.gameObject.SetActive(true);
        ARCoreController.OnEnterHostingModeClick();
    }

    public void LeaveApp()
    {
        Application.Quit();            //TODO: Quit is in CloudAnchorsController...?
                                       //CloudAnchorsController._DoQuit(); or isQuitting = true...
    }
}
