using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush : Tool
{
    private GameManager gameManager;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (hasAuthority)
        {
            if (other.CompareTag("Block"))
            {
                Block block = other.gameObject.transform.parent.GetComponent<Block>();
                Debug.Log("Colliding with " + block.BlockMaterial);
                foreach (BlockMaterial blockMaterial in gameManager.toolsForBlocks[ToolType])
                {
                    if (block.BlockMaterial == blockMaterial)
                    {
                        Debug.Log("TOOL: BlockMaterial: " + block.BlockMaterial + ", own tooltype is for: " + gameManager.toolsForBlocks[ToolType]);
                        block.ExplorationStatus += 1;
                    }
                }
            }
        }
    }
}
