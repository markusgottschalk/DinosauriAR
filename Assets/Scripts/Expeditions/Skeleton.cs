using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    /// <summary>
    /// All the bones of the hidden dinosaur.
    /// </summary>
    private List<Bones> bonesOfDinosaur;

    /// <summary>
    /// All found bones.
    /// </summary>
    private List<Bones> bonesFound;

    /// <summary>
    /// All destroyed bones.
    /// </summary>
    private List<Bones> bonesDestroyed;

    private CloudAnchorController cloudAchorController;

    void Start()
    {
        cloudAchorController = transform.root.gameObject.GetComponent<CloudAnchorController>();
        bonesOfDinosaur = new List<Bones>();
        bonesFound = new List<Bones>();
        bonesDestroyed = new List<Bones>();

        foreach(Transform child in transform)
        {
            bonesOfDinosaur.Add(child.gameObject.GetComponent<Bones>());
        }
    }

    public void AddFoundBones(Bones bones)
    {
        bonesFound.Add(bones);
        checkForEnd();
    }

    public void AddDestroyedBones(Bones bones)
    {
        bonesDestroyed.Add(bones);
        checkForEnd();
    }

    /// <summary>
    /// Check if all bones are found or destroyed.
    /// </summary>
    private void checkForEnd()
    {
        if((bonesFound.Count + bonesDestroyed.Count) == bonesOfDinosaur.Count)
        {
            cloudAchorController.EndExpedition(bonesFound.Count >= bonesDestroyed.Count);   //if more bones found than destroyed the expedition is a success
        }
    }
}
