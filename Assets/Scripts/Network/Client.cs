using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using LitJson;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MiniJSON;

public class Client {

    public Socket _clientSocket = new Socket(AddressFamily.InterNetwork,
  SocketType.Stream, ProtocolType.Tcp);
    private byte[] _recieveBuffer = new byte[8142];

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

    public string buffer = string.Empty;

    public event Action<string> OnMessageReceived;

    private static double ConvertToUnixTimestamp(DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        TimeSpan diff = date - origin;
        return Math.Floor(diff.TotalSeconds);
    }

    public void SetupConnection()
    {
        try
        {
            IPAddress adress;
            IPAddress.TryParse(rawIP, out adress);
            _clientSocket.Connect(new IPEndPoint(adress, rawPort));
        }
        catch (SocketException ex)
        {
            Debug.Log(ex.Message);
        }

        _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);

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
    }

    private void ReceiveCallback(IAsyncResult AR)
    {
        int recieved = _clientSocket.EndReceive(AR);
        if (recieved <= 0)
            return;
        byte[] recData = new byte[recieved];
        Buffer.BlockCopy(_recieveBuffer, 0, recData, 0, recieved);
        buffer += Encoding.UTF8.GetString(recData, 0, recieved);
        if (buffer.Contains('\f'))
        {
            string[] messages = buffer.Split(new char[] { '\f'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var message in messages)
            {
               OnMessage(message);
            }
            Array.Clear(_recieveBuffer, 0, _recieveBuffer.Length);
            buffer = string.Empty;
        }
        _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
    }

    private void OnMessage(string message)
    {
        if (OnMessageReceived != null)
            OnMessageReceived(message);
    }

    public void Send(string data)
    {
        byte[] byteData = Encoding.UTF8.GetBytes(data);
        SendData(byteData);
    }

    private void SendData(byte[] data)
    {
        SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
        socketAsyncData.SetBuffer(data, 0, data.Length);
        _clientSocket.SendAsync(socketAsyncData);
    }

}
