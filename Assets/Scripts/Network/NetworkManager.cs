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

    //public event Action<string> OnMessageRedirect;

    public class BattleDataEventArgs : EventArgs
    {
        public BattleData BattleData { get; set; }
    }

    public class UserInfoEventArgs : EventArgs
    {
        public PlayerData Data { get; set; }
    }

    public event EventHandler<BattleDataEventArgs> OnMessageBattle = (sender, e) => { };
    public event EventHandler<UserInfoEventArgs> OnMessageInfo = (sender, e) => { };

    public Queue<KeyValuePair<string, string>> ServerMessages = new Queue<KeyValuePair<string, string>>();

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

    private void EvaluateMessages(KeyValuePair<string, string> message)
    {
        switch (message.Key)
        {
            case "battle":
                Debug.Log("battle");
                if (OnMessageBattle != null) OnMessageBattle(this, new BattleDataEventArgs { BattleData = JsonMapper.ToObject<BattleData>(message.Value) });
                break;
            case "info":
                Debug.Log("info");
                if (OnMessageInfo != null) OnMessageInfo(this, new UserInfoEventArgs{ Data = JsonMapper.ToObject<PlayerData>(message.Value) });
                break;
        }
    }

    private void ParseMessage(string json)
    {
        var jsonData = JsonMapper.ToObject(json);
        var message = new KeyValuePair<string, string>();
        foreach (var key in jsonData.Keys)
        {
            message = new KeyValuePair<string, string>(key, json);
        }
    
        ServerMessages.Enqueue(message);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application Quit, Socket shutdown");
        _client._clientSocket.Shutdown(SocketShutdown.Both);
        _client._clientSocket.Close();
    }

    void Update()
    {
        if (ServerMessages.Count > 0)
        {
            EvaluateMessages(ServerMessages.Dequeue());
        }
    }
}
