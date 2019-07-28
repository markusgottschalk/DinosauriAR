using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush : Tool
{
    public float SecondsToBrush;
    private float timer = 0;

    /// <summary>
    /// Override base methods. Brush should not crack the block...
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        
    }
    private void OnTriggerExit(Collider other)
    {
        
    }

    /// <summary>
    /// Checks the collisions. If the client has authority, change status of block if this is the right tool. 
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if (hasAuthority)
        {
            if (other.CompareTag("BlockGraphics"))
            {
                Block block = other.gameObject.transform.parent.GetComponent<Block>();
                Debug.Log("Colliding with " + block.BlockMaterial);
                foreach (BlockMaterial blockMaterial in gameManager.toolsForBlocks[ToolType])
                {
                    //Debug.Log("TOOL: BlockMaterial: " + block.BlockMaterial + ", own tooltype is for: " + blockMaterial);
                    if (block.BlockMaterial == blockMaterial)
                    {
                        timer += Time.deltaTime;
                        if(timer >= SecondsToBrush)
                        {
                            block.ChangeExplorationStatus(1);
                            timer = 0;
                        }
                    }
                }
            }
        }
    }
}
