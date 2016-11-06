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

public class StateObject
{
    // Client socket.
    public Socket workSocket = null;
    // Size of receive buffer.
    public const int BufferSize = 10000;

    public static String responseBuffer = String.Empty;

    // Receive buffer.
    public byte[] buffer = new byte[BufferSize];
    // Received data string.
    public StringBuilder sb = new StringBuilder();
}

public class AsynchronousClient : MonoBehaviour
{

    public Socket Client = new Socket(AddressFamily.InterNetwork,
      SocketType.Stream, ProtocolType.Tcp);

    private const int staticResourcesPort = 8080;
    private const string staticResourcesIP = "188.93.18.141";

    private const int rawPort = 5000;
    private const string rawIP = "188.93.18.139";
    public static AsynchronousClient Instance;
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

    public bool TutorialAuth;
    public bool GuestAuth;
    public bool UUIDAuth;
    public string UUID;

    private double timestamp;

    private string weekMarker = String.Format("{0}/60/60/24/7", ConvertToUnixTimestamp(DateTime.Now));
    private string monthMarker = String.Format("{0}/60/60/24/7/4", ConvertToUnixTimestamp(DateTime.Now));

    void Awake()
    {
        Instance = gameObject.GetComponent<AsynchronousClient>();
    }


    public static string SetHttpRequest(string serverIp, int port, string resourceKey, string cacheMarkerType)
    {
        string httpRequest = String.Format("http://{0}:{1}/{2}.json?{{{3}}}", serverIp, port, resourceKey,
            cacheMarkerType);
        return httpRequest;
    }

    private static double ConvertToUnixTimestamp(DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        TimeSpan diff = date - origin;
        return Math.Floor(diff.TotalSeconds);
    }

    private IEnumerator GetServerStaticResources(string url)
    {
        WWW www = new WWW(url);
        yield return www;

        Debug.Log(www.text);

        ParseData(www.text);

    }


    private static ManualResetEvent connectDone =
        new ManualResetEvent(false);

    private static ManualResetEvent sendDone =
        new ManualResetEvent(false);

    private static ManualResetEvent receiveDone =
        new ManualResetEvent(false);

    public static String response = String.Empty;
    public static string buffer = string.Empty;

    public static void RefreshEvents()
    {
        sendDone.Reset();
        //receiveDone.Reset();
    }

    public void StartClient()
    {
        try
        {
            IPHostEntry IPHostInfo = Dns.GetHostEntry(rawIP);

            foreach (IPAddress ip in IPHostInfo.AddressList)
            {
                Debug.Log(ip);
            }

            IPAddress cIPAddress = IPHostInfo.AddressList[0];

            IPEndPoint remoteEP = new IPEndPoint(cIPAddress, rawPort);

            Client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), Client);
            connectDone.WaitOne();
            if (GuestAuth)
            {
                Dictionary<string, string> dict = new Dictionary<string, string> { { "authorize", "guest" } };
                string json = Json.Serialize(dict);
                Debug.Log(json);
                Send(json + "\f");
            }

            if (UUIDAuth)
            {
                Dictionary<string, object> dict = new Dictionary<string, object> { { "authorize", "guest" } };
                Dictionary<string, string> uuid_dict = new Dictionary<string, string>
                    {
                        {"uuid", UUID}
                    };
                dict.Add("with", uuid_dict);
                dict.Add("mature", TutorialAuth);
                string json = Json.Serialize(dict);
                Debug.Log(json);
                Send(json + "\f");
            }

            Receive();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static void ConnectCallback(IAsyncResult ar)
    {

        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection.
            client.EndConnect(ar);

            Debug.Log("Socket connected to {0} " + client.RemoteEndPoint.ToString());

            // Signal that the connection has been made.
            connectDone.Set();

        }
        catch (Exception e)
        {

            Console.WriteLine(e.ToString());

        }

    }

    public void Receive()
    {
        try
        {

            // Create the state object.

            StateObject state = new StateObject();

            state.workSocket = Client;

            // Begin receiving the data from the remote device.
            Client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback),
                state);

            //byte[] bufferm = new byte[100000000];
            //client.Receive(bufferm);

            //Debug.Log(Encoding.UTF8.GetString(bufferm));

        }
        catch (Exception e)
        {

            Console.WriteLine(e.ToString());

        }

    }


    public static void ParseData(string data)
    {
        var nparsed = JSON.Parse(data);
        Debug.Log(nparsed.Count);
        foreach (JSONClass a in nparsed.AsArray)
        {
            Debug.Log(a.ToString());

        }
        //var arrData = nparsed.Count;
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;
            int bytesRead = client.EndReceive(ar);
            buffer += Encoding.UTF8.GetString(state.buffer, 0, bytesRead);
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback),
                state);
            Stack<string> mStack = new Stack<string>(buffer.Split('\f'));
            Debug.Log("Server Message Stack count is:" + mStack.Count);
            buffer = mStack.Pop();
            Debug.Log(buffer.Length);
            foreach (string message in mStack)
            {
                Debug.Log("Server Message Stack:" + message);
                ParseJSON(message);
            }
            //receiveDone.Set();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private void ParseJSON(string json)
    {
        var jsonData = JsonMapper.ToObject(json);

        var message = new KeyValuePair<string, JsonData>();

        foreach (var key in jsonData.Keys)
        {
            message = new KeyValuePair<string, JsonData>(key, jsonData);
            Debug.Log(message.Key);
        }

        switch (message.Key)
        {
            case "battle":
                ServerCTest.Instance.CBattleController.Initialize(json);
                break;
        }

    }

    public void Send(String data)
    {
        byte[] byteData = Encoding.UTF8.GetBytes(data);
        Client.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), Client);
    }

    public static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.
            int bytesSent = client.EndSend(ar);

            Debug.Log(String.Format("Sent {0} bytes to server.", bytesSent));

            // Signal that all bytes have been sent.

            // sendDone.Set();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
