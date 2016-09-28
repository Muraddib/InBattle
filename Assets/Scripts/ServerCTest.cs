using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MiniJSON;
using SimpleJSON;

public class ServerCTest : MonoBehaviour
{

    public static ServerCTest Instance;
    public int BattleID;
    private const int staticResourcesPort = 8080;
    private const string staticResourcesIP = "188.93.18.141";

    private const int rawPort = 5000;
    private const string rawIP = "188.93.18.139";

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

    public static Socket Client = new Socket(AddressFamily.InterNetwork,
        SocketType.Stream, ProtocolType.Tcp);

    private IEnumerator GetServerStaticResources(string url)
    {
        WWW www = new WWW(url);
        yield return www;

        Debug.Log(www.text);

        AsynchronousClient.ParseData(www.text);

    }

    private void Awake()
    {
        Instance = gameObject.GetComponent<ServerCTest>();
    }

    private void Start()
    {
        //StartCoroutine(GetServerStaticResources(SetHttpRequest(serverIp, httpPort, httpGetCharactersInfo, monthMarker)));
        //StartCoroutine(GetServerStaticResources(SetHttpRequest(serverIp, httpPort, httpGetLocales, weekMarker)));
        //StartCoroutine(GetServerStaticResources(SetHttpRequest(serverIp, httpPort, httpGetShop, weekMarker)));
        //StartCoroutine(GetServerStaticResources(SetHttpRequest(serverIp, httpPort, httpGetOnline, ConvertToUnixTimestamp(DateTime.Now).ToString())));
    
        AsynchronousClient.StartClient();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application Quit, Socket shutdown");
        Client.Shutdown(SocketShutdown.Both);
        Client.Close();
    }


    // State object for receiving data from remote device.
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

