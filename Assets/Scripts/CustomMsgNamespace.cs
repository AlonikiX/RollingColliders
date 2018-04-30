using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace CustomNetworkMessage
{
    public enum CustomMsg
    {
        RequestLobbies = MsgType.Highest + 1,
        CreateLobby,
        JoinLobby,
        ExitLobby,
        Ready,
        CancelReady,
        Start
    }

    public class RequestLobbiesMsg : MessageBase
    {
        public int page;
    }

    public class CreateLobbyMsg : MessageBase
    {
        public string name;
        public int capacity;
        public LobbyPlayer owner;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(name);
            writer.Write(capacity);
            owner.Serialize(writer);
        }

        public override void Deserialize(NetworkReader reader)
        {
            name = reader.ReadString();
            capacity = reader.ReadInt32();
            owner = LobbyPlayer.Deserialize(reader);
        }
    }

    public class LobbySpecificMsg: MessageBase
    {
        public long lobbyId;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(lobbyId);
            base.Serialize(writer);
        }

        public override void Deserialize(NetworkReader reader)
        {
            lobbyId = reader.ReadInt64();
            base.Deserialize(reader);
        }
    }

    public class JoinLobbyMsg : LobbySpecificMsg
    {
        public LobbyPlayer player;

        public override void Serialize(NetworkWriter writer)
        {
            player.Serialize(writer);
        }

        public override void Deserialize(NetworkReader reader)
        {
            player = LobbyPlayer.Deserialize(reader);
        }
    }

    public class ExitLobbyMsg : LobbySpecificMsg
    {
        public long playerId;
    }

    public class ReadyMsg : LobbySpecificMsg
    {
        public long playerId;
    }

    public class CancelReadyMsg : LobbySpecificMsg
    {
        public long playerId;
    }

    public class StartMsg : LobbySpecificMsg
    {
        
    }

}
