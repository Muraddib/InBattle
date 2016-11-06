using System;
using MiniJSON;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using SimpleJSON;
using System;
using System.Security.AccessControl;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public string battle_json;
    [SerializeField] public BattleData currentBattle;
    public GameObject HexPrefab;
    public Vector2 HexSize;
    public event EventHandler<HexClickedEventArgs> OnClicked = (sender, e) => { };

    public Material HexHover;
    public Material HexNormal;
    public Material HexSelected;
    public List<Hex> Hexes;
    public Hex SelectedHex;

    public GameObject OrcPrefab;
    public GameObject ElfPrefab;
    public GameObject HumanPrefab;
    public GameObject DwarfPrefab;
    public GameObject UndeadPrefab;

    public List<GameObject> BattleCharacters;

    public Vector2 ActionsFrameSize;
    public bool debugJSON;
    public class HexClickedEventArgs : EventArgs
    {
    }

    void Start()
    {
        if (debugJSON) Initialize(battle_json);
    }

    public void Initialize(string b_json)
    {
        currentBattle = JsonMapper.ToObject<BattleData>(b_json);
        CreateHexMap();
    }

    private void CreateHexMap()
    {
        var data = JsonMapper.ToObject(battle_json);

        //battle = new Battle
        //    {
        //        id = (int) data["battle"]["id"],
        //        //seed = (int) data["battle"]["seed"],
        //        status = (string) data["battle"]["status"],
        //        number = (int) data["battle"]["number"],
        //        min_lvl = (byte) data["battle"]["min_lvl"],
        //        max_lvl = (byte) data["battle"]["max_lvl"],
        //        width = (int) data["battle"]["width"],
        //        height = (int) data["battle"]["height"],
        //        round = (int) data["battle"]["round"],
        //        start_time = (int) data["battle"]["start_time"],
        //        round_time = (int) data["battle"]["round_time"],
        //        updated_at = (int) data["battle"]["updated_at"],
        //        variations = null,
        //        essences = null
        //    };

        //foreach (var key in data.Keys)
        //{
        //    Debug.Log(key);
        //}

        //Debug.Log(data["battle"]["seed"].GetJsonType());

        GameObject fieldRoot = new GameObject("Battlefield root");
        fieldRoot.transform.position = Vector3.zero;

        int countX = currentBattle.battle.width;
        int countY = currentBattle.battle.height;
        Hexes = new List<Hex>();
        
        for (int i = 0; i < countX; i++)
        {
            for (int j = 0; j < countY; j++)
            {
                GameObject hex =
                      Instantiate(HexPrefab, new Vector3(i % 2 == 0 ? j * 7f + 3.5f - (countX * 7f - 3.5f) * 0.5f : j * 7f - (countX * 7f - 3.5f) * 0.5f, 0f, -i * 6f + ((countY-1)*6f)*0.5f), Quaternion.identity)
                        as GameObject;
                hex.transform.parent = fieldRoot.transform;
                var nHex = hex.AddComponent<Hex>();
                nHex.X = j+1;
                nHex.Y = i+1;
                nHex.Index = Hexes.Count;
                Hexes.Add(nHex);
            }
        }

        BattleCharacters = new List<GameObject>();

        foreach (var essence in currentBattle.battle.essences)
        {
            if (essence.kind == "hollow")
            {
                var gex = Hexes.Find(a => a.X == essence.gex.x && a.Y == essence.gex.y).gameObject;
                if (gex)
                {
                    gex.GetComponent<Hex>().Visible = false;
                    gex.GetComponent<MeshRenderer>().enabled = false;
                    gex.GetComponent<Collider>().enabled = false;
                }
            }

            if (essence.kind == "battler")
            {
                var gex = Hexes.Find(a => a.X == essence.gex.x && a.Y == essence.gex.y).gameObject;
                if (gex)
                {
                    GameObject battler = null;
                    switch (essence.info.race)
                    {
                        case "elf":
                            battler = Instantiate(ElfPrefab, gex.gameObject.transform.position + Vector3.up,
                                Quaternion.Euler(90f, 0f, 0f)) as GameObject;
                            break;
                        case "human":
                            battler = Instantiate(HumanPrefab, gex.gameObject.transform.position + Vector3.up,
                                Quaternion.Euler(90f, 0f, 0f)) as GameObject;
                            break;
                        case "dwarf":
                            battler = Instantiate(DwarfPrefab, gex.gameObject.transform.position + Vector3.up,
                                Quaternion.Euler(90f, 0f, 0f)) as GameObject;
                            break;
                        case "orc":
                            battler = Instantiate(OrcPrefab, gex.gameObject.transform.position + Vector3.up,
                                Quaternion.Euler(90f, 0f, 0f)) as GameObject;
                            break;
                        case "undead":
                            battler = Instantiate(UndeadPrefab, gex.gameObject.transform.position + Vector3.up,
                                Quaternion.Euler(90f, 0f, 0f)) as GameObject;
                            break;
                    }

                }
            }
        }
    }

    private void SelectHex(Hex hex)
    {
        Hexes.ForEach(a=>a.GetComponent<MeshRenderer>().material = HexNormal);
        SelectedHex = hex;
        SelectedHex.gameObject.GetComponent<MeshRenderer>().material = HexSelected;
    }

    public void OnABWindowCloseClick()
    {
        AttackBlockWindow.SetActive(false);
        inTargetSelection = false;
    }

    public void OnABWindowConfirmClick()
    {
        //AttackBlockWindow.SetActive(false);
        //inTargetSelection = false;
        if (LeftHandAttackGroup.ActiveToggles().Count == 0 && LeftHandBlockGroup.ActiveToggles().Count < 2) return;
        if (RightHandAttackGroup.ActiveToggles().Count == 0 && RightHandBlockGroup.ActiveToggles().Count < 2) return;

        GetTargetsData();
    }


    private void GetTargetsData()
    {
        Dictionary<string,string> handActions = new Dictionary<string, string>();

        bool leftIsAttack = LeftHandAttackGroup.ActiveToggles().Count > 0;
        bool rightIsAttack = RightHandAttackGroup.ActiveToggles().Count > 0;

        handActions.Add("right_type", rightIsAttack ? "attack" : "block");
        handActions.Add("left_type", leftIsAttack ? "attack" : "block");

        if (leftIsAttack)
        {
            handActions.Add("left_first", LeftHandAttackGroup.ActiveToggles().Find(a=>a.isOn).gameObject.GetComponent<CustomToggle>().ToggleBodyTarget.ToString());
        }
        else
        {
            var leftBlockGroupToggles = LeftHandBlockGroup.ActiveToggles().FindAll(a => a.isOn);
            handActions.Add("left_first", leftBlockGroupToggles[0].GetComponent<CustomToggle>().ToggleBodyTarget.ToString());
            handActions.Add("left_second", leftBlockGroupToggles[1].GetComponent<CustomToggle>().ToggleBodyTarget.ToString());
        }

        if (rightIsAttack)
        {
            handActions.Add("right_first", RightHandAttackGroup.ActiveToggles().Find(a => a.isOn).gameObject.GetComponent<CustomToggle>().ToggleBodyTarget.ToString());
        }
        else
        {
            var rightBlockGroupToggles = RightHandBlockGroup.ActiveToggles().FindAll(a => a.isOn);
            handActions.Add("right_first", rightBlockGroupToggles[0].GetComponent<CustomToggle>().ToggleBodyTarget.ToString());
            handActions.Add("right_second", rightBlockGroupToggles[1].GetComponent<CustomToggle>().ToggleBodyTarget.ToString());
        }

        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("battle#action", handActions);
        string s = Json.Serialize(dict);
        Debug.Log(s);
    }


    private void Update()
    {
        if(inTargetSelection) return;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.transform.gameObject.tag == "Hex")
        {
            //Debug.Log(hit.transform.gameObject.name);
            if (Input.GetMouseButtonDown(0))
            {
                SelectHex(hit.transform.gameObject.GetComponent<Hex>());
                var eventArgs = new HexClickedEventArgs();
                OnClicked(this, eventArgs);
            }
        }
    }


    public bool attack_left_head;
    public bool attack_left_body;
    public bool attack_left_left_hand;
    public bool attack_left_right_hand;
    public bool attack_left_legs;

    public bool attack_right_head;
    public bool attack_right_body;
    public bool attack_right_left_hand;
    public bool attack_right_right_hand;
    public bool attack_right_legs;

    public bool left_hand_used;
    public bool right_hand_used;


    public ToggleGroupCustom LeftHandAttackGroup;
    public ToggleGroupCustom RightHandAttackGroup;
    public ToggleGroupCustom LeftHandBlockGroup;
    public ToggleGroupCustom RightHandBlockGroup;

    public GameObject AttackBlockWindow;
    public bool inTargetSelection;


    public void OnToggleClick(GameObject go)
    {
        ToggleGroupCustom group = go.GetComponent<CustomToggle>().GroupTarget;
        group.SetToggleActive(go.GetComponent<Toggle>());
        switch ( group.GroupHandType)
        {
                case ToggleGroupCustom.HandType.LeftHandAttack:
                LeftHandBlockGroup.SetTogglesInactive();
                break;
                case ToggleGroupCustom.HandType.RightHandAttack:
                RightHandBlockGroup.SetTogglesInactive();
                break;
                case ToggleGroupCustom.HandType.LeftHandBlock:
                LeftHandAttackGroup.SetTogglesInactive();
                break;
                case ToggleGroupCustom.HandType.RightHandBlock:
                RightHandAttackGroup.SetTogglesInactive();
                break;
        }
    }

    void OnGUI()
    {

        if (SelectedHex == null) return;

            if (GUI.Button(new Rect(Screen.width/2f - 100f, Screen.height - 100f, 100f, 100f), "Walk"))
            {
                Debug.Log("Walk to:" + "X:" + SelectedHex.X + "Y:" + SelectedHex.Y);
            }

            if (GUI.Button(new Rect(Screen.width / 2f, Screen.height - 100f, 100f, 100f), "Attack/Block"))
            {
                Debug.Log("Walk to:" + "X:" + SelectedHex.X + "Y:" + SelectedHex.Y);
                AttackBlockWindow.SetActive(true);
            }

            //if (GUI.Button(new Rect(Screen.width / 2f+100f, Screen.height - 100f, 100f, 100f), "Toggles"))
            //{
            //  // DebugToogleGroup();
            //}
            //GUI.BeginGroup(new Rect(Screen.width / 2f - ActionsFrameSize.x/2f, Screen.height / 2f - ActionsFrameSize.y / 2f, ActionsFrameSize.x, ActionsFrameSize.y));
            //GUI.Box(new Rect(0, 0, ActionsFrameSize.x, ActionsFrameSize.y), "Attack/Block");

            //attack_left_head = GUI.Toggle(new Rect(10, 10, 20, 20), attack_left_head, "");

            //GUI.EndGroup();
            
    }
}
