using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VuforiaController : MonoBehaviour
{
    /// <summary>
    /// The UI Controller.
    /// </summary>
    public NetworkManagerController NetworkManagerController;

    public UIController UIController;
    public GameManager GameManager;

    /// <summary>
    /// The root for Vuforia-specific GameObjects in the scene.
    /// </summary>
    public GameObject VuforiaRoot;

    public bool ARMode = false;


    void Start()
    {
        // A Name is provided to the Game Object so it can be found by other Scripts
        // instantiated as prefabs in the scene.
        gameObject.name = "VuforiaController";
        VuforiaRoot.SetActive(false);
        ARMode = false;
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Callback that happens when the client successfully connected to the server.
    /// </summary>
    public void OnConnectedToServer()
    {
        UIController.ShowMessage("Finde die Expedition indem Du die Umgebung nach dem speziellen Bild absuchst.", 10);
        UIController.ShowHandAnimation(5f);
    }

    public void StartExpedition()
    {
        VuforiaRoot.SetActive(true);
        ARMode = true;
    }

    /// <summary>
    /// Quit only the ARMode. 
    /// </summary>
    public void QuitARMode()
    {
        _ResetStatus();
    }

    private void _ResetStatus()
    {
        VuforiaRoot.SetActive(false);
        ARMode = false;
    }


    ///// <summary>
    ///// Forward a displayable message to the UIController.
    ///// </summary>
    ///// <param name="message">The message</param>
    ///// <param name="seconds">The duration it will be shown</param>
    //public void ShowMessage(string message, int seconds)
    //{
    //    UIController.ShowMessage(message, seconds);
    //}
}
