using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.Networking;


public class NetworkController : MonoBehaviour {

    const bool SERVER = false;

    NetworkManager networkManager;

    // Use this for initialization
    void Start() {

        if (SERVER) {

            networkManager = (NetworkManager)GetComponent<NetworkManager>();
            Thread.Sleep(1000);
            networkManager.StartHost();
        }
    }

}
