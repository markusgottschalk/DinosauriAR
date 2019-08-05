//-----------------------------------------------------------------------
// <copyright file="CloudAnchorsController.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//
//THIS SCRIPT WAS MODIFIED.
//-----------------------------------------------------------------------


using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

/// <summary>
/// Controller for the Cloud Anchor. Handles the ARCore lifecycle.
/// </summary>
public class ARCoreController : MonoBehaviour
{
    [Header("ARCore")]

    /// <summary>
    /// The UI Controller.
    /// </summary>
    public NetworkManagerController NetworkManagerController;

    public UIController UIController;
    public GameManager GameManager;

    /// <summary>
    /// The root for ARCore-specific GameObjects in the scene.
    /// </summary>
    public GameObject ARCoreRoot;

    /// <summary>
    /// The helper that will calculate the World Origin offset when performing a raycast or
    /// generating planes.
    /// </summary>
    public ARCoreWorldOriginHelperClass ARCoreWorldOriginHelperClass;


    /// <summary>
    /// Indicates whether the Origin of the new World Coordinate System, i.e. the Cloud Anchor,
    /// was placed.
    /// </summary>
    private bool m_IsOriginPlaced = false;

    /// <summary>
    /// Indicates whether the Anchor was already instantiated.
    /// </summary>
    private bool m_AnchorAlreadyInstantiated = false;

    /// <summary>
    /// Indicates whether the Cloud Anchor finished hosting.
    /// </summary>
    private bool m_AnchorFinishedHosting = false;

    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error,
    /// otherwise false.
    /// </summary>
    private bool m_IsQuitting = false;

    /// <summary>
    /// The anchor component that defines the shared world origin.
    /// </summary>
    private Component m_WorldOriginAnchor = null;

    /// <summary>
    /// The last pose of the hit point from AR hit test.
    /// </summary>
    private Pose? m_LastHitPose = null;

    /// <summary>
    /// The current cloud anchor mode.
    /// </summary>
    private ApplicationMode m_CurrentMode = ApplicationMode.Ready;

    public GameObject ARCoreDevice;

    /// <summary>
    /// The currently active ARCore Session.
    /// </summary>
    private ARCoreSession arCoreSession;

    /// <summary>
    /// The prefab for the ARCore Session Config.
    /// </summary>
    [SerializeField]
    private ARCoreSessionConfig arCoreSessionConfigPrefab = null;

    /// <summary>
    /// The prefab for the preview (the ground) of the expedition.
    /// </summary>
    public GameObject GroundPrefab;

    /// <summary>
    /// The preview of the expedition.
    /// </summary>
    private GameObject groundForExpedition;

    /// <summary>
    /// A Touch of the user.
    /// </summary>
    private Touch touch;

    /// <summary>
    /// Checks if the touch was the first or not (for showing the preview of the expedition).
    /// </summary>
    private bool firstTouch = false;

    /// <summary>
    /// A prefab for visualizing an AugmentedImage.
    /// </summary>
    public ARImageVisualizer ARImageVisualizer;

    /// <summary>
    /// Dictionary for existing visualizers for images.
    /// </summary>
    public Dictionary<int, ARImageVisualizer> visualizers = new Dictionary<int, ARImageVisualizer>();

    /// <summary>
    /// The images tracked in the current frame.
    /// </summary>
    private List<AugmentedImage> augmentedImages = new List<AugmentedImage>();

    /// <summary>
    /// GameObject for all augmented Images. Set offset when world anchor is placed.
    /// </summary>
    public GameObject AugmentedImages;


    /// <summary>
    /// Enumerates modes the example application can be in.
    /// </summary>
    public enum ApplicationMode
    {
        Ready,
        Hosting,
        Resolving,
    }

    public ApplicationMode getApplicationMode()
    {
        return m_CurrentMode;
    }

    /// <summary>
    /// The Unity Start() method.
    /// </summary>
    public void Start()
    {
        // A Name is provided to the Game Object so it can be found by other Scripts
        // instantiated as prefabs in the scene.
        gameObject.name = "ARCoreController";
        ARCoreRoot.SetActive(false);
        _ResetStatus();
        arCoreSession = ARCoreDevice.GetComponent<ARCoreSession>();
        groundForExpedition = Instantiate(GroundPrefab);
        groundForExpedition.SetActive(false);
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {
        _UpdateApplicationLifecycle();

        // If we are neither in hosting nor resolving mode then the update is complete.
        if (m_CurrentMode != ApplicationMode.Hosting &&
            m_CurrentMode != ApplicationMode.Resolving)
        {
            return;
        }

        //Image augmentation. Get updated images for this frame.
        Session.GetTrackables<AugmentedImage>(augmentedImages, TrackableQueryFilter.Updated);

        //Create visualizers for updated augmented images that are tracking and do not previously have a visualizer. Remove visualizers for stopped images.
        foreach(AugmentedImage image in augmentedImages)
        {
            //Debug.Log("Image found, Tracking State: " + image.TrackingState);
            ARImageVisualizer visualizer = null;

            //when visualizer was added before
            visualizers.TryGetValue(image.DatabaseIndex, out visualizer);

            //if no visualizer was found, add one
            if(visualizer == null && image.TrackingState == TrackingState.Tracking)
            {
                //Debug.Log("Add visualizer");
                visualizer = (ARImageVisualizer)Instantiate(ARImageVisualizer, image.CenterPose.position, image.CenterPose.rotation, AugmentedImages.transform);
                visualizer.Image = image;
                visualizers.Add(image.DatabaseIndex, visualizer);
            }
            //if tracking has stopped and will never resume
            else if ((visualizer != null && image.TrackingState == TrackingState.Stopped))
            {
                visualizers.Remove(image.DatabaseIndex);
                Destroy(visualizer.transform.parent.gameObject);    //destroy visualizer
            }

            if(visualizer != null)
            {
                //always give the CentralImagePose to visualizer for the correct position
                visualizer.SetTransform(image.CenterPose);
            }
        }



        // If the origin anchor has not been placed yet, then update in resolving mode is
        // complete.
        if (m_CurrentMode == ApplicationMode.Resolving && !m_IsOriginPlaced)
        {
            return;
        }

        //if the anchor is already placed then update for all is complete
        if (m_IsOriginPlaced)
        {
            return;
        }

        TrackableHit arcoreHitResult = new TrackableHit();
        m_LastHitPose = null;

        //if the player has already touched the screen once, show  the expedition dimensions and where it could be placed (preview)
        if (firstTouch && !m_IsOriginPlaced && m_CurrentMode == ApplicationMode.Hosting)
        {
            // Raycast against the location the player touched to search for planes.
            if (ARCoreWorldOriginHelperClass.Raycast(touch.position.x, touch.position.y,
                    TrackableHitFlags.PlaneWithinPolygon, out arcoreHitResult))
            {
                m_LastHitPose = arcoreHitResult.Pose;
            }
        }

        //retransform the preview to where the smartphone camera points
        if (m_LastHitPose != null)
        {
            groundForExpedition.transform.position = arcoreHitResult.Pose.position;
            groundForExpedition.transform.rotation = arcoreHitResult.Pose.rotation;
            groundForExpedition.SetActive(true);
        }

        //if there was no touch, the update is complete
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        if (!firstTouch)
        {
            firstTouch = true;
            ShowMessage("Mit einem weiteren Tippen auf dem Bildschirm erstellst du die Expedition dort, wo das grüne Rechteck angezeigt wird.", 10);
            return;
        }

        m_LastHitPose = null;

        // Raycast against the location the player touched to search for planes.
        if (ARCoreWorldOriginHelperClass.Raycast(touch.position.x, touch.position.y,
                TrackableHitFlags.PlaneWithinPolygon, out arcoreHitResult))
        {
            m_LastHitPose = arcoreHitResult.Pose;
        }


        // If there was an anchor placed, then instantiate the corresponding object.
        if (m_LastHitPose != null)
        {
            // The first touch on the Hosting mode will instantiate the origin anchor.
            if (!m_IsOriginPlaced && m_CurrentMode == ApplicationMode.Hosting)
            {
                m_WorldOriginAnchor =
                    arcoreHitResult.Trackable.CreateAnchor(arcoreHitResult.Pose);

                SetWorldOrigin(m_WorldOriginAnchor.transform);
                _InstantiateAnchor();
                OnAnchorInstantiated(true);
                groundForExpedition.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Sets the apparent world origin so that the Origin of Unity's World Coordinate System
    /// coincides with the Anchor. This function needs to be called once the Cloud Anchor is
    /// either hosted or resolved.
    /// </summary>
    /// <param name="anchorTransform">Transform of the Cloud Anchor.</param>
    public void SetWorldOrigin(Transform anchorTransform)
    {
        if (m_IsOriginPlaced)
        {
            Debug.LogWarning("The World Origin can be set only once.");
            return;
        }

        m_IsOriginPlaced = true;

        ARCoreWorldOriginHelperClass.SetWorldOrigin(anchorTransform);
    }

    /// <summary>
    /// Handles user intent to enter a mode where they can place an anchor to host or to exit
    /// this mode if already in it.
    /// </summary>
    public void OnEnterHostingModeClick()
    {
        if (m_CurrentMode == ApplicationMode.Hosting)
        {
            m_CurrentMode = ApplicationMode.Ready;
            _ResetStatus();
            Debug.Log("Reset ApplicationMode from Hosting to Ready.");
        }

        ARCoreRoot.SetActive(true);
        m_CurrentMode = ApplicationMode.Hosting;
    }

    /// <summary>
    /// Handles a user intent to enter a mode where they can input an anchor to be resolved or
    /// exit this mode if already in it.
    /// </summary>
    public void OnEnterResolvingModeClick()
    {
        if (m_CurrentMode == ApplicationMode.Resolving)
        {
            m_CurrentMode = ApplicationMode.Ready;
            _ResetStatus();
            Debug.Log("Reset ApplicationMode from Resolving to Ready.");
        }

        ARCoreRoot.SetActive(true);
        m_CurrentMode = ApplicationMode.Resolving;
    }

    /// <summary>
    /// Callback indicating that the Cloud Anchor was instantiated and the host request was
    /// made.
    /// </summary>
    /// <param name="isHost">Indicates whether this player is the host.</param>
    public void OnAnchorInstantiated(bool isHost)
    {
        if (m_AnchorAlreadyInstantiated)
        {
            return;
        }

        m_AnchorAlreadyInstantiated = true;
        UIController.OnAnchorInstantiated(isHost);
    }

    /// <summary>
    /// Callback indicating that the Cloud Anchor was hosted.
    /// </summary>
    /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was hosted
    /// successfully.</param>
    /// <param name="response">The response string received.</param>
    public void OnAnchorHosted(bool success, string response)
    {
        m_AnchorFinishedHosting = success;
        UIController.OnAnchorHosted(success, response);
    }

    /// <summary>
    /// Callback indicating that the Cloud Anchor was resolved.
    /// </summary>
    /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was resolved
    /// successfully.</param>
    /// <param name="response">The response string received.</param>
    public void OnAnchorResolved(bool success, string response)
    {
        UIController.OnAnchorResolved(success, response);
    }

    /// <summary>
    /// Callback that happens when the client successfully connected to the server.
    /// </summary>
    public void OnConnectedToServer()
    {
        if (m_CurrentMode == ApplicationMode.Hosting)
        {
            UIController.ShowMessage("Finde eine geeignete Fläche und tippe auf den Bildschirm um zu sehen, wo die Expedition erstellt werden würde.", 10);
            UIController.ShowHandAnimation(5f);
        }
        else if (m_CurrentMode == ApplicationMode.Resolving)
        {
            UIController.ShowMessage("Warte darauf, dass die Expedition von der Expeditionsleitung erstellt und verbreitet wird. Versuche mit der Kamera die gleiche Umgebung wie die Expeditionsleitung zu finden.", 10);
            UIController.ShowHandAnimation(5f);
        }
        else
        {
            _QuitWithReason("Es ist ein Fehler aufgetreten. Bitte starte die Anwendung erneut.");
            Debug.LogError("Connected to server with neither Hosting nor Resolving mode. ");
        }
    }


    /// <summary>
    /// Instantiates the anchor object at the pose of the m_LastPlacedAnchor Anchor. This will
    /// host the Cloud Anchor.
    /// </summary>
    private void _InstantiateAnchor()
    {
        // The anchor will be spawned by the host, so no networking Command is needed.
        GameObject.Find("LocalPlayer").GetComponent<LocalPlayerControllerClass>()
            .SpawnAnchor(Vector3.zero, Quaternion.identity, m_WorldOriginAnchor);
    }

    /// <summary>
    /// Forward a displayable message to the UIController.
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="seconds">The duration it will be shown</param>
    public void ShowMessage(string message, int seconds)
    {
        UIController.ShowMessage(message, seconds);
    }

    /// <summary>
    /// End the expedition with success or no success.
    /// </summary>
    /// <param name="success">If the expedition was successful or not</param>
    public void EndExpedition(bool success)
    {
        UIController.EndExpedition(success);
    }

    /// <summary>
    /// Resets the internal status.
    /// </summary>
    private void _ResetStatus()
    {
        // Reset internal status.
        m_CurrentMode = ApplicationMode.Ready;
        if (m_WorldOriginAnchor != null)
        {
            Destroy(m_WorldOriginAnchor.gameObject);
        }

        m_WorldOriginAnchor = null;
    }

    /// <summary>
    /// Check and update the application lifecycle.
    /// </summary>
    private void _UpdateApplicationLifecycle()
    {
        //Screen of smartphone should never sleep when expedition is active.
        var sleepTimeout = SleepTimeout.NeverSleep;
        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            sleepTimeout = lostTrackingSleepTimeout;
        }
        Screen.sleepTimeout = sleepTimeout;

        if (m_IsQuitting)
        {
            return;
        }

        //different messages for different problems
        if (Session.Status == SessionStatus.LostTracking && Session.LostTrackingReason != LostTrackingReason.None)
        {
            switch (Session.LostTrackingReason)
            {
                case LostTrackingReason.InsufficientLight:
                    UIController.ShowMessage("Es ist zu dunkel. Bewege dein Smartphone in eine hellere Umgebung.", 7);
                    break;
                case LostTrackingReason.InsufficientFeatures:
                    UIController.ShowMessage("Es können nicht genügend Merkmale erkannt werden. Richte deine Kamera auf eine Fläche mit mehr unterschiedlicher Farbe oder anderer Textur.", 7);
                    break;
                case LostTrackingReason.ExcessiveMotion:
                    UIController.ShowMessage("Du bewegst dein Smartphone zu schnell. Versuche es ruhiger zu halten.", 7);
                    break;
                default:
                    UIController.ShowMessage("Es kann leider nichts erkannt werden.", 7);
                    break;
            }
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to
        // appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _QuitWithReason("Die App braucht die Berechtigung auf die Kamera zuzugreifen!");
        }
        else if (Session.Status.IsError())
        {
            _QuitWithReason("ARCore hat ein Verbindungsproblem erkannt. Bitte starte die App erneut.");
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void _QuitWithReason(string reason)
    {
        if (m_IsQuitting)
        {
            return;
        }
        UIController.ShowMessage(reason, 5);
        m_IsQuitting = true;
        Invoke("_DoQuit", 5.0f);
    }

    private void _DoQuit()
    {
        GameManager.LeaveApp();
    }

    /// <summary>
    /// Quit only the ARMode. 
    /// </summary>
    public void QuitARMode()
    {
        ARCoreRoot.SetActive(false);
        _ResetStatus();
        //StartCoroutine(ResetARSession());
    }

    public IEnumerator ResetARSession()
    {
        Debug.Log("Destroy the AR Session");
        arCoreSession.enabled = false;
        //ARCoreSessionConfig arCoreSessionConfig = arCoreSession.SessionConfig;
        DestroyImmediate(arCoreSession);
        Debug.Log("Is arcoreSession destroyed?: " + (ARCoreDevice.GetComponent<ARCoreSession>() == null));
        ARCoreWorldOriginHelperClass.ResetPlanes();
        yield return null;
        arCoreSession = ARCoreDevice.AddComponent<ARCoreSession>();
        arCoreSession.SessionConfig = arCoreSessionConfigPrefab;
        arCoreSession.enabled = true;
        Debug.Log("New arcore session?: " + (ARCoreDevice.GetComponent<ARCoreSession>() != null));
    }
}

