using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using SimpleJSON;
using System;
using System.Security.AccessControl;

public class BattleController : MonoBehaviour
{
    public string battle_json;
    [SerializeField] public BattleData currentBattle;
    public GameObject HexPrefab;
    public Vector2 HexSize;
    public event EventHandler<HexClickedEventArgs> OnClicked = (sender, e) => { };

    public Material HexHover;
    public Material HexNormal;
    public List<Hex> Hexes;
    public Hex SelectedHex;


    public class HexClickedEventArgs : EventArgs
    {
    }

    private void Start()
    {
        currentBattle = JsonMapper.ToObject<BattleData>(battle_json);
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
        }
    }

    private void SelectHex(Hex hex)
    {
        SelectedHex = hex;
    }

    private void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.transform.gameObject.tag == "Hex")
        {
            Debug.Log(hit.transform.gameObject.name);
            if (Input.GetMouseButtonDown(0))
            {
                SelectHex(hit.transform.gameObject.GetComponent<Hex>());
                var eventArgs = new HexClickedEventArgs();
                OnClicked(this, eventArgs);
            }
        }
    }
}
