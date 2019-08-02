using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class ARImageVisualizer : MonoBehaviour
{
    /// <summary>
    /// The augmented image to visualize. Will be set by the ARCoreController.
    /// </summary>
    public AugmentedImage Image;

    /// <summary>
    /// The virtual object to be shown.
    /// </summary>
    private GameObject augmentedObject = null;

    /// <summary>
    /// The network ID of the virtual object.
    /// </summary>
#pragma warning disable 618
    private UnityEngine.Networking.NetworkInstanceId augmentedObjectNetworkID;
#pragma warning restore 618

    private bool requestToSpawn = false;
    private LocalPlayerControllerClass localPlayer;

    private void Start()
    {
        localPlayer = GameObject.Find("LocalPlayer").GetComponent<LocalPlayerControllerClass>();
        //Debug.Log("Visualizer started");
    }

    void Update()
    {

        //if no prefab was already created, create one
        if (augmentedObject == null && !requestToSpawn)
        {
            //Debug.Log("Visualizer wants to spawn tool");
            localPlayer.CmdSpawnTool(Image.DatabaseIndex);      //Send request for Instantiating tool to server
            requestToSpawn = true;
            return;
        }

        //if the request has not been resolved yet
        if (requestToSpawn)
        {
            return;
        }

        //if no image is tracked, disable visualized object -> send a request to the server which propagates the request to all clients
        if (Image == null || Image.TrackingState != TrackingState.Tracking || Image.TrackingMethod != AugmentedImageTrackingMethod.FullTracking)
        {
            //if augmented object was prevously enabled -> disable it
            if (augmentedObject.activeSelf)
            {
                augmentedObject.GetComponent<Tool>().ChangeIsUsed(false);       //change "isUsed" to false when augmentedObject is not in sight of camera (for when image is not detected while inside a block)
                localPlayer.CmdSetGameObjectState(augmentedObjectNetworkID, false);
            }
            return;
        }

        //if augmented object was prevously disabled -> enable it
        if (!augmentedObject.activeSelf)
        {
            localPlayer.CmdSetGameObjectState(augmentedObjectNetworkID, true);
        }
    }

    /// <summary>
    /// Sets the position and rotation of this visualizer and all children (with a tool)
    /// </summary>
    /// <param name="imagePose">The correct pose of the augmented image in world coordinates</param>
    public void SetTransform(Pose imagePose)
    {
        if (imagePose == null)
        {
            return;
        }
        transform.localPosition = imagePose.position;
        transform.localRotation = imagePose.rotation;
    }

    /// <summary>
    /// Callback when the tool has spawned and anchored itself to this transform as a child.
    /// </summary>
    public void ToolHasSpawned()
    {
        requestToSpawn = false;
        augmentedObject = transform.GetChild(0).gameObject;
#pragma warning disable 618
        augmentedObjectNetworkID = augmentedObject.GetComponent<UnityEngine.Networking.NetworkIdentity>().netId;
#pragma warning restore 618
        if (augmentedObject == null)
        {
            Debug.LogError("Tool was not spawned correctly!");
        }
    }

}

