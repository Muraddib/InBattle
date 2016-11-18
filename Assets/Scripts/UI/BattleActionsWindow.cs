using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine.UI;

public class BattleActionsWindow : BaseWindow {

    public ToggleGroupCustom LeftHandAttackGroup;
    public ToggleGroupCustom RightHandAttackGroup;
    public ToggleGroupCustom LeftHandBlockGroup;
    public ToggleGroupCustom RightHandBlockGroup;
    public bool inTargetSelection;

    public void OnCloseClick()
    {
        base.Close();
    }

    public void OnConfirmClick()
    {
        //AttackBlockWindow.SetActive(false);
        //inTargetSelection = false;
        if (LeftHandAttackGroup.ActiveToggles().Count == 0 && LeftHandBlockGroup.ActiveToggles().Count < 2) return;
        if (RightHandAttackGroup.ActiveToggles().Count == 0 && RightHandBlockGroup.ActiveToggles().Count < 2) return;

        GetTargetsData();
    }

    public void OnToggleClick(GameObject go)
    {
        ToggleGroupCustom group = go.GetComponent<CustomToggle>().GroupTarget;
        group.SetToggleActive(go.GetComponent<Toggle>());
        switch (group.GroupHandType)
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

    private void GetTargetsData()
    {
        Dictionary<string, string> handActions = new Dictionary<string, string>();

        bool leftIsAttack = LeftHandAttackGroup.ActiveToggles().Count > 0;
        bool rightIsAttack = RightHandAttackGroup.ActiveToggles().Count > 0;

        handActions.Add("right_type", rightIsAttack ? "attack" : "block");
        handActions.Add("left_type", leftIsAttack ? "attack" : "block");

        if (leftIsAttack)
        {
            handActions.Add("left_first", LeftHandAttackGroup.ActiveToggles().Find(a => a.isOn).gameObject.GetComponent<CustomToggle>().ToggleBodyTarget.ToString());
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

}
