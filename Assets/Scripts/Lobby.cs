using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Lobby
{
    public long Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }
    public bool IsGaming { get; set; }

    readonly List<LobbyPlayer> players;

    public Lobby(string name, int capacity)
    {
        Id = UniNumGenetator.NewId();
        Name = name;
        Capacity = capacity;
        IsGaming = false;
        players = new List<LobbyPlayer>();
    }

    public void Join(LobbyPlayer player)
    {
        players.Add(player);
    }

    public void Exit(long playerId)
    {
        var player = GetLobbyPlayer(playerId);
        players.Remove(player);
    }

    public void Ready(long playerId)
    {
        var player = GetLobbyPlayer(playerId);
        player.isReady = true;
    }

    public void CancelReady(long playerId)
    {
        var player = GetLobbyPlayer(playerId);
        player.isReady = false;
    }

    public bool Start()
    {
        foreach(var player in players)
        {
            if (!player.isReady)
            {
                return false;
            }
        }
        return true;
    }

    private LobbyPlayer GetLobbyPlayer(long playerId)
    {
        return players.Single((player) =>
        {
            return player.id == playerId;
        });
    }

    public List<LobbyPlayer> GetLobbyPlayers()
    {
        return players;
    }
}

