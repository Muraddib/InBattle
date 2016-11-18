using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using LitJson;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MiniJSON;
using SimpleJSON;

public class ServerCTest : MonoBehaviour
{
    public BattleController CBattleController;
    public static ServerCTest Instance;
    public int BattleID;

    public Client client;

    private void Awake()
    {
        Instance = gameObject.GetComponent<ServerCTest>();
    }

    private void Start()
    {
    }

  
}