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
    public GameManager gameManager;

    //tools work only for one block
    private bool isUsed;

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

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        switch (imageDatabaseindex)
        {
            case 0: ToolType = ToolType.BRUSH;
                break;
            case 1:
            case 2: ToolType = ToolType.ROCKHAMMER;
                break;
            case 3: ToolType = ToolType.SENSOR;
                break;
            case 4: ToolType = ToolType.SHOVEL;
                break;
            case 5: ToolType = ToolType.SHOVEL;
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

    /// <summary>
    /// Checks the collisions. If the tool is not in use in another block, the client has authority, change status of block if this is the right tool. 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!isUsed)
        {
            isUsed = true;

            if (hasAuthority)
            {
                if (other.CompareTag("Block"))
                {
                    Block block = other.gameObject.transform.parent.GetComponent<Block>();
                    //Debug.Log("Colliding with " + block.BlockMaterial);
                    foreach(BlockMaterial blockMaterial in gameManager.toolsForBlocks[ToolType])
                    {
                        //Debug.Log("TOOL: BlockMaterial: " + block.BlockMaterial + ", own tooltype is for: " + blockMaterial);
                        if (block.BlockMaterial == blockMaterial)
                        {
                            block.ChangeDestroyStatus(1);
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isUsed = false;
    }

    public void ChangeIsUsed(bool state)
    {
        isUsed = state;
    }
    
}
