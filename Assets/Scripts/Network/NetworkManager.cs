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

    public class ChatEventArgs : EventArgs
    {
        public string ChatMessage { get; set; }
    }

    public class OnlineListEventArgs : EventArgs
    {
        public Player[] OnlinePlayersList { get; set; }
    }

    public event EventHandler<BattleDataEventArgs> OnMessageBattle = (sender, e) => { };
    public event EventHandler<UserInfoEventArgs> OnMessageInfo = (sender, e) => { };
    public event EventHandler<ChatEventArgs> OnMessageChat = (sender, e) => { };
    public event EventHandler<OnlineListEventArgs> OnMessageOnlineList = (sender, e) => { };

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
            case "chat":
                Debug.Log("chat");
                if (OnMessageChat != null) OnMessageChat(this, new ChatEventArgs { ChatMessage = JsonMapper.ToObject(message.Value)["chat"]["text"].ToString() });
                break;
            case "online":
                Debug.Log("online");
                StartCoroutine(GetServerStaticResources(SetHttpRequest(staticResourcesIP, staticResourcesPort, httpGetOnline, ConvertToUnixTimestamp(DateTime.Now).ToString()),
                    onDone:
                        wwwText =>
                        {
                            var path = (@"Assets/Data/online.txt");
                            //System.IO.File.WriteAllText(path, wwwText);
                            var playersOnline = JsonMapper.ToObject<Player[]>(wwwText);
                            if (OnMessageOnlineList != null) OnMessageOnlineList(this, new OnlineListEventArgs {OnlinePlayersList = playersOnline});
                        }
                    ));
            
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


    /// <summary>
    /// Game static resources
    /// </summary>
    private const string httpGetCharactersInfo = "info";
    private const string httpGetLocales = "locales/ru";
    private const string httpGetShop = "shop";
    private const string httpGetHall = "hall";
    private const string httpGetOnline = "online";
    private const string httpGetRating = "rating_top";
    private const string httpGetAuthority = "authority_top";
    private const string httpGetConsequence = "consequence_top";

    private const int staticResourcesPort = 8080;
    private const string staticResourcesIP = "188.93.18.141";

    private string weekMarker = String.Format("{0}/60/60/24/7", ConvertToUnixTimestamp(DateTime.Now));
    private string monthMarker = String.Format("{0}/60/60/24/7/4", ConvertToUnixTimestamp(DateTime.Now));

    private static double ConvertToUnixTimestamp(DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        TimeSpan diff = date - origin;
        return Math.Floor(diff.TotalSeconds);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(300f, 0f, 100f, 100f), "Get Battles"))
        {
            StartCoroutine(GetServerStaticResources(SetHttpRequest(staticResourcesIP, staticResourcesPort, httpGetHall, ConvertToUnixTimestamp(DateTime.Now).ToString())));
        }
    }

    public static string SetHttpRequest(string serverIp, int port, string resourceKey, string cacheMarkerType)
    {
        string httpRequest = String.Format("http://{0}:{1}/{2}.json?{{{3}}}", serverIp, port, resourceKey,
            cacheMarkerType);
        return httpRequest;
    }

    private IEnumerator GetServerStaticResources(string url, Action<string> onDone = null)
    {
        WWW www = new WWW(url);
        yield return www;
        Debug.Log(www.bytesDownloaded);
        Debug.Log(www.text);
        Debug.Log(www.error);
        if (onDone != null) onDone(www.text);
    }
}
