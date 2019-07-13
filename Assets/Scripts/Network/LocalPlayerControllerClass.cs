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
        Debug.LogError("Server: Request to spawn tool for visualizer");
        GameObject tool = Instantiate(VisualizedObjects[imageDatabaseindex]);
        tool.GetComponent<Tool>().imageDatabaseindex = imageDatabaseindex;
#pragma warning disable 618
        NetworkServer.SpawnWithClientAuthority(tool, connectionToClient);
#pragma warning restore 618
    }


    /// <summary>
    /// Set a tool active/inactive.
    /// </summary>
    /// <param name="active">The state the tool should have</param>
    /// <param name="networkID">The Network id of the tool</param>
#pragma warning disable 618
    [Command]
    public void CmdIsToolActive(bool active, NetworkInstanceId networkID)
#pragma warning restore 618
    {
#pragma warning disable 618
        GameObject tool = NetworkServer.FindLocalObject(networkID); //Find the local tool object
#pragma warning restore 618
        Debug.Log("Server: tool object network id: " + networkID + ", is tool == null?: " + (tool == null));
        tool.SetActive(active);                                     //set it to active/inactive
        RpcIsToolActive(active, tool);                         //propagate it to all clients
    }

    /// <summary>
    /// Propagates the state of a tool to all clients
    /// </summary>
    /// <param name="active">The state the tool should have</param>
    /// <param name="networkID">The Network id of the tool</param>
#pragma warning disable 618
    [ClientRpc]
    public void RpcIsToolActive(bool active, /*NetworkInstanceId networkID*/GameObject tool)
#pragma warning restore 618
    {
#pragma warning disable 618
        //GameObject tool = ClientScene.FindLocalObject(networkID);     //each Client finds the local tool with the given network id...
        //Debug.Log("Clients: tool object network id: " + networkID + ", is tool == null?: " + (tool == null));
#pragma warning restore 618
        tool.SetActive(active);                                         //...and sets it active/inactive
    }
}

