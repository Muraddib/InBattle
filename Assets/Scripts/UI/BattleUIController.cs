using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class BattleUIController : MonoBehaviour
{
    public event Action OnPlayerActionMove;
    public event Action<UIForm> OnPlayerActionAttackBlock;
    public event Action OnPlayerActionBackpackUse;
    public event Action<List<Dictionary<string, object>>> OnFightActionsConfirm;
    public event Action OnHexSelected;

    private RectTransform battleUIRoot;
    private BattleController battleController;
    private List<UIForm> battleUIForms;   

    public GameObject CreateUIForm(UIFormInfo form, RectTransform parent)
    {
        GameObject newForm = Instantiate(form.FormPrefab) as GameObject;
        newForm.GetComponent<RectTransform>().SetParent(parent);
        newForm.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        newForm.GetComponent<RectTransform>().localScale = Vector3.one;
        return newForm;
    }

    public void OnMoveClick()
    {
        Debug.Log("Move Clicked!");

        if (battleController.SelectedHex != null)
        {
            Debug.Log("Moving to hex:");
            Debug.Log(battleController.SelectedHex.X);
            Debug.Log(battleController.SelectedHex.Y);
            if(battleController.IsNeighbourHex(battleController.SelectedHex))
            {
                Dictionary<string, object> movement = new Dictionary<string, object>();
                Dictionary<string, object> dict = new Dictionary<string, object>();
                movement.Add("kind", "move");
                movement.Add("x", battleController.SelectedHex.X);
                movement.Add("y", battleController.SelectedHex.Y);
                dict.Add("battle#action", movement);
                string s = Json.Serialize(dict);
                Debug.Log(s);
                NetworkManager.Instance.Send(s+"\f");
            }
        }
        else
        {
            Debug.Log("No hex selected");
        }
    }

    public void OnAttackBlockClick(UIForm form)
    {
        Debug.Log("AttackBlock Clicked!");
        if (battleController.SelectedHex != null)
        {
            Debug.Log(form.name);
            form.gameObject.SetActive(true);
            form.Show();
        }
        else
        {
            Debug.Log("No hex selected");
        }
    }

    public void OnBackpackClick()
    {
        Debug.Log("Backpack Clicked!");
    }

    public void OnFightActionsConfirmClick(List<Dictionary<string, object>> handActions)
    {
        Debug.Log("FightActionsConfirm Clicked!");

        if (handActions != null)
        {
            foreach (var hand in handActions)
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                hand.Add("id", battleController.SelectedHex.HexEssence.id);
                //hand.Add("kind", "attack");
                //if (hand.ContainsKey("right_type"))
                //{
                //    if ((string) hand["right_type"] == "attack")
                //    {
                //        hand.Add("x", battleController.SelectedHex.X);
                //        hand.Add("y", battleController.SelectedHex.Y);
                //    }
                //}
                //if (hand.ContainsKey("kind"))
                //{
                //    if ((string)hand["kind"] == "attack")
                //    {
                //        hand.Add("x", battleController.SelectedHex.X);
                //        hand.Add("y", battleController.SelectedHex.Y);
                //    }
                //}
                dict.Add("battle#action", hand);
                string s = Json.Serialize(dict);
                Debug.Log(s);
                NetworkManager.Instance.Send(s + "\f");
            }
        }
    }

    private UIForm GetUIFormByID(UIFormIDs id)
    {
        return battleUIForms.Find(a => a.FormID == id);
    }

    private void SubscribeToEvents()
    {
        OnPlayerActionMove += OnMoveClick;
        OnPlayerActionAttackBlock += OnAttackBlockClick;
        OnFightActionsConfirm += OnFightActionsConfirmClick;
    }

    public void Init(UIFormInfo[] UIForms, GameObject rootPrefab)
    {
        battleUIRoot = Instantiate(rootPrefab).GetComponent<RectTransform>();
        battleController = gameObject.GetComponent<BattleController>();

        SubscribeToEvents();

        battleUIForms = new List<UIForm>();
        foreach (var UIForm in UIForms)
        {
            var form = CreateUIForm(UIForm, battleUIRoot).GetComponent<UIForm>();
            switch (form.FormID)
            {
                case UIFormIDs.FormAttackBlock:
                    ((BattleActionsForm)form).Init(
                        onConfirmButtonClick: (data) => { OnFightActionsConfirmClick(data); });
                    break;
                case UIFormIDs.FormBattleActions:
                    ((PlayerActionsForm)form).Init(
                        onMoveButtonClick: () => { OnPlayerActionMove(); },
                        onAttackBlockButtonClick: () => { OnAttackBlockClick(GetUIFormByID(UIFormIDs.FormAttackBlock)); },
                        onBackPackButtonClick: () => { }
                        );
                    break;
            }
            battleUIForms.Add(form);
        }
    }
}
