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
    /// The Anchor model that will represent the anchor in the scene.
    /// </summary>
    public GameObject AnchorPrefab;

    /// <summary>
    /// The name of the player. 
    /// </summary>
    public string playerName { get; private set; }

    /// <summary>
    /// The player object prefab which should be spawned by the server. 
    /// </summary>
    public GameObject playerObjectPrefab;

    //The prefabs to visualize on different images. Which visualized object for which image is used depends on position on imageList -> position 0: image in databse with position:0...
    public List<GameObject> VisualizedObjects;

    /// <summary>
    /// The blocks from the expedition which needs synchronization with the host. 
    /// </summary>
    private Dictionary<string, GameObject> blocksOnClientSynchronize;

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

        blocksOnClientSynchronize = new Dictionary<string, GameObject>();
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach(GameObject block in blocks)
        {
            blocksOnClientSynchronize.Add(block.gameObject.name, block);
        }
        playerName = GameObject.Find("GameManager").GetComponent<GameManager>().PlayerName;
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


    /// <summary>
    /// Command to change status of block. 
    /// </summary>
    /// <param name="netID">The network Id of the anchor to find the right child.</param>
    /// <param name="name">The name of the block.</param>
    /// <param name="currentStatus">The new status of the block which should be passed onto the clients.</param>
#pragma warning disable 618
    [Command]
    public void CmdSetBlock(NetworkInstanceId netID, string name, int currentStatus)
    {
        GameObject anchor = NetworkServer.FindLocalObject(netID);
#pragma warning restore 618
        GameObject block = anchor.transform.FindDeepChild(name).gameObject;
        block.GetComponent<Block>().CurrentStatus = currentStatus;

        RpcChangeBlockStatus(anchor, name, currentStatus);
    }

    /// <summary>
    /// The RPC to change the status of a block on every client.
    /// </summary>
    /// <param name="anchor">The anchor Object</param>
    /// <param name="name">The name of the block.</param>
    /// <param name="currentStatus">The new status of the block.</param>
#pragma warning disable 618
    [ClientRpc]
#pragma warning restore 618
    public void RpcChangeBlockStatus(GameObject anchor, string name, int currentStatus)
    {
        Transform block = anchor.transform.FindDeepChild(name);
        if (block != null)
        {
            Block foundBlock = block.gameObject.GetComponent<Block>();
            if(foundBlock.CurrentStatus != currentStatus)
            {
                foundBlock.CurrentStatus = currentStatus;
            }
        }
    }

    /// <summary>
    /// Command to change status (percent analyzed) of block. 
    /// </summary>
    /// <param name="netID">The network Id of the anchor to find the right child.</param>
    /// <param name="name">The name of the block.</param>
    /// <param name="newStatus">The new status (percent analyzed) of the block which should be passed onto the clients.</param>
#pragma warning disable 618
    [Command]
    public void CmdSetBlockPercentAnalyzed(NetworkInstanceId netID, string name, int newStatus)
    {
        GameObject anchor = NetworkServer.FindLocalObject(netID);
#pragma warning restore 618
        GameObject block = anchor.transform.FindDeepChild(name).gameObject;
        block.GetComponent<Block>().PercentAnalyzed = newStatus;
        RpcChangeBlockPercentAnalyzed(anchor, name, newStatus);
    }

    /// <summary>
    /// The RPC to change the status (percent analyzed) of a block on every client.
    /// </summary>
    /// <param name="anchor">The anchor Object</param>
    /// <param name="name">The name of the block.</param>
    /// <param name="percentAnalyzed">The new status (percent analyzed) of the block.</param>
#pragma warning disable 618
    [ClientRpc]
#pragma warning restore 618
    public void RpcChangeBlockPercentAnalyzed(GameObject anchor, string name, int percentAnalyzed)
    {
        Transform block = anchor.transform.FindDeepChild(name);
        if (block != null)
        {
            Block foundBlock = block.gameObject.GetComponent<Block>();
            if(foundBlock.PercentAnalyzed != percentAnalyzed)
            {
                foundBlock.PercentAnalyzed = percentAnalyzed;
            }
        }
    }

    /// <summary>
    /// Command to change status (exploration status) of block. 
    /// </summary>
    /// <param name="netID">The network Id of the anchor to find the right child.</param>
    /// <param name="name">The name of the block.</param>
    /// <param name="newStatus">The new status (exploration status) of the block which should be passed onto the clients.</param>
#pragma warning disable 618
    [Command]
    public void CmdSetBlockExplorationStatus(NetworkInstanceId netID, string name, int newStatus)
    {
        GameObject anchor = NetworkServer.FindLocalObject(netID);
#pragma warning restore 618
        GameObject block = anchor.transform.FindDeepChild(name).gameObject;
        block.GetComponent<Block>().ExplorationStatus = newStatus;
        RpcChangeBlockExplorationStatus(anchor, name, newStatus);
    }

    /// <summary>
    /// The RPC to change the status (exploration status) of a block on every client.
    /// </summary>
    /// <param name="anchor">The anchor Object</param>
    /// <param name="name">The name of the block.</param>
    /// <param name="explorationStatus">The new status (exploration status) of the block.</param>
#pragma warning disable 618
    [ClientRpc]
