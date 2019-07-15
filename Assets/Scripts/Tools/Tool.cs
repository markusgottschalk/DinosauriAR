using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 618
public class Tool : NetworkBehaviour
#pragma warning restore 618
{
    public ToolType ToolType;
    private ARImageVisualizer arImageVisualizer;
    private ARCoreController arCoreController;

    [HideInInspector]
#pragma warning disable 618
    [SyncVar]
#pragma warning restore 618
    public int imageDatabaseindex;


    public override void OnStartAuthority()
    {
        //Set the own tool object spawned by the server as a child of the corresponding visualizer
        Debug.Log("Tool has spawned");
        arCoreController = GameObject.Find("ARCoreController").GetComponent<ARCoreController>();
        arImageVisualizer = arCoreController.visualizers[imageDatabaseindex];

        switch (imageDatabaseindex)
        {
            case 0:
            case 1: ToolType = ToolType.ROCKHAMMER;
                break;
            case 2: ToolType = ToolType.SENSOR;
                break;
            case 3: ToolType = ToolType.SHOVEL;
                break;
            default:
                Debug.LogError("No Tooltype could be assigned.");
                break;
        }

        transform.parent = arImageVisualizer.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        arImageVisualizer.ToolHasSpawned();
    }

    //TODO: if hasAuthority -> collider
}
