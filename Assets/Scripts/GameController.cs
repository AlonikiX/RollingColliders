using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour
{

    private NetworkController networkController;

    public Dictionary<NetworkInstanceId, GameObject> playerPool;
    // Use this for initialization
    void Start()
    {
        networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        if (isServer)
            networkController.ServerAddPlayer += AddNewPlayer;

        playerPool = new Dictionary<NetworkInstanceId, GameObject>();
        
    }

    private void AddNewPlayer(object sender, ServerAddPlayerEventArgs e)
    {
        if (!isServer) {
            return;
        }
        var player = e.Player;
        var nid = player.GetComponent<NetworkIdentity>().netId;
        playerPool.Add(nid, player);
        Debug.Log("GameController: new player");

        RpcNewPlayer(nid);
    }

    // Update is called once per frame
    void Update()
    {
    }


    [ClientRpc]
    public void RpcNewPlayer(NetworkInstanceId nid)
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            if (nid == player.GetComponent<NetworkIdentity>().netId
                && !playerPool.ContainsKey(nid)) {
                playerPool.Add(nid, player);
            }
        }

        Debug.Log("Client: new player!");
    }

}
