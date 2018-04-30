using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using CustomNetworkMessage;
using UnityEngine.UI;

public class LobbyClient : MonoBehaviour
{
    public Slider playerCountSlider;
    public InputField lobbyNameInput;

    NetworkClient networkClient;

    private void Start()
    {
        networkClient = new NetworkClient();
        networkClient.RegisterHandler(MsgType.Connect, Onconnected);
        networkClient.Connect(Constants.ServerAddress, Constants.Port);
    }

    public void CreateLobby()
    {
        var msg = new CreateLobbyMsg();
        msg.name = lobbyNameInput.text;
        msg.capacity = (int)playerCountSlider.value;
        msg.owner = new LobbyPlayer();
        networkClient.Send((short)CustomMsg.CreateLobby, msg);
        Debug.Log("Client: Createing lobby...");
    }

    void Onconnected(NetworkMessage netMsg)
    {
        Debug.Log("Client: Connected to the server.");
    }
}
