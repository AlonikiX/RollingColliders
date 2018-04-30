using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using CustomNetworkMessage;
using System;

public class LobbyPlayer
{
    public long id;
    public bool isOwner;
    public bool isReady;
    public string name;
    public string avatar;

    public void Serialize(NetworkWriter writer)
    {
        writer.Write(id);
        writer.Write(isOwner);
        writer.Write(isReady);
        writer.Write(name);
        writer.Write(avatar);
    }

    public static LobbyPlayer Deserialize(NetworkReader reader)
    {
        var player = new LobbyPlayer();
        player.id = reader.ReadInt64();
        player.isOwner = reader.ReadBoolean();
        player.isReady = reader.ReadBoolean();
        player.name = reader.ReadString();
        player.avatar = reader.ReadString();

        return player;
    }

}
