using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using CustomNetworkMessage;
using System.Linq;

public class LobbyServer : MonoBehaviour
{
    readonly int LobbiesPerRequest = 8;

    List<Lobby> lobbies;

    // Use this for initialization
    void Start()
    {
        lobbies = new List<Lobby>();

        NetworkServer.Listen(Constants.Port);
        NetworkServer.RegisterHandler((short)CustomMsg.RequestLobbies, OnRequestLobbies);
        NetworkServer.RegisterHandler((short)CustomMsg.CreateLobby, OnRequestCreateLobby);
        NetworkServer.RegisterHandler((short)CustomMsg.JoinLobby, (netMsg) => 
        {
            var msg = netMsg.ReadMessage<JoinLobbyMsg>();
            GetDestinedLobby(msg.lobbyId).Join(msg.player);
        });
        NetworkServer.RegisterHandler((short)CustomMsg.ExitLobby, (netMsg) => 
        {
            var msg = netMsg.ReadMessage<ExitLobbyMsg>();
        });
        NetworkServer.RegisterHandler((short)CustomMsg.Ready, (netMsg) => 
        { });
        NetworkServer.RegisterHandler((short)CustomMsg.CancelReady, (netMsg) => 
        { });
        NetworkServer.RegisterHandler((short)CustomMsg.Start, (netMsg) => 
        { });
    }

    Lobby GetDestinedLobby(long lobbyId)
    {
        return lobbies.Single((lobby) =>
        {
            return lobby.Id == lobbyId;
        });
    }

    void OnRequestLobbies(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<RequestLobbiesMsg>();
        var start = (msg.page - 1) * LobbiesPerRequest;
        if (start >= lobbies.Count)
        {
            return;
        }
        var remains = lobbies.Count - start;
        var count = remains >= LobbiesPerRequest ? LobbiesPerRequest : remains;
        var range = lobbies.GetRange(start, count);
    }

    void OnRequestCreateLobby(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<CreateLobbyMsg>();
        var lobby = new Lobby(msg.name, msg.capacity);
        lobbies.Add(lobby);
        lobby.Join(msg.owner);
    }

}

