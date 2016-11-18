using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using LitJson;

public class NetworkManager : MonoBehaviour
{

    public static NetworkManager Instance;

    private Client _client { get; set; }

    public event Action<string> OnMessageRedirect;
    public event Action<string> OnMessageInfo;
    public event Action<BattleData> OnMessageBattle;

    void Awake()
    {
        Instance = gameObject.GetComponent<NetworkManager>();
    }

    public void Initialize()
    {
        _client = new Client
        {
            UUID = "0da908d1-8342-48f2-bd31-53e571759dab",
            UUIDAuth = true,
            TutorialAuth = true
        };
        _client.OnMessageReceived += OnClientMessageReceived;
        _client.SetupConnection();
    }

    private void OnClientMessageReceived(string message)
    {
        Debug.Log(message);
        ParseMessage(message);
    }

    public void Send(string message)
    {
        _client.Send(message);
    }

    private void ParseMessage(string json)
    {
        var jsonData = JsonMapper.ToObject(json);
        var message = new KeyValuePair<string, JsonData>();
        foreach (var key in jsonData.Keys)
        {
            message = new KeyValuePair<string, JsonData>(key, jsonData);
        }
        switch (message.Key)
        {
            case "battle":
                if (OnMessageBattle != null) OnMessageBattle(JsonMapper.ToObject<BattleData>(json));
                break;
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application Quit, Socket shutdown");
        _client._clientSocket.Shutdown(SocketShutdown.Both);
        _client._clientSocket.Close();
    }
}
