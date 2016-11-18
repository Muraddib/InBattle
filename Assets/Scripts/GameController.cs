using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MiniJSON;
using UnityEditor;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int BattleID;

    public static GameController Instance;
    private NetworkManager _networkManager;
    public Scene[] Scenes;
    public GameObject BattleControllerPrefab;
    public BattleController ActiveBattle;

    [SerializeField] private GameStates _gameState;

    public void Awake()
    {
        Instance = gameObject.GetComponent<GameController>();
        _networkManager = gameObject.GetComponent<NetworkManager>();
        _networkManager.OnMessageBattle += _networkManager_OnMessageBattle;
        _networkManager.Initialize();
    }

    private void _networkManager_OnMessageBattle(BattleData battleData)
    {
        switch (_gameState)
        {
                case GameStates.StateCity:
                LoadBattle(battleData);
                break;
        }
    }

    private void LoadBattle(BattleData data)
    {
        _gameState = GameStates.StateBattle;
        SceneManager.LoadScene("Battle");
        StartCoroutine(WaitForSceneLoad(SceneManager.GetSceneByName("Battle"), (battle) => { OnSceneLoaded(data); }));
    }

    public IEnumerator WaitForSceneLoad(Scene scene, Action<BattleData> onDone)
    {
        while (!scene.isLoaded)
        {
            yield return null;
        }
        Debug.Log("Setting active scene..");
        SceneManager.SetActiveScene(scene);
    }

    private void OnSceneLoaded(BattleData battle)
    {
       var battleController = Instantiate(BattleControllerPrefab) as GameObject;
       ActiveBattle = battleController.GetComponent<BattleController>();
       ActiveBattle.Initialize(battle);
    }

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
            NetworkManager.Instance.Send(s + "\f");
        }

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
            NetworkManager.Instance.Send(s + "\f");
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
            NetworkManager.Instance.Send(s + "\f");
        }

        if (GUI.Button(new Rect(300f, 0f, 100f, 100f), "Get Battles"))
        {
            //StartCoroutine(GetServerStaticResources(SetHttpRequest(staticResourcesIP, staticResourcesPort, httpGetHall, ConvertToUnixTimestamp(DateTime.Now).ToString())));
        }

        if (GUI.Button(new Rect(400f, 0f, 100f, 100f), "Potion"))
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("client#potion", "");
            string s = Json.Serialize(dict);
            Debug.Log(s);
            NetworkManager.Instance.Send(s + "\f");
        }

        if (GUI.Button(new Rect(500f, 0f, 100f, 100f), "Add Ghost"))
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("battle#ghost", "");
            string s = Json.Serialize(dict);
            Debug.Log(s);
            NetworkManager.Instance.Send(s + "\f");
        }
    }

}


