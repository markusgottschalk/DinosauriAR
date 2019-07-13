using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private bool explored = false;

    [SerializeField]
    private bool BonesAvailable;
    [SerializeField]
    private BlockMaterial BlockMaterial;

    private Dictionary<BlockMaterial, List<ToolType>> toolsForBlocks;

    [SerializeField]
    private int maxStatus;
    private int currentStatus = 0;
    public int CurrentStatus
    {
        get { return currentStatus; }
        set {
                if (value >= maxStatus)
                {
                    currentStatus = maxStatus;
                    explored = true;
                    isExplored();
                }
                else
                {
                    currentStatus = value;
                    //TODO: render block differently (with cracks, like in minecraft -> the more the current status, the more cracks)
                }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        toolsForBlocks = new Dictionary<BlockMaterial, List<ToolType>>();
        toolsForBlocks.Add(BlockMaterial.ROCK, new List<ToolType>() { ToolType.ROCKHAMMER });
        toolsForBlocks.Add(BlockMaterial.SAND, new List<ToolType>() { ToolType.SHOVEL });
    }


    private void OnCollisionEnter(Collision collision)
    {
        Tool collidingTool = collision.gameObject.GetComponent<Tool>();

        foreach(ToolType toolType in toolsForBlocks[BlockMaterial])
        {
            if(collidingTool.ToolType == toolType)
            {
                CurrentStatus++;
                return;
            }
        }
    }

    private void isExplored()
    {
        gameObject.SetActive(false);    //TODO: or destroy...?
    }
}
