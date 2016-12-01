using HtmlParser;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class ChatController : MonoBehaviour
{
    public Text MainChatText;
    public string textText;
    public float chatMaxHeight;
    public RectTransform ContentRect;
    public Scrollbar VerticalScrollbar;

    void Awake()
    {
        DontDestroyOnLoad(gameObject.transform.parent.gameObject);
        NetworkManager.Instance.OnMessageChat += NetworkManager_OnMessageChat;
    }

    void NetworkManager_OnMessageChat(object sender, NetworkManager.ChatEventArgs e)
    {
        AddChatMessage(e.ChatMessage);
    }

    void AddChatMessage(string message)
    {
        MainChatText.text += "\n" + message;
        ScanLinks(message);
        SetContentViewRectHeight(MainChatText.preferredHeight);
        VerticalScrollbar.Rebuild(CanvasUpdate.Prelayout);
        VerticalScrollbar.value = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AddChatMessage(textText);
        }
    }

    void SetContentViewRectHeight(float height)
    {
        ContentRect.sizeDelta = new Vector2(0f, height);
    }

    protected void ScanLinks(string message)
    {
        HtmlTag tag;
        HtmlParser.HtmlParser parse = new HtmlParser.HtmlParser(message);
        while (parse.ParseNext("a", out tag))
        {

            foreach (var kv in tag.Attributes)
            {
                Debug.Log("key: " + kv.Key + " value: " + kv.Value);
            }

            //string value;
            //if (tag.Attributes.TryGetValue("href", out value))
            //{
              

            //    // value contains URL referenced by this link
            //}

        }
    }




}
