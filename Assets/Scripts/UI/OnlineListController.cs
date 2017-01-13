using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class OnlineListController : MonoBehaviour
{
    private Player[] currentOnlinePlayers;
    private Dictionary<int, GameObject> onlineListItems;
    public GameObject OnlineListItemPrefab;
    public RectTransform listItemsRoot;
    public Text OnlineCounterText;

    void Awake()
    {
        NetworkManager.Instance.OnMessageOnlineList += Instance_OnMessageOnlineList;
        onlineListItems = new Dictionary<int, GameObject>();
        currentOnlinePlayers = new Player[0];
    }

    private void Instance_OnMessageOnlineList(object sender, NetworkManager.OnlineListEventArgs e)
    {
        Debug.Log("Online List Update");
        Debug.Log(e.OnlinePlayersList.Length);

        for (int i = 0; i < e.OnlinePlayersList.Length; i++)
        {
            var player = e.OnlinePlayersList[i];
            if (!onlineListItems.ContainsKey(player.id))
            {
                var listItem = Instantiate(OnlineListItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                listItem.transform.SetParent(listItemsRoot, false);
                onlineListItems.Add(player.id, listItem);
                listItem.GetComponent<OnlineListItem>().Init(null, player);
            }
        }

        var playerItemsToDelete = currentOnlinePlayers.Select(a => a.id).Except(e.OnlinePlayersList.Select(a => a.id)).ToArray();
        Debug.Log("Deleting:" + playerItemsToDelete.Count() + " missing players");
        for (int i = 0; i < playerItemsToDelete.Count(); i++)
        {
            Destroy(onlineListItems[playerItemsToDelete[i]]);
            onlineListItems.Remove(playerItemsToDelete[i]);
        }
        currentOnlinePlayers = e.OnlinePlayersList;
        OnlineCounterText.text = "Players online: " + currentOnlinePlayers.Length.ToString();
    }
}