    public class AsynchronousClient
    {
        // The port number for the remote device.
        //private const int port = 5000;
        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);

        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);

        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        // The response from the remote device.
        public static String response = String.Empty;
        public static string buffer = string.Empty;

        public static void RefreshEvents()
        {
            sendDone.Reset();
            //receiveDone.Reset();
        }

        public static void StartClient()
        {
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // The name of the 
                // remote device is "host.contoso.com".
                IPHostEntry IPHostInfo = Dns.GetHostEntry(rawIP);

                foreach (IPAddress ip in IPHostInfo.AddressList)
                {
                    Debug.Log(ip);
                }

                IPAddress cIPAddress = IPHostInfo.AddressList[0];

                IPEndPoint remoteEP = new IPEndPoint(cIPAddress, rawPort);

                // Create a TCP/IP socket.

                // Connect to the remote endpoint.
                Client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), Client);
                connectDone.WaitOne();
                if (ServerCTest.Instance.GuestAuth)
                {
                    Dictionary<string, string> dict = new Dictionary<string, string> {{"authorize", "guest"}};
                    string json = Json.Serialize(dict);
                    Debug.Log(json);
                    Send(Client, json + "\f");
                }

                if (ServerCTest.Instance.UUIDAuth)
                {
                    Dictionary<string, object> dict = new Dictionary<string, object> {{"authorize", "guest"}};
                    Dictionary<string, string> uuid_dict = new Dictionary<string, string>
                    {
                        {"uuid", ServerCTest.Instance.UUID}
                    };
                    dict.Add("with", uuid_dict);
                    dict.Add("mature", ServerCTest.Instance.TutorialAuth);
                    string json = Json.Serialize(dict);
                    Debug.Log(json);
                    Send(Client, json + "\f");
                }

                //Send(Client, "51132672\f7a0d42bbb39f676d1ea83c23491c4657\f");
                //sendDone.WaitOne();

                // Receive the response from the remote device.
                Receive(Client);
                //receiveDone.WaitOne();
                //RefreshEvents();
                // Write the response to the console.


                // Release the socket.
                //Client.Shutdown(SocketShutdown.Both);
                //Client.Close();

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
                Socket client = (Socket) ar.AsyncState;

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

        public static void Receive(Socket client)
        {
            try
            {

                // Create the state object.

                StateObject state = new StateObject();

                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback),
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

        //private static void ParseData(string data)
        //{
        //    var message = Json.Deserialize(data) as Dictionary<string, object>;

        //    foreach (KeyValuePair<string, object> pair in message)
        //    {

        //        Debug.Log("{ " + "key:" + pair.Key + " value:" + pair.Value + " }");

        //        //Debug.Log(type.Key.ToString() + " " + type.Value.ToString());


        //       // Debug.Log(type.Key + " " + type.Value);



        //        //string value = (string)pair.Value;


        //        //string cKey = pair.Key;


        //        //Debug.Log(cKey);



        //        switch (pair.Key)
        //        {

        //            case "redirect":


        //                if (pair.Value as string != null)
        //                {
        //                    Debug.Log(pair.Value as string);


        //                }

        //                //if (value == "refresh" || value == "duplicate" || value == "banned" || value == "destroy")
        //                //{
        //                //    //quiet_close = true;
        //                //    //puts("OK");
        //                //}
        //                //activate_title_button(value);
        //                break;
        //            case "info":

        //                if (pair.Value as Dictionary<string, object> != null)
        //                {

        //                    var infoDict = pair.Value as Dictionary<string, object>;

        //                    foreach (KeyValuePair<string, object> infoP in infoDict)
        //                    {
        //                        Debug.Log("info{ " + "key:" + infoP.Key + " value:" + infoP.Value + " }");

        //                        switch (infoP.Key)
        //                        {

        //                            case "user":



        //                                var userDict = infoP.Value as Dictionary<string, object>;


        //                                foreach (var userI in userDict)
        //                                {
        //                                    Debug.Log("user{ " + "key:" + userI.Key + " value:" + userI.Value + " }");

        //                                }

        //                                break;

        //                            case "items":

        //                                if (infoP.Value as List<object> != null)
        //                                {

        //                                    var itemsList = infoP.Value as List<object>;

        //                                    foreach (var item in itemsList)
        //                                    {
        //                                        if (item as Dictionary<string, object> != null)
        //                                        {

        //                                            var itemDict = item as Dictionary<string, object>;

        //                                            foreach (var o in itemDict)
        //                                            {
        //                                                //Debug.Log(o);
        //                                            }


        //                                        }
        //                                    }

        //                                }

        //                                break;

        //                        }


        //                    }


        //                }

        //                //character_refresh(value);
        //                break;
        //            case "characters":
        //                //character_list(value);
        //                break;
        //            case "online":
        //                //refresh_online();
        //                break;
        //            case "level_up":
        //                //level_up(value);
        //                break;
        //            case "present":
        //                //show_present();
        //                break;
        //            case "about":
        //                //about_activate(value);
        //                break;
        //            case "chat":

        //                if (pair.Value as Dictionary<string, object> != null)
        //                {

        //                    var infoDict = pair.Value as Dictionary<string, object>;

        //                    foreach (KeyValuePair<string, object> infoP in infoDict)
        //                    {
        //                        Debug.Log("chat{ " + "key:" + infoP.Key + " value:" + infoP.Value + " }");
        //                    }


        //                }


        //                //Debug.Log(type.Value.GetType().Name);

        //                //if (type.Value as Dictionary<string, object> != null)
        //                //{

        //                //    foreach (var VARIABLE in type.Value)
        //                //    {
        //                //        Debug.Log(VARIABLE);
        //                //    }

        //                //}

        //                //write_to_chat(value);
        //                break;
        //            case "flashback":
        //                //flashback(value);
        //                break;
        //            case "hall":
        //                //refresh_hall();
        //                break;
        //            case "battle":
        //                //battle_refresh(value);
        //                break;
        //            case "battler":
        //                //battler_refresh(value);
        //                break;
        //            case "action":
        //                //action_performed(value);
        //                break;
        //            case "message":
        //                //print(value);
        //                break;
        //            case "error":
        //                //show_error(value);
        //                break;
        //            case "sync":
        //                // reset_timestamp(value);
        //                break;
        //            case "farmers":
        //                break;
        //            case "options":
        //                break;
        //            case "battles":
        //                break;
        //            case "keeper":
        //                break;
        //            case "competent":
        //                // show_log(type, value);
        //                break;
        //        }
        //    }
        //}

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

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject) ar.AsyncState;
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
                }
                //receiveDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Send(Socket client, String data)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        public static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket) ar.AsyncState;

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

    /*
	 * {"battle":{"id":1627,"seed":334714069327943495777788188039447539275,"number":2,"min_lvl":1,"max_lvl":2,"width":6,"height":3,"round":1,"status":"battle","start_time":300,"round_time":120,"updated_at":1474814477,"variations":[],"est":1391906203,"mature":true},"items":[{"id":1628433,"template":"sword_of_warrior","status":"in_use","position":"right_hand","enchases":0}]}},{"id":3,"seed":176817120023849377057678116100736220693,"gex":{"x":6,"y":1},"kind":"hollow","size":null},{"id":4,"seed":56507082204359919425935633483715266246,"gex":{"x":6,"y":3},"kind":"hollow","size":null},{"id":5,"seed":129702209111302248299830023102912484010,"gex":{"x":3,"y":1},"kind":"obstacle","size":null},{"id":6,"seed":35562155197979381540295376935895979293,"gex":{"x":5,"y":1},"kind":"obstacle","size":null},{"id":7,"seed":57715622161967998714868162822420519431,"gex":{"x":3,"y":3},"kind":"obstruction","size":null,"solidity":3,"injury":0},{"id":8,"seed":312522174294341243229774697148625352991,"gex":{"x":4,"y":3},"kind":"obstruction","size":null,"solidity":3,"injury":0}],"teams":[{"index":0,"color":"red","need_password":false},{"index":1,"color":"blue","need_password":false}]}}
	 * 
	 * 
	 */

    public void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "Register"))
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            Dictionary<string, string> gener_dict = new Dictionary<string, string>
            {
                {"name", "Luke Skywalker"},
                {"race", "elf"},
                {"gender", "male"}
            };
            dict.Add("client#generate", gener_dict);
            string s = Json.Serialize(dict);
            Debug.Log(s);
            AsynchronousClient.Send(Client, s + "\f");
            AsynchronousClient.Receive(Client);
        }

        /*
		   battle#create: создать битву, принимаются параметры: number - количество участников, min_lvl - минимальный уровень, max_lvl - максимальный уровень, variations - типы битвы и их подробные настройки
		*/

        if (GUI.Button(new Rect(200f, 0f, 100f, 100f), "Battle"))
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            Dictionary<string, object> gener_dict = new Dictionary<string, object>
            {
                {"number", 2},
                {"min_lvl", 1},
                {"max_lvl", 2}
            };
            dict.Add("battle#create", gener_dict);
            string s = Json.Serialize(dict);
            Debug.Log(s);
            AsynchronousClient.Send(Client, s + "\f");
            AsynchronousClient.Receive(Client);
        }

        if (GUI.Button(new Rect(100f, 0f, 100f, 100f), "Join"))
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            Dictionary<string, object> gener_dict = new Dictionary<string, object>
            {
                {"id", BattleID}
            };
            dict.Add("battle#connect", gener_dict);
            string s = Json.Serialize(dict);
            Debug.Log(s);
            AsynchronousClient.Send(Client, s + "\f");
            AsynchronousClient.Receive(Client);
        }

        if (GUI.Button(new Rect(300f, 0f, 100f, 100f), "Get Battles"))
        {
            StartCoroutine(GetServerStaticResources(SetHttpRequest(staticResourcesIP, staticResourcesPort, httpGetHall, ConvertToUnixTimestamp(DateTime.Now).ToString())));
        }
    }
}