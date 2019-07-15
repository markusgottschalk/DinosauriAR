using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 618
public class PlayerObject : NetworkBehaviour
#pragma warning restore 618
{

    private GameObject firstPersonCamera = null;

    /// <summary>
    /// Attach this player object to the first person camera, ONLY on own client/when you have authority!
    /// The transform component will be propagated through the NetworkTransform component.
    /// </summary>
    public override void OnStartAuthority()
    {
        gameObject.name = "PlayerObject";
        firstPersonCamera = GameObject.Find("First Person Camera");
        transform.parent = firstPersonCamera.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
