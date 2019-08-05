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
    private GameObject lastWorkedBlock = null;

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
                arCoreController.ShowMessage("Mit der Bürste kannst du Blöcke aus Sand und Stein langsam abtragen. Halte dafür die Bürste in den Block. Dieser Vorgang ist sehr schonend und kann Knochen zu Tage bringen!", 20);
                break;
            case 1:
            case 2: ToolType = ToolType.ROCKHAMMER;
                arCoreController.ShowMessage("Mit dem Geologenhammer kannst du Blöcke aus Stein zerstören. Dafür muss die Schaufel die Blöcke berühren und wieder Abstand nehmen. Führe dies fort um den Block zu zerstören. \nVORSICHT: Diese Aktion kann auch die Knochen in den Blöcken zerstören. Wenn du sicher bist, dass ein Knochen enthalten ist, benutze lieber die Bürste.", 20);
                break;
            case 3: ToolType = ToolType.SENSOR;
                arCoreController.ShowMessage("Mit dem Sensor kannst du Blöcke analysieren und sehen, ob Knochen enthalten sind. Halte dafür den Sensor so, dass der orangene Strahl auf einen Block fällt. Wenn die gelbe Anzeige rechts unten komplett gefüllt ist, siehst du das Ergebnis.", 20);
                break;
            case 4: ToolType = ToolType.SHOVEL;
                arCoreController.ShowMessage("Mit der Schaufel kannst du Blöcke aus Sand zerstören. Dafür muss die Schaufel die Blöcke berühren und wieder Abstand nehmen. Führe dies fort um den Block zu zerstören. \nVORSICHT: Diese Aktion kann auch die Knochen in den Blöcken zerstören. Wenn du sicher bist, dass ein Knochen enthalten ist, benutze lieber die Bürste.", 20);
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
        if(lastWorkedBlock == null)
        {
            isUsed = false;
        }

        //Debug.Log("Is the Tool already in use? " + isUsed);

        if (!isUsed)
        {
            isUsed = true;

            //Debug.Log("Has the client the authority over the tool? " + hasAuthority);

            if (hasAuthority)
            {
                //Debug.Log("What is the tag of the colliding object: " + other.tag);

                if (other.CompareTag("BlockGraphics"))
                {
                    Block block = other.gameObject.transform.parent.GetComponent<Block>();
                    //Debug.Log("Colliding with " + block.BlockMaterial);
                    foreach (BlockMaterial blockMaterial in gameManager.toolsForBlocks[ToolType])
                    {
                        //Debug.Log("TOOL: BlockMaterial: " + block.BlockMaterial + ", own tooltype is for: " + blockMaterial);
                        if (block.BlockMaterial == blockMaterial)
                        {
                            block.ChangeDestroyStatus(1);
                            lastWorkedBlock = block.gameObject;
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
