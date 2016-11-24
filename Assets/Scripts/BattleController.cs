using System;
using MiniJSON;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using SimpleJSON;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Text;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public event EventHandler OnBattleEnd = (sender, e) => { };

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
    private GameObject HexPlayer;
    public GameObject HexPlayerPrefab;

    public GameObject SmallObstaclePrefab;
    public GameObject SmallObstructionPrefab;

    public List<Battler> BattleCharacters;

    public Vector2 ActionsFrameSize;
    public bool debugJSON;

    public class HexClickedEventArgs : EventArgs
    {
    }

    public void UpdateButtle(BattleData battle)
    {
        switch (battle.battle.status)
        {
            case "battle":
                break;
            case "closed":
                if(OnBattleEnd!=null) OnBattleEnd(this, new EventArgs());
                break;
        }
        currentBattle = battle;
        UpdateBattlersInfo();
        UpdateBattlersPositions();
        UpdatePlayerHex();
        UpdateHexEssences();
    }

    public bool IsNeighbourHex(Hex hex)
    {
        var playerEssense = currentBattle.battle.essences.Find(a => a.info.id == GameController.Instance.PlayerCharacter.id);

        if (hex.Y == playerEssense.gex.y)
        {
            if (hex.X == playerEssense.gex.x - 1 || hex.X == playerEssense.gex.x + 1)
                return true;
        }
        else
        {
            if (hex.Y == playerEssense.gex.y - 1 || hex.Y == playerEssense.gex.y + 1)
            {
                if (hex.X == playerEssense.gex.x - 1 || hex.X == playerEssense.gex.x)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Initialize(BattleData data)
    {
        currentBattle = data;
        HexPlayer = Instantiate(HexPlayerPrefab);
        CreateHexMap();
    }

    private void UpdateBattlersInfo()
    {
        foreach (var battler in BattleCharacters)
        {
            battler.EssenceInfo = currentBattle.battle.essences.Find(a => a.info.id == battler.EssenceInfo.info.id);
        }
    }

    private void UpdateBattlersPositions()
    {
        foreach (var battler in BattleCharacters)
        {
            var gex = Hexes.Find(a => a.X == battler.EssenceInfo.gex.x && a.Y == battler.EssenceInfo.gex.y).gameObject;
            battler.gameObject.transform.position = gex.gameObject.transform.position + Vector3.up;
        }
    }

    private void UpdateHexEssences()
    {
        Hexes.ForEach(a=>a.HexEssence = null);
        foreach (var essence in currentBattle.battle.essences)
        {
            var gex = Hexes.Find(a => a.X == essence.gex.x && a.Y == essence.gex.y).gameObject;
            if (gex)
            {
                gex.GetComponent<Hex>().HexEssence = essence;
            }
        }
    }

    private void CreateHexMap()
    {
        GameObject fieldRoot = new GameObject("Battlefield root");
        fieldRoot.transform.position = Vector3.zero;

        int countX = currentBattle.battle.width;
        int countY = currentBattle.battle.height;
        Hexes = new List<Hex>();
        
        for (int i = 0; i < countY; i++)
        {
            for (int j = 0; j < countX; j++)
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

        BattleCharacters = new List<Battler>();

        foreach (var essence in currentBattle.battle.essences)
        {
            var gex = Hexes.Find(a => a.X == essence.gex.x && a.Y == essence.gex.y).gameObject;
            if (gex)
            {
                gex.GetComponent<Hex>().HexEssence = essence;
            }

            if (essence.kind == "hollow")
            {
                if (gex)
                {
                    gex.GetComponent<Hex>().Visible = false;
                    gex.GetComponent<MeshRenderer>().enabled = false;
                    gex.GetComponent<Collider>().enabled = false;
                }
            }

            if (essence.kind == "obstacle")
            {
                if (gex)
                {
                    var obstacle = Instantiate(SmallObstaclePrefab, gex.gameObject.transform.position + Vector3.up,
                                               Quaternion.Euler(90f, 0f, 0f)) as GameObject;
                }
            }

            if (essence.kind == "obstruction")
            {
                if (gex)
                {
                    var obstacle = Instantiate(SmallObstructionPrefab, gex.gameObject.transform.position + Vector3.up,
                                               Quaternion.Euler(90f, 0f, 0f)) as GameObject;
                }
            }

            if (essence.kind == "battler")
            {
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
                    var battlerInfo = battler.AddComponent<Battler>();
                    battlerInfo.EssenceInfo = essence;
                    BattleCharacters.Add(battlerInfo);
                }
            }
        }
        UpdatePlayerHex();
    }

    private void UpdatePlayerHex()
    {
        var playerEssense =
            currentBattle.battle.essences.Find(a => a.info.id == GameController.Instance.PlayerCharacter.id);
        HexPlayer.transform.position =
            Hexes.Find(a => a.X == playerEssense.gex.x && a.Y == playerEssense.gex.y).transform.position +
            new Vector3(0f, 0.1f, 0.1f);
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (SelectedHex != null)
            {
                Debug.Log(IsNeighbourHex(SelectedHex));
            }
        }

    }
}
