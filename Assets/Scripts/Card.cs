using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public string id;
    public string playerId;


    private void Update()
    {
        SocketManager socketManager = GameObject.Find("SocketManager").GetComponent<SocketManager>();
        this.playerId = socketManager.playerData.id;
    }
}
