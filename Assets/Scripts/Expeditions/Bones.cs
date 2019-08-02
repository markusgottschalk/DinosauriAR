using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bones : MonoBehaviour
{
    /// <summary>
    /// The specific message for each section of bones when found. 
    /// </summary>
    [SerializeField]
    private string messageAboutBones = "";

    private CloudAnchorController cloudAnchorController;

    /// <summary>
    /// The skeleton to which the bones belong to.
    /// </summary>
    private Skeleton skeleton;

    private void Start()
    {
        cloudAnchorController = transform.root.gameObject.GetComponent<CloudAnchorController>();
        skeleton = transform.parent.GetComponent<Skeleton>();
    }

    /// <summary>
    /// Checks whether a block with bones is completely destroyed (without the brush) or not (with brush -> bones are still there). 
    /// </summary>
    /// <param name="withBrush">The parameter to show whether a brush was used.</param>
    public void BlockDestroyed(bool withBrush)
    {
        if (withBrush)
        {
            cloudAnchorController.ShowMessage(messageAboutBones, 10);
            skeleton.AddFoundBones(this);
        }
        else
        {
            skeleton.AddDestroyedBones(this);
        }

    }
}
