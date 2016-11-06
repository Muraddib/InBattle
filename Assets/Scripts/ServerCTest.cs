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
    
        AsynchronousClient.Instance.StartClient();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application Quit, Socket shutdown");
        AsynchronousClient.Instance.Client.Shutdown(SocketShutdown.Both);
        AsynchronousClient.Instance.Client.Close();
    }




    // State object for receiving data from remote device.




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
            AsynchronousClient.Instance.Send(s + "\f");
            AsynchronousClient.Instance.Receive();
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
            AsynchronousClient.Instance.Send(s + "\f");
            AsynchronousClient.Instance.Receive();
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
            AsynchronousClient.Instance.Send(s + "\f");
            AsynchronousClient.Instance.Receive();
        }

        if (GUI.Button(new Rect(300f, 0f, 100f, 100f), "Get Battles"))
        {
            //StartCoroutine(GetServerStaticResources(SetHttpRequest(staticResourcesIP, staticResourcesPort, httpGetHall, ConvertToUnixTimestamp(DateTime.Now).ToString())));
        }

         if(GUI.Button(new Rect(400f, 0f, 100f, 100f), "Potion"))
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("client#potion", "");
            string s = Json.Serialize(dict);
            Debug.Log(s);
            AsynchronousClient.Instance.Send(s + "\f");
            AsynchronousClient.Instance.Receive();
        }

         if (GUI.Button(new Rect(500f, 0f, 100f, 100f), "Add Ghost"))
         {
             Dictionary<string, object> dict = new Dictionary<string, object>();
             dict.Add("battle#ghost", "");
             string s = Json.Serialize(dict);
             Debug.Log(s);
             AsynchronousClient.Instance.Send(s + "\f");
             AsynchronousClient.Instance.Receive();
         }

        
    }
}