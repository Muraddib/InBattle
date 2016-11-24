using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using MiniJSON;
using UnityEditor;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public string BattleTest;
    public int BattleID;
    public static GameController Instance;
    private NetworkManager _networkManager;
    public Scene[] Scenes;
    public GameObject BattleControllerPrefab;
    public BattleController ActiveBattle;
    public GameUIFormsKeeper GameUIForms;
    public UserInfo PlayerCharacter;

    [SerializeField] private GameStates _gameState;

    public void Awake()
    {
        Instance = gameObject.GetComponent<GameController>();
        _networkManager = gameObject.AddComponent<NetworkManager>();
        _networkManager.Initialize();
        _networkManager.OnMessageBattle += _networkManager_OnMessageBattle;
        _networkManager.OnMessageInfo += _networkManager_OnMessageInfo;
        DontDestroyOnLoad(gameObject);
        LoadCity();
    }

    private void _networkManager_OnMessageInfo(object sender, NetworkManager.UserInfoEventArgs user)
    {
        PlayerCharacter = user.Data.info;
        Debug.Log(PlayerCharacter.id);
        Debug.Log(PlayerCharacter.name);
    }

    private void _networkManager_OnMessageBattle(object obj, NetworkManager.BattleDataEventArgs battleData)
    {
        Debug.Log("On battle message");
        switch (_gameState)
        {
                case GameStates.StateCity:
                LoadBattle(battleData.BattleData);
                break;
                case GameStates.StateBattle:
                if(ActiveBattle!=null)
                ActiveBattle.UpdateButtle(battleData.BattleData);
                break;
        }
    }

    private void OnBattleEnd(object obj, EventArgs e)
    {
        if (ActiveBattle != null)
        {
            ActiveBattle.OnBattleEnd -= OnBattleEnd;
            ActiveBattle = null;
        }
        LoadCity();
    }

    private void LoadCity()
    {
        _gameState = GameStates.StateCity;
        SceneManager.LoadScene("City", LoadSceneMode.Single);
        StartCoroutine(WaitForSceneLoad(SceneManager.GetSceneByName("City")));
    }

    private void LoadBattle(BattleData data)
    {
        _gameState = GameStates.StateBattle;
        SceneManager.LoadScene("Battle", LoadSceneMode.Single);
        StartCoroutine(WaitForSceneLoad(SceneManager.GetSceneByName("Battle"), data));
    }

    public IEnumerator WaitForSceneLoad(Scene scene, BattleData battle = null)
    {
        while (!scene.isLoaded)
        {
            yield return null;
        }
        Debug.Log("Setting active scene..");
        SceneManager.SetActiveScene(scene);

        if (battle != null)
        {
            OnSceneLoaded(battle);
        }
    }

    private void OnSceneLoaded(BattleData battle)
    {
       var battleController = Instantiate(BattleControllerPrefab) as GameObject;
       ActiveBattle = battleController.GetComponent<BattleController>();
       battleController.GetComponent<BattleUIController>().Init(GameUIForms.UIForms.BattleUIForms, GameUIForms.UIForms.BattleUIRoot);
       ActiveBattle.Initialize(battle);
       ActiveBattle.OnBattleEnd += OnBattleEnd;
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

        if (GUI.Button(new Rect(600f, 0f, 100f, 100f), "TestBattle"))
        {
            var battle = JsonMapper.ToObject<BattleData>(BattleTest);
            _networkManager_OnMessageBattle(this, new NetworkManager.BattleDataEventArgs { BattleData = battle });
        }

    }

}


