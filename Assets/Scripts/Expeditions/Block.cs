using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Block : MonoBehaviour
{
    /// <summary>
    /// The cloud anchor.
    /// </summary>
    private CloudAnchorController cloudAnchor;

    /// <summary>
    /// The materials of the block. Material 1 is for <see cref="BlockMaterial"/> and cracks <see cref="CurrentStatus"/>. Material 2 is for analyzed block <see cref="PercentAnalyzed"/> and showing bones or not.
    /// </summary>
    private Material[] currentMaterials;

    /// <summary>
    /// The rate with which the block will disappear when the brush is used. Depends on localScale.up
    /// </summary>
    private float explorationRate;

    [SerializeField]
    private int explorationStatusMax = 0;
    [SerializeField]
    private float lerpTimeForScale = 0;

    private Vector3 startScale;

    /// <summary>
    /// The exploration status when the brush is used. Will shrink to the ground and gets destroyed when 0 is reached.
    /// </summary>
    private int explorationStatus = 0;
    public int ExplorationStatus
    {
        get { return explorationStatus; }
        set
        {
            explorationStatus = Mathf.Clamp(value, 0, explorationStatusMax);

            Vector3 newScale = startScale - (this.gameObject.transform.up * (explorationRate * explorationStatus));
            StartCoroutine(lerpToNewScale(gameObject.transform.localScale, newScale));

            if (explorationStatus == explorationStatusMax)
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

    [SerializeField]
    private int analyzeMax = 0;

    /// <summary>
    /// The analyze status of the block. If 100% is reached, show whether bones are available or not. 
    /// </summary>
    private int percentAnalyzed = 0;
    public int PercentAnalyzed
    {
        get { return percentAnalyzed; }
        set
        {
            percentAnalyzed = Mathf.Clamp(value, 0, analyzeMax);
            if (percentAnalyzed == analyzeMax)
            {
                Analyzed = true;
                if (BonesAvailable)
                {
                    changeMaterial(null, boneMaterials[0]);     //material 2 is for available bones
                }
                else
                {
                    changeMaterial(null, boneMaterials[1]);
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
    /// The materials which show whether there is a bone inside or not. Material 0: bone, Material 1: no bone, Material 2: neutralBone.
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
                    changeMaterial(crackMaterials[CurrentStatus], null);
                    //Debug.Log("Block: " + name + ", current block material: " + this.GetComponent<Renderer>().material.name + " material: " + BlockMaterial);
                }
        }

    }

    void Start()
    {
        cloudAnchor = this.gameObject.transform.root.gameObject.GetComponent<CloudAnchorController>();

        currentMaterials = transform.GetChild(0).GetComponent<MeshRenderer>().materials;
        changeMaterial(crackMaterials[CurrentStatus], boneMaterials[2]);

        explorationRate = Vector3.Dot(gameObject.transform.up, gameObject.transform.localScale) / (float)explorationStatusMax;

        startScale = this.gameObject.transform.localScale;
    }

    /// <summary>
    /// Destroys the block. When there are bones, destroy the parent of the block and therefore destroy the bones but only when using another tool than the brush. The brush only destroys the block, never anything else
    /// </summary>
    /// <param name="withBrush">Checks if it gets destroyed with brush or another tool</param>
    private void destroy(bool withBrush)
    {
        if (!withBrush)     //if block is completely destroyed without the brush
        {
            if (BonesAvailable)
            {
                transform.parent.gameObject.GetComponent<Bones>().BlockDestroyed(false);
                Destroy(transform.parent.gameObject);
                return;
            }
        }

        if (BonesAvailable)
        {
            transform.parent.gameObject.GetComponent<Bones>().BlockDestroyed(true);
        }

        Destroy(this.gameObject);
    }

    /// <summary>
    /// Command to server send with already changed status of cracks.
    /// </summary>
    /// <param name="statusChange">The new status</param>
    public void ChangeDestroyStatus(int statusChange)
    {
        //Debug.Log("BLOCK: change current status from " + CurrentStatus + " to " + (CurrentStatus + statusChange));
        cloudAnchor.CmdSetBlock(this.gameObject.name, CurrentStatus + statusChange);
    }

    /// <summary>
    /// Command to server with already changed status of percent analyzed.
    /// </summary>
    /// <param name="statusChange">The new percent analyzed status</param>
    public void ChangePercentAnalyzed(int statusChange)
    {
        cloudAnchor.CmdSetBlockPercentAnalyzed(this.gameObject.name, PercentAnalyzed + statusChange);
    }

    /// <summary>
    /// Command to server with already changed status of exploration state (localScale.up).
    /// </summary>
    /// <param name="statusChange">The new exploration status</param>
    public void ChangeExplorationStatus(int statusChange)
    {
        cloudAnchor.CmdSetBlockExplorationStatus(this.gameObject.name, ExplorationStatus + statusChange);
    }

    /// <summary>
    /// Change a specific material.
    /// </summary>
    /// <param name="firstMaterial">The first material</param>
    /// <param name="secondMaterial">The second material</param>
    private void changeMaterial(Material firstMaterial, Material secondMaterial)
    {
        if(firstMaterial == null)
        {
            firstMaterial = currentMaterials[0];
        }
        if(secondMaterial == null)
        {
            secondMaterial = currentMaterials[1];
        }
        currentMaterials[0] = firstMaterial;
        currentMaterials[1] = secondMaterial;
        transform.GetChild(0).GetComponent<MeshRenderer>().materials = currentMaterials;
    }

    /// <summary>
    /// Show smooth animation when updating localScale.
    /// </summary>
    /// <param name="oldScale">The old scale</param>
    /// <param name="newScale">The new scale</param>
    private IEnumerator lerpToNewScale(Vector3 oldScale, Vector3 newScale)
    {
        float elapsedTime = 0;
        while(elapsedTime < lerpTimeForScale)
        {
            gameObject.transform.localScale = Vector3.Lerp(oldScale, newScale, (elapsedTime / lerpTimeForScale));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.localScale = newScale;
        yield return null;
    }

    public int getAnalyzeMax()
    {
        return analyzeMax;
    }

}
