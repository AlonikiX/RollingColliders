using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class PlayerController : PlayerBehaviour
{

    private NetworkController networkController;

    //normal moving speed of the player
    public float speed;
    //player's rigidbody component
    private Rigidbody rb;
    //mana, using every skill will consume mana
    int mana = 100;
    float oldTime;
    float currentTime;

    float ort;
    float crt;

    private Vector3 curretnVelocity;

    private Dictionary<NetworkInstanceId, GameObject> playerPool = new Dictionary<NetworkInstanceId, GameObject>();
    public Dictionary<NetworkInstanceId, GameObject> PlayerPool {
        get {
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players) {
                var nid = player.GetComponent<NetworkIdentity>().netId;
                if (!playerPool.ContainsKey(nid)) {
                    playerPool.Add(nid, player);
                }
            }

            return playerPool;
        }
    }
    private Dictionary<NetworkInstanceId, Queue<Vector3>> positionBufferPool = new Dictionary<NetworkInstanceId, Queue<Vector3>>();
    private Dictionary<NetworkInstanceId, Queue<Vector3>> velocityBufferPool = new Dictionary<NetworkInstanceId, Queue<Vector3>>();

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.velocity = new Vector3(3, 0, -3);
        oldTime = 0;
        currentTime = 0;

        ort = 0;
        crt = 0;

    }

    [ClientRpc]
    void RpcNewPlayer(NetworkInstanceId id) {
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players) {
            var networkIdentity = player.GetComponent<NetworkIdentity>();
            if (networkIdentity.netId == id) {
                PlayerPool.Add(id, player);
            }
        }
        Debug.Log("Client: new player: " + id);
        Debug.Log("Client: player count: " + PlayerPool.Count);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //foreach (var player in Players)
        //{
            //var playerRigidbody = player.Value.GetComponent<Rigidbody>();
            //if (positionBufferPool.ContainsKey(player.Key))
            //{
            //    var positionBuffer = positionBufferPool[player.Key];
            //    if (positionBuffer.Count > 0)
            //    {
            //        playerRigidbody.position = positionBuffer.Dequeue();
            //    }
            //}
            //if (velocityBufferPool.ContainsKey(player.Key))
            //{
            //    var velocityBuffer = velocityBufferPool[player.Key];
            //    if (velocityBuffer.Count > 0)
            //    {
            //        playerRigidbody.velocity = velocityBuffer.Dequeue();
            //    }
            //}
        //}

        currentTime += Time.deltaTime;
        crt += Time.deltaTime;
        if (currentTime - oldTime < 0.0667)
        {
            return;
        }
        else
        {
            oldTime = currentTime;
        }
        //Debug.Log(rb.velocity);
        var moveX = Input.GetAxisRaw("Horizontal");
        var moveZ = Input.GetAxisRaw("Vertical");

        if ((!moveX.Equals(0.0f)) || (!moveZ.Equals(0.0f)))
        {
            curretnVelocity = new Vector3(moveX * speed, 0, moveZ * speed);
            CmdMovePlayer(curretnVelocity, gameObject.GetComponent<NetworkIdentity>().netId);

        }

        if (mana < 100)
        {
            mana += 20;
        }

        if (mana == 100)
        {

            if (Input.GetKey(KeyCode.Space))
            {
                rb.velocity = new Vector3(rb.velocity.x, 10, rb.velocity.z);
                mana -= 100;
            }

            if (Input.GetKey(KeyCode.J))
            {
                rb.velocity = new Vector3(rb.velocity.x * 3, rb.velocity.y, rb.velocity.z * 3);
                mana -= 100;
            }

        }


        if (isServer)
        {
            if (crt - ort >= Time.deltaTime * 4)
            {
                ort = crt;

                foreach (var player in PlayerPool)
                {
                    var playerRigidbody = player.Value.GetComponent<Rigidbody>();
                    RpcSyncObject(player.Key, playerRigidbody.position, playerRigidbody.velocity);
                }
            }
        }
    }

    [ClientRpc]
    public void RpcSyncObject(NetworkInstanceId id, Vector3 pos, Vector3 vel)
    {

        var player = PlayerPool[id];
        var playerRigidbody = player.GetComponent<Rigidbody>();
        playerRigidbody.position = pos;
        playerRigidbody.velocity = vel;

        //if (!positionBufferPool.ContainsKey(id))
        //{
        //    positionBufferPool.Add(id, new Queue<V   ector3>());
        //}
        //var positionBuffer = positionBufferPool[id];

        //if (!velocityBufferPool.ContainsKey(id))
        //{
        //    velocityBufferPool.Add(id, new Queue<Vector3>());
        //}
        //var velocityBuffer = velocityBufferPool[id];

        //var posDelta = playerRigidbody.position - pos;
        //var posInterpolation = posDelta / 4;
        //var velDelta = playerRigidbody.velocity - vel;
        //var velInterpolation = velDelta / 4;
        //for (int i = 1; i <= 4; i++)
        //{
            //positionBuffer.Enqueue(playerRigidbody.position + posInterpolation * i);
            //velocityBuffer.Enqueue(playerRigidbody.velocity + velInterpolation * i);
        //}
        //playerRigidbody.position = positionBuffer.Dequeue();
        //playerRigidbody.velocity = velocityBuffer.Dequeue();

    }

    [Command]
    public void CmdMovePlayer(Vector3 velocity, NetworkInstanceId id)
    {
        MovePlayer(velocity, id);
        RpcMovePlayer(velocity, id);
    }

    [ClientRpc]
    public void RpcMovePlayer(Vector3 velocity, NetworkInstanceId id)
    {
        MovePlayer(velocity, id);
    }

    private void MovePlayer(Vector3 velocity, NetworkInstanceId id)
    {
        var player = PlayerPool[id];
        var playerRigidbody = player.GetComponent<Rigidbody>();
        playerRigidbody.velocity = velocity;
    }


}
