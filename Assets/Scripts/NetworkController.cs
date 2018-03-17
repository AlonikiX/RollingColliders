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

    }

    private void Update()
    {
        //Debug.Log("start");
        if (Input.GetKeyDown(KeyCode.O)) {
            //this.StartHost();
            Debug.Log("start");
        }
    }



}
