using GoogleARCore.Examples.CloudAnchors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The name of the player. Can be modified in-app. Saves and loads from the Player Preferences. 
    /// </summary>
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
            PlayerPrefs.SetString("PlayerName", playerName);
        }
    }

    /// <summary>
    /// A list of all expeditions.
    /// </summary>
    public List<Expedition> expeditions;

    /// <summary>
    /// The camera which will show the UI elements.
    /// </summary>
    public GameObject UICamera;

    /// <summary>
    /// The ARCore Controller.
    /// </summary>
    public ARCoreController ARCoreController;

    /// <summary>
    /// The UI Controller.
    /// </summary>
    public UIController UIController;

    /// <summary>
    /// The NetworkManager Controller.
    /// </summary>
    public NetworkManagerController NetworkManagerController;

    /// <summary>
    /// The list which describes which tools can work on which block materials.
    /// </summary>
    public Dictionary<ToolType, List<BlockMaterial>> toolsForBlocks;


    void Start()
    {
        //Define a name so that other gameobjects can find it
        gameObject.name = "GameManager";

        //If not player name could be loaded, set name to default
        if (!PlayerPrefs.HasKey("PlayerName"))
        {
            PlayerName = "Dein Name";
        }

        //Create the list of all expeditions and initialize it
        expeditions = new List<Expedition>();
        Expedition expedition1 = new Expedition("TendaguruExpedition");
        expeditions.Add(expedition1);

        //Start with UI
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
                //QuitExpedition();     //doesn't work yet
            }

        }
    }

    /// <summary>
    /// Deactivate all expeditions.
    /// </summary>
    public void DeactivateAllExpeditions()
    {
        foreach(Expedition expedition in expeditions)
        {
            expedition.active = false;
        }
    }

    /// <summary>
    /// Start specific expedition with name. End the UI and start AR.
    /// </summary>
    /// <param name="expeditionName">The name of the expedition</param>
    public void StartExpedition(string expeditionName)
    {
        UICamera.SetActive(false);
        ARCoreController.OnEnterHostingModeClick();
    }

    /// <summary>
    /// Join a specific expedition with name. End the UI and start AR.
    /// </summary>
    /// <param name="expeditionName"The name of the expedition></param>
    public void JoinExpedition(string expeditionName)
    {
        UICamera.SetActive(false);
        ARCoreController.OnEnterResolvingModeClick();
    }

    /// <summary>
    /// Quit the active expedition. End the AR mode, destroy the networked room and start the UI. 
    /// </summary>
    public void QuitExpedition()
    {
        ARCoreController.QuitARMode();
        UICamera.SetActive(true);
        NetworkManagerController.QuitMatch();
    }

    /// <summary>
    /// Leaves the app.
    /// </summary>
    public void LeaveApp()
    {
        Application.Quit();
    }
}
