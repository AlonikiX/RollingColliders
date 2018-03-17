using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class PlayerController : NetworkBehaviour {

    //normal moving speed of the player
    public float speed;
    //player's rigidbody component
    private Rigidbody rb;
    //mana, using every skill will consume mana
    int mana = 100;
    bool canSkill;
    float oldTime;
    float currentTime;

    float ort;
    float crt;

    private Vector3 velocity;
    [SyncVar]
    public string playerName;

    private Dictionary<NetworkInstanceId, GameObject> players = new Dictionary<NetworkInstanceId, GameObject>();

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        //rb.velocity = new Vector3(3, 0, -3);
        oldTime = 0;
        currentTime = 0;

        ort = 0;
        crt = 0;

        var playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (var playerGameObject in playerGameObjects) {
            var networkIdentity = playerGameObject.GetComponent<NetworkIdentity>();
            players.Add(networkIdentity.netId, playerGameObject);
        }
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
            velocity = new Vector3(moveX * speed, 0, moveZ * speed);
            CmdMove(velocity, gameObject.GetComponent<NetworkIdentity>().netId);
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
            if (crt - ort >= Time.deltaTime * 10) {
                ort = crt;

                foreach (var player in players) {
                    var playerRigidbody = player.Value.GetComponent<Rigidbody>();

                    RpcSyncObject(player.Value.GetComponent<NetworkIdentity>().netId, playerRigidbody.position, playerRigidbody.velocity);
                }
            }
        }
	}

    [ClientRpc]
    public void RpcSyncObject(NetworkInstanceId id, Vector3 pos, Vector3 vel) {

        foreach (var player in players) {
            if (player.Key == id) {
                var playerRigidbody = player.Value.GetComponent<Rigidbody>();
                playerRigidbody.position = pos;
                playerRigidbody.velocity = vel;
            }
        }

    }


    [Command]
    public void CmdMove(Vector3 velocity, NetworkInstanceId id) {
        foreach (var player in players) {
            if (player.Key == id) {
                var playerRigidbody = player.Value.GetComponent<Rigidbody>();
                playerRigidbody.velocity = velocity;
                RpcMove(velocity, player.Value.GetComponent<NetworkIdentity>().netId);
            }

        }
    }

    [ClientRpc]
    public void RpcMove(Vector3 velocity, NetworkInstanceId id) {
        foreach (var player in players) {
            if (player.Key == id) {
                var playerRigidbody = player.Value.GetComponent<Rigidbody>();
                playerRigidbody.velocity = velocity;
            }

        }
    }

    public override void OnStartClient() {
        if (isLocalPlayer) {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        }
    }


}
