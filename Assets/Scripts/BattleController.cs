﻿using System;
using MiniJSON;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using SimpleJSON;
using System.Security.AccessControl;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
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

    public void Initialize(BattleData data)
    {
        currentBattle = data;
        CreateHexMap();
    }

    private void CreateHexMap()
    {
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
    
    private void Update()
    {
        //if(inTargetSelection) return;
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

    //void OnGUI()
    //{

    //    if (SelectedHex == null) return;

    //        if (GUI.Button(new Rect(Screen.width/2f - 100f, Screen.height - 100f, 100f, 100f), "Walk"))
    //        {
    //            Debug.Log("Walk to:" + "X:" + SelectedHex.X + "Y:" + SelectedHex.Y);
    //        }

    //        if (GUI.Button(new Rect(Screen.width / 2f, Screen.height - 100f, 100f, 100f), "Attack/Block"))
    //        {
    //            Debug.Log("Walk to:" + "X:" + SelectedHex.X + "Y:" + SelectedHex.Y);
    //        }
    //}
}
