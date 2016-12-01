using UnityEngine;
using System.Collections;

public class OnlineListController : MonoBehaviour {

    void Awake()
    {
        NetworkManager.Instance.OnMessageOnline += NetworkManager_OnMessageOnline;
    }

    void NetworkManager_OnMessageOnline(object sender, System.EventArgs e)
    {

    }
}
