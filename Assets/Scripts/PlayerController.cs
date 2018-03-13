using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class PlayerController : NetworkBehaviour {

    public float speed;
    public float speedLimit;
    private Rigidbody rb;
    int mana = 100;
    bool canSkill;
    float oldTime;
    float currentTime;

    float ort;
    float crt;

    bool isSet = false;

    private Vector3 velocity;
    [SyncVar]
    public string playerName;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        //rb.velocity = new Vector3(3, 0, -3);
        oldTime = 0;
        currentTime = 0;

        ort = 0;
        crt = 0;
        //if (!isSet) {
        //    playerName = "Server";
        //    gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        //    isSet = true;
        //}else {
        //    playerName = "Client";
        //}
	}

    // Update is called once per frame
    void FixedUpdate () {
        if (!isLocalPlayer) {
            return;
        }

        velocity = rb.velocity;

        currentTime += Time.deltaTime;
        crt += Time.deltaTime;
        if (currentTime - oldTime < 0.0667) {
            return;
        }else {
            oldTime = currentTime;
        }
        Debug.Log(rb.velocity);
        var moveX = Input.GetAxisRaw("Horizontal");
        var moveZ = Input.GetAxisRaw("Vertical");

        if ((!moveX.Equals(0.0f)) || (!moveZ.Equals(0.0f))) {
            velocity = new Vector3(moveX * speedLimit, 0, moveZ * speedLimit);
            //rb.AddForce(new Vector3(moveX * speedLimit, 0, moveZ * speedLimit));
            if (velocity.magnitude >= speed) {
                velocity = velocity.normalized * speed;
            }
            Debug.Log(playerName);
            CmdMove(velocity, gameObject.GetComponent<NetworkIdentity>().netId.ToString());
          
            //if (isServer) {
            //    rb.velocity = 
            //}

        }



        if (mana < 100) {
            mana += 1;
        }

        if (mana == 100) {

            if (Input.GetKey(KeyCode.Space)) {
                rb.velocity = new Vector3(rb.velocity.x, 10, rb.velocity.z);
                mana -= 100;
            }

            if (Input.GetKey(KeyCode.J)) {
                rb.velocity = new Vector3(rb.velocity.x * 3, rb.velocity.y, rb.velocity.z * 3);
                mana -= 100;
            }

        }


        if (isServer) {
            if (crt - ort >= Time.deltaTime * 30) {
                ort = crt;


                var clients = GameObject.FindGameObjectsWithTag("Player");
                foreach (var client in clients) {
                    var clientRb = client.GetComponent<Rigidbody>();

                    RpcSyncObject(client.GetComponent<NetworkIdentity>().netId.ToString(), clientRb.position, clientRb.velocity);
                }
            }
        }
	}

    [ClientRpc]
    public void RpcSyncObject(string id, Vector3 pos, Vector3 vel) {
        var clients = GameObject.FindGameObjectsWithTag("Player");

        foreach (var client in clients) {
            if (client.GetComponent<NetworkIdentity>().netId.ToString().Equals(id)) {
                //if (!client.GetComponent<NetworkIdentity>().isLocalPlayer) {
                var clientRb = client.GetComponent<Rigidbody>();
                clientRb.position = pos;
                clientRb.velocity = vel;
                //}
            }
        }

    }


    [Command]
    public void CmdMove(Vector3 velocity, string id) {
        var clients = GameObject.FindGameObjectsWithTag("Player");
        foreach (var client in clients) {
            if (client.GetComponent<NetworkIdentity>().netId.ToString().Equals(id)) {
                //if (!client.GetComponent<NetworkIdentity>().isLocalPlayer) {
                var clientRb = client.GetComponent<Rigidbody>();
                clientRb.velocity = velocity;
                RpcMove(velocity, client.GetComponent<NetworkIdentity>().netId.ToString());
                //}
            }

        }
        //RpcMove(velocity)
    }

    [ClientRpc]
    public void RpcMove(Vector3 velocity, string id) {
        var clients = GameObject.FindGameObjectsWithTag("Player");
        foreach (var client in clients) {
            if (client.GetComponent<NetworkIdentity>().netId.ToString().Equals(id)) {
                var clientRb = client.GetComponent<Rigidbody>();
                clientRb.velocity = velocity;
            }

        }
    }

    public override void OnStartClient() {
        if (isLocalPlayer) {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        }
    }


}
