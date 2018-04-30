using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using CustomNetworkMessage;
using System;
using UnityEngine.SceneManagement;

public class GameRoomServer : MonoBehaviour
{
    private Lobby lobby;
	// Use this for initialization
	void Start()
	{
        NetworkServer.Listen(6969);
        NetworkServer.RegisterHandler((short)CustomMsg.CreateLobby, OnCreateLobby);
        NetworkServer.RegisterHandler((short)CustomMsg.ExitLobby, OnExitLobby);
        NetworkServer.RegisterHandler((short)CustomMsg.JoinLobby, OnJoinLobby);
        NetworkServer.RegisterHandler((short)CustomMsg.Ready, OnReady);
        NetworkServer.RegisterHandler((short)CustomMsg.CancelReady, OnCancelReady);
        NetworkServer.RegisterHandler((short)CustomMsg.Start, OnStart);
	}

    void OnCreateLobby(NetworkMessage netMsg)
    {
        if (lobby != null)
        {
            return;
        }
        var msg = netMsg.ReadMessage<CreateLobbyMsg>();
        lobby = new Lobby(msg.name, msg.capacity);
    }

    void OnExitLobby(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<ExitLobbyMsg>();
        lobby.Exit(msg.playerId);
    }

    void OnJoinLobby(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<JoinLobbyMsg>();
        lobby.Join(msg.player);
    }

    void OnReady(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<ReadyMsg>();
        lobby.Ready(msg.playerId);
    }

    void OnCancelReady(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<CancelReadyMsg>();
        lobby.CancelReady(msg.playerId);
    }

    void OnStart(NetworkMessage netMsg)
    {
        if (lobby.Start())
        {
            NetworkServer.SendToAll((short)CustomMsg.Start, netMsg.ReadMessage<StartMsg>());
        }
        //Load Game Scene
        lobby.IsGaming = true;
    }







}
