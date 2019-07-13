using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class ARImageVisualizer : MonoBehaviour
{
    //The augmented image to visualize. Will be set by the ARCoreController.
    public AugmentedImage Image;


    private GameObject augmentedObject = null;
#pragma warning disable 618
    private UnityEngine.Networking.NetworkInstanceId augmentedObjectNetworkID;
#pragma warning restore 618
    private bool requestToSpawn = false;
    //private ARCoreWorldOriginHelperClass ARCoreWorldOriginHelperClass;
    private LocalPlayerControllerClass localPlayer;

    private void Start()
    {
        //ARCoreWorldOriginHelperClass = GameObject.Find("ARCoreWorldOriginHelperClass").GetComponent<ARCoreWorldOriginHelperClass>();
        localPlayer = GameObject.Find("LocalPlayer").GetComponent<LocalPlayerControllerClass>();
        Debug.LogError("Visualizer started");
    }

    void Update()
    {

        //if no prefab was already created, create one
        if (augmentedObject == null && !requestToSpawn)
        {
            localPlayer.CmdSpawnTool(Image.DatabaseIndex);      //Send request for Instantiating tool to server
            requestToSpawn = true;
            return;
        }

        //if the request has not be resolved yet
        if (requestToSpawn)
        {
            return;
        }

        //if no image is tracked, disable visualized object -> send a request to the server which propagates the request to all clients
        if (Image == null || Image.TrackingState != TrackingState.Tracking || Image.TrackingMethod != AugmentedImageTrackingMethod.FullTracking)
        {
            localPlayer.CmdIsToolActive(false, augmentedObjectNetworkID);
            return;
        }

        localPlayer.CmdIsToolActive(true, augmentedObjectNetworkID);
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

