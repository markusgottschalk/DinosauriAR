//-----------------------------------------------------------------------
// <copyright file="LocalPlayerController.cs" company="Google">
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
//THIS SCRIPT WAS MODIFIED.
//-----------------------------------------------------------------------


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Local player controller. Handles the spawning of the networked Game Objects.
/// </summary>
#pragma warning disable 618
public class LocalPlayerControllerClass : NetworkBehaviour
#pragma warning restore 618
{
    /// <summary>
    /// The Star model that will represent networked objects in the scene.
    /// </summary>
    public GameObject StarPrefab;

    /// <summary>
    /// The Anchor model that will represent the anchor in the scene.
    /// </summary>
    public GameObject AnchorPrefab;

    public string playerName; //TODO: get Playername from GameManager

    /// <summary>
    /// The player object prefab which should be spawned by the server. 
    /// </summary>
    public GameObject playerObjectPrefab;

    //The prefabs to visualize on different images. Which visualized object for which image is used depends on position on imageList -> position 0: image in databse with position:0...
    public List<GameObject> VisualizedObjects;

    /// <summary>
    /// The Unity OnStartLocalPlayer() method. This works only for the local client.
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // A Name is provided to the Game Object so it can be found by other Scripts, since this
        // is instantiated as a prefab in the scene.
        gameObject.name = "LocalPlayer";
#pragma warning disable 618
        CmdSpawnPlayerObject();
#pragma warning restore 618
    }

    /// <summary>
    /// Will spawn the origin anchor and host the Cloud Anchor. Must be called by the host.
    /// </summary>
    /// <param name="position">Position of the object to be instantiated.</param>
    /// <param name="rotation">Rotation of the object to be instantiated.</param>
    /// <param name="anchor">The ARCore Anchor to be hosted.</param>
    public void SpawnAnchor(Vector3 position, Quaternion rotation, Component anchor)
    {
        // Instantiate Anchor model at the hit pose.
        var anchorObject = Instantiate(AnchorPrefab, position, rotation);

        // Anchor must be hosted in the device.
        anchorObject.GetComponent<CloudAnchorController>().HostLastPlacedAnchor(anchor);

        // Host can spawn directly without using a Command because the server is running in this
        // instance.
#pragma warning disable 618
        NetworkServer.Spawn(anchorObject);
#pragma warning restore 618

        //disable graphics of expedition/cloud anchor on all clients (but not the host) 
#pragma warning disable 618
        foreach (NetworkConnection client in NetworkServer.connections)
#pragma warning restore 618
        {
            if (client != connectionToClient)
            {
                TargetSetCloudAnchorGraphicState(client, anchorObject, false);
            }
        }
        
    }

    /// <summary>
    /// A command run on the server that will spawn the Star prefab in all clients.
    /// </summary>
    /// <param name="position">Position of the object to be instantiated.</param>
    /// <param name="rotation">Rotation of the object to be instantiated.</param>
#pragma warning disable 618
    [Command]
#pragma warning restore 618
    public void CmdSpawnStar(Vector3 position, Quaternion rotation)
    {
        // Instantiate Star model at the hit pose.
        var starObject = Instantiate(StarPrefab, position, rotation);

        // Spawn the object in all clients.
#pragma warning disable 618
        NetworkServer.Spawn(starObject);
#pragma warning restore 618
    }

    /// <summary>
    /// Instantiates the player object on the server and spawns it on all clients. Only the client who spawned it has the authority.
    /// </summary>
#pragma warning disable 618
    [Command]
    public void CmdSpawnPlayerObject()
#pragma warning restore 618
    {
        GameObject playerObject = Instantiate(playerObjectPrefab);
#pragma warning disable 618
        NetworkServer.SpawnWithClientAuthority(playerObject, connectionToClient);
#pragma warning restore 618
    }

    /// <summary>
    /// Instantiates the right tool object on the server and spawns it on all clients. Only the client who spawned it has the authority.
    /// </summary>
    /// <param name="imageDatabaseindex">The corresponding tool for the image</param>
#pragma warning disable 618
    [Command]
#pragma warning restore 618
    public void CmdSpawnTool(int imageDatabaseindex)
    {
        Debug.Log("Server: Request to spawn tool for visualizer");
        GameObject tool = Instantiate(VisualizedObjects[imageDatabaseindex]);
        tool.GetComponent<Tool>().imageDatabaseindex = imageDatabaseindex;
#pragma warning disable 618
        NetworkServer.SpawnWithClientAuthority(tool, connectionToClient);
#pragma warning restore 618
    }


    /// <summary>
    /// Set a gameobject active/inactive.
    /// </summary>
    /// <param name="networkID">The Network id of the gameobject</param>
    /// <param name="active">The state the gameobject should have</param>
#pragma warning disable 618
    [Command]
    public void CmdSetGameObjectState(NetworkInstanceId networkID, bool active)
#pragma warning restore 618
    {
#pragma warning disable 618
        GameObject gameobject = NetworkServer.FindLocalObject(networkID); //Find the local gameobject
#pragma warning restore 618
        Debug.Log("Server: Tool object network id: " + networkID + ", set to " + active);
        gameobject.SetActive(active);                                     //set it to active/inactive
        RpcSetGameObjectState(gameobject, active);                         //propagate it to all clients
    }

    /// <summary>
    /// Propagates the state of a gameobject to all clients
    /// </summary>
    /// <param name="gameobject">The gameobject to enable/disable</param>
    /// <param name="active">The state the gameobject should have</param>
#pragma warning disable 618
    [ClientRpc]
    public void RpcSetGameObjectState(GameObject gameobject, bool active)
#pragma warning restore 618
    {
        gameobject.SetActive(active);                                         //each Client finds the local gameobject with the given network id and sets it active/inactive
        
    }

    /// <summary>
    /// Enables/disables the graphics of the expedition/cloud anchor on a specific client
    /// </summary>
    /// <param name="client">The target client</param>
    /// <param name="anchor">The expedition/cloud anchor object</param>
    /// <param name="active">The state the expedition/cloud anchor should have</param>
#pragma warning disable 618
    [TargetRpc]
    public void TargetSetCloudAnchorGraphicState(NetworkConnection client, GameObject anchor, bool active)
#pragma warning restore 618
    {
        foreach (Transform child in anchor.transform)
        {
            child.gameObject.SetActive(active);
        }
    }
}

