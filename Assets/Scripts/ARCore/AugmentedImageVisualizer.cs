using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class AugmentedImageVisualizer : MonoBehaviour
{
    //The augmented image to visualize. Will be set by the ARCoreController.
    public AugmentedImage Image;
    //The prefabs to visualize on different images. Which visualized object for which image is used depends on position on imageList -> position 0: image in databse with position:0...
    public List<GameObject> VisualizedObjects;

    private GameObject augmentedObject = null;

    void Update()
    {
        //if no image is tracked, disable visualized object
        if(Image == null || Image.TrackingState != TrackingState.Tracking || Image.TrackingMethod != AugmentedImageTrackingMethod.FullTracking)
        {
            augmentedObject.SetActive(false);
            return;
        }

        //if no prefab was already created, create one
        if(augmentedObject == null)
        {
            augmentedObject = Instantiate(VisualizedObjects[Image.DatabaseIndex]);
            augmentedObject.transform.SetParent(this.transform);
        }

        //change position to image position in every frame
        augmentedObject.transform.localPosition = Vector3.zero;
        augmentedObject.SetActive(true);
    }
}
