using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using SimpleJSON;
using System;

public class BattleController : MonoBehaviour
{
    // Use this for initialization
    public string battle_json;
    [SerializeField]
    public Battle battle;
    public GameObject HexPrefab;

    private void Start()
    {
        CreateHexMap();
    }

    private void CreateHexMap()
    {
        var data = JsonMapper.ToObject(battle_json);
        battle = new Battle
            {
                id = (int) data["battle"]["id"],
                seed = (int) data["battle"]["seed"],
                status = (string) data["battle"]["status"],
                number = (int) data["battle"]["number"],
                min_lvl = (byte) data["battle"]["min_lvl"],
                max_lvl = (byte) data["battle"]["max_lvl"],
                width = (int) data["battle"]["width"],
                height = (int) data["battle"]["height"],
                round = (int) data["battle"]["round"],
                start_time = (int) data["battle"]["start_time"],
                round_time = (int) data["battle"]["round_time"],
                updated_at = (int) data["battle"]["updated_at"],
                variations = null,
                essences = null
            };

        foreach (var key in data.Keys)
        {
            Debug.Log(key);
        }

        Debug.Log(data["battle"]["seed"].GetJsonType());

        for (int i = 0; i < battle.height; i++)
        {
            for (int j = 0; j < battle.width; j++)
            {
                GameObject hex =
                    Instantiate(HexPrefab, new Vector3(i%2 == 0 ? j*7f + 3.5f : j*7f, 0f, -i*6f), Quaternion.identity)
                    as GameObject;
            }
        }
    }
}
