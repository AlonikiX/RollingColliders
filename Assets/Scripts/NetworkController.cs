using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkController : NetworkManager
{

    bool isRoleSet;

    public event EventHandler<ServerAddPlayerEventArgs> ServerAddPlayer;

    // Use this for initialization
    void Start()
    {
        isRoleSet = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRoleSet)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                StartHost();
                isRoleSet = true;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                StartServer();
                isRoleSet = true;
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                StartClient();
                isRoleSet = true;
            }
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        ServerAddPlayer?.Invoke(this, new ServerAddPlayerEventArgs(player));
        Debug.Log("NetworkManager: new player");
    }

}

public class ServerAddPlayerEventArgs : EventArgs
{
    public GameObject Player;
    public ServerAddPlayerEventArgs(GameObject player)
    {
        this.Player = player;
    }
}