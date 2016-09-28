using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using SimpleJSON;


public class Battle : MonoBehaviour
{

    // Use this for initialization

    public string battle_json;


    private void Start()
    {
        CreateHexMap();
    }

    private void CreateHexMap()
    {
        JsonData data = JsonMapper.ToObject(battle_json);
        Debug.Log(data.Count);

        foreach (var key in data["battle"].Keys)
        {
            Debug.Log(key);
        }
    }
}