#pragma warning restore 618
    public void RpcChangeBlockExplorationStatus(GameObject anchor, string name, int explorationStatus)
    {
        Transform block = anchor.transform.FindDeepChild(name);
        if (block != null)
        {
            Block foundBlock = block.gameObject.GetComponent<Block>();
            if (foundBlock.ExplorationStatus != explorationStatus)
            {
                foundBlock.ExplorationStatus = explorationStatus;
            }
        }
    }

    /// <summary>
    /// Command to update all blocks. If all blocks are updated, call TargetRPC to the client.
    /// </summary>
    /// <param name="netID">The net ID of the cloud anchor</param>
#pragma warning disable 618
    [Command]
    public void CmdUpdateAllBlocks(NetworkInstanceId netID)
    {
        GameObject anchor = NetworkServer.FindLocalObject(netID);
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
#pragma warning restore 618
        //Debug.Log("SERVER: number of blocks: " + blocks.Length);

        //Send update of each block to the client
        foreach (GameObject blockGO in blocks)
        {
            Block block = blockGO.GetComponent<Block>();
            TargetUpdateAllBlocks(connectionToClient, anchor, blockGO.name, block.CurrentStatus, block.PercentAnalyzed, block.ExplorationStatus);
            //Debug.Log("SERVER: Update " + blockGO.name + ": current status: " + block.CurrentStatus + ", percent analyzed: " + block.PercentAnalyzed + ", exploration status: " + block.ExplorationStatus);
        }

        TargetUpdateAllBlocksEnd(connectionToClient);
    }

    /// <summary>
    /// RPC to specific client with update of one block. 
    /// </summary>
    /// <param name="client">The client</param>
    /// <param name="anchor">The cloud anchor</param>
    /// <param name="blockName">The name of the block as identifier</param>
    /// <param name="currentStatus">The updated current status</param>
    /// <param name="percentAnalyzed">The updated percent analyzed</param>
    /// <param name="explorationStatus">The updated exploration status</param>
#pragma warning disable 618
    [TargetRpc]
    public void TargetUpdateAllBlocks(NetworkConnection client, GameObject anchor, string blockName, int currentStatus, int percentAnalyzed, int explorationStatus)
    {
#pragma warning restore 618
        //Debug.Log("CLIENT: Update Block! " + blockName);
        Transform block = anchor.transform.FindDeepChild(blockName);
        if (block != null)
        {
            blocksOnClientSynchronize.Remove(block.gameObject.name);
            //Debug.Log("CLIENT: BEFORE UPDATE " + blockName + ": current status: " + currentStatus + ", percent analyzed: " + percentAnalyzed + ", exploration status: " + explorationStatus);
            block.gameObject.GetComponent<Block>().CurrentStatus = currentStatus;
            block.gameObject.GetComponent<Block>().PercentAnalyzed = percentAnalyzed;
            block.gameObject.GetComponent<Block>().ExplorationStatus = explorationStatus;
            //Debug.Log("CLIENT: AFTER UPDATE " + blockName + ": current status: " + currentStatus + ", percent analyzed: " + percentAnalyzed + ", exploration status: " + explorationStatus);
        }
    }

    /// <summary>
    /// Function which gets called on the client when the synchronization of the blocks is finished. Checks with own list of all blocks. If there are any blocks which are already destroyed on the server but not on the client, send another command to server.
    /// </summary>
    /// <param name="client">The client</param>
#pragma warning disable 618
    [TargetRpc]
    public void TargetUpdateAllBlocksEnd(NetworkConnection client)
#pragma warning restore 618
    {
        //check all still existing blocks on the client which don't exist on the server
        foreach (KeyValuePair<string, GameObject> block in blocksOnClientSynchronize)
        {
            CmdCheckBlockForBones(block.Value.transform.parent.name, block.Value.name);
        }

        blocksOnClientSynchronize.Clear();
    }

    /// <summary>
    /// Server checks the blocks for bones: 
    /// When the parent gameObject does not exist, it should be destroyed on the client as well.
    /// When the parent gameObject exists, only the block should be destroyed on the client.
    /// </summary>
    /// <param name="parentBlockName">The name of the parent gameObject</param>
    /// <param name="childBlockName">The name of the block</param>
#pragma warning disable 618
    [Command]
#pragma warning restore 618
    public void CmdCheckBlockForBones(string parentBlockName, string childBlockName)
    {
        GameObject parentBlock = GameObject.Find(parentBlockName);
        if(parentBlock == null)
        {
            TargetDestroySpecificBlock(connectionToClient, childBlockName, true);
        }
        else
        {
            TargetDestroySpecificBlock(connectionToClient, childBlockName, false);
        }
    }

    /// <summary>
    /// Specific client destroys a specific gameObject (either a parent of a block or the block itself). 
    /// </summary>
    /// <param name="client">The client</param>
    /// <param name="blockNameToDestroy">The gameObject to destroy</param>
#pragma warning disable 618
    [TargetRpc]
    public void TargetDestroySpecificBlock(NetworkConnection client, string blockNameToDestroy, bool bonesDestroyed)
#pragma warning restore 618
    {
        GameObject blockToDestroy = GameObject.Find(blockNameToDestroy);
        if(blockToDestroy != null)
        {
            if (bonesDestroyed)
            {
                blockToDestroy.GetComponent<Block>().DestroyBlock(false);
            }
            else
            {
                blockToDestroy.GetComponent<Block>().DestroyBlock(true);
            }
        }
    }
}

