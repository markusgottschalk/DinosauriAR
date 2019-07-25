using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private CloudAnchorController rootAnchorController;

    /// <summary>
    /// The rate with which the block will disappear when the brush is used. Depends on localScale.y
    /// </summary>
    private float explorationRate;

    /// <summary>
    /// The exploration status when the brush is used. Will shrink to the ground and gets destroyed when 0 is reached.
    /// </summary>
    private int explorationStatus;
    public int ExplorationStatus
    {
        get { return explorationStatus; }
        set
        { 
            explorationStatus = Mathf.Clamp(value, 0, 150);
            gameObject.transform.localScale.Set(gameObject.transform.localScale.x, gameObject.transform.localScale.y - explorationRate, gameObject.transform.localScale.z);
            if (explorationStatus == 150)
            {
                destroy(true);
            }
        }
    }

    /// <summary>
    /// Checks whether the block has been analyzed by the sensor. 
    /// </summary>
    private bool analyzed = false;
    public bool Analyzed
    {
        get { return analyzed; }
        private set { analyzed = value; }
    }

    /// <summary>
    /// The analyze status of the block. If 100% is reached, show whether bones are available or not. 
    /// </summary>
    private int percentAnalyzed;
    public int PercentAnalyzed
    {
        get { return percentAnalyzed; }
        set
        {
            percentAnalyzed = Mathf.Clamp(value, 0, 100);
            if (percentAnalyzed == 100)
            {
                Analyzed = true;
                if (BonesAvailable)
                {
                    transform.GetChild(0).GetComponent<Renderer>().materials[1] = boneMaterials[0];     //material 2 is for available bones
                }
                else
                {
                    transform.GetChild(0).GetComponent<Renderer>().materials[1] = boneMaterials[1];
                }
                
            }
        }
    }

    /// <summary>
    /// Whether bones are in this block or not. 
    /// </summary>
    [SerializeField]
    private bool bonesAvailable;
    public bool BonesAvailable
    {
        get { return bonesAvailable; }
        set { bonesAvailable = value; }
    }

    /// <summary>
    /// The material of the block. Only the right tools can be used with the right material.
    /// </summary>
    [SerializeField]
    private BlockMaterial blockMaterial;
    public BlockMaterial BlockMaterial
    {
        get { return blockMaterial; }
        private set { blockMaterial = value; }
    }

    /// <summary>
    /// The materials used for showing the different cracks. 
    /// </summary>
    [SerializeField]
    private List<Material> crackMaterials = default;

    /// <summary>
    /// The materials which show whether there is a bone inside or not. Material 1: bone, Material 2: no bone.
    /// </summary>
    [SerializeField]
    private List<Material> boneMaterials = default;

    /// <summary>
    /// Shows how many times the block needs to get worked by a tool. 
    /// </summary>
    [SerializeField]
    private int maxStatus = default;

    /// <summary>
    /// The current status of the block when worked by a tool (not the brush).
    /// </summary>
    private int currentStatus = 0;
    public int CurrentStatus
    {
        get { return currentStatus; }
        set {
                if (value >= maxStatus)
                {
                    currentStatus = maxStatus;
                    destroy(false);
                }
                else
                {
                    currentStatus = value;

                    //render block differently (with cracks -> the higher the current status, the more cracks appear)
                    //the first material is the one for cracks
                    transform.GetChild(0).GetComponent<Renderer>().materials[0] = crackMaterials[CurrentStatus];
                    //Debug.Log("Block: " + name + ", current block material: " + this.GetComponent<Renderer>().material.name + " material: " + BlockMaterial);
                }
        }

    }

    void Start()
    {
        transform.GetChild(0).GetComponent<Renderer>().materials[0] = crackMaterials[CurrentStatus];

        rootAnchorController = transform.root.GetComponent<CloudAnchorController>();
        //Debug.Log(name + " has BlockMaterial: " + BlockMaterial);
        PercentAnalyzed = 0;

        ExplorationStatus = 0;
        explorationRate = gameObject.transform.localScale.y / 150;
    }

    /// <summary>
    /// Destroys the block. When there are bones, destroy the parent of the block and therefore destroy the bones but only when using another tool than the brush. The brush only destroys the block, never anything else
    /// </summary>
    /// <param name="withBrush">Checks if it gets destroyed with brush or another tool</param>
    private void destroy(bool withBrush)
    {
        if (!withBrush)
        {
            if (BonesAvailable)
            {
                Destroy(transform.parent.gameObject);
                return;
            }
        }
        Destroy(this.gameObject);
    }

    public void ChangeDestroyStatus(int statusChange)
    {
        rootAnchorController.CmdSetBlock(this.gameObject.name, CurrentStatus + statusChange);   //send already changed status to server
    }

}
