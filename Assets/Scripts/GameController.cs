using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour
{

    private NetworkController networkController;

    private Dictionary<NetworkInstanceId, GameObject> playerPool;
    // Use this for initialization
    void Start()
    {
        networkController = GetComponent<NetworkController>();
        networkController.ServerAddPlayer += AddNewPlayer;

        playerPool = new Dictionary<NetworkInstanceId, GameObject>();
    }

    private void AddNewPlayer(object sender, ServerAddPlayerEventArgs e)
    {
        var player = e.Player;
        var networkId = player.GetComponent<NetworkIdentity>().netId;
        playerPool.Add(networkId, player);
        Debug.Log("GameController: new player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
