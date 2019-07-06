using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class AugmentedImageVisualizer : MonoBehaviour
{
    //The augmented image to visualize. Will be set by the ARCoreController.
    public AugmentedImage Image;
    //The prefabs to visualize on different images. Which visualized object for which image is used depends on position on imageList -> position 0: image in databse with position:0...
    //public List<GameObject> VisualizedObjects;

    //private GameObject augmentedObject = null;

    /// <summary>
    /// A model for the lower left corner of the frame to place when an image is detected.
    /// </summary>
    public GameObject FrameLowerLeft;

    /// <summary>
    /// A model for the lower right corner of the frame to place when an image is detected.
    /// </summary>
    public GameObject FrameLowerRight;

    /// <summary>
    /// A model for the upper left corner of the frame to place when an image is detected.
    /// </summary>
    public GameObject FrameUpperLeft;

    /// <summary>
    /// A model for the upper right corner of the frame to place when an image is detected.
    /// </summary>
    public GameObject FrameUpperRight;

    void Update()
    {
        ////if no image is tracked, disable visualized object
        //if(Image == null || Image.TrackingState != TrackingState.Tracking || Image.TrackingMethod != AugmentedImageTrackingMethod.FullTracking)
        //{
        //    Debug.Log("Image is not in sight");
        //    augmentedObject.SetActive(false);
        //    return;
        //}

        ////if no prefab was already created, create one
        //if(augmentedObject == null)
        //{
        //    augmentedObject = Instantiate(VisualizedObjects[Image.DatabaseIndex]);
        //    augmentedObject.transform.SetParent(this.transform);
        //}

        ////change position to image position in every frame
        //Vector3 midPoint = Image.CenterPose.position;
        //augmentedObject.transform.localPosition = midPoint;
        //augmentedObject.SetActive(true);
        //Debug.Log("Virtual Object for image is shown!");


        if (Image == null || Image.TrackingState != TrackingState.Tracking || Image.TrackingMethod != AugmentedImageTrackingMethod.FullTracking)
        {
            Debug.Log("Image is not in sight");
            FrameLowerLeft.SetActive(false);
            FrameLowerRight.SetActive(false);
            FrameUpperLeft.SetActive(false);
            FrameUpperRight.SetActive(false);
            return;
        }

        float halfWidth = Image.ExtentX / 2;
        float halfHeight = Image.ExtentZ / 2;
        FrameLowerLeft.transform.localPosition =
            (halfWidth * Vector3.left) + (halfHeight * Vector3.back);
        FrameLowerRight.transform.localPosition =
            (halfWidth * Vector3.right) + (halfHeight * Vector3.back);
        FrameUpperLeft.transform.localPosition =
            (halfWidth * Vector3.left) + (halfHeight * Vector3.forward);
        FrameUpperRight.transform.localPosition =
            (halfWidth * Vector3.right) + (halfHeight * Vector3.forward);

        FrameLowerLeft.SetActive(true);
        FrameLowerRight.SetActive(true);
        FrameUpperLeft.SetActive(true);
        FrameUpperRight.SetActive(true);
        Debug.Log("Virtual Object for image is shown!");
    }
}

