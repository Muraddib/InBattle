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



    private const int rawPort = 5000;
    private const string rawIP = "188.93.18.139";


    public bool TutorialAuth;
    public bool GuestAuth;
    public bool UUIDAuth;
    public string UUID;

    private double timestamp;

  
    public string buffer = string.Empty;

    public event Action<string> OnMessageReceived;

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
