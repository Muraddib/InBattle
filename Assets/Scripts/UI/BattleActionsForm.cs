using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleActionsForm : UIForm
{
    public Button ConfirmButton;
    public ToggleGroupCustom LeftHandAttackGroup;
    public ToggleGroupCustom RightHandAttackGroup;
    public ToggleGroupCustom LeftHandBlockGroup;
    public ToggleGroupCustom RightHandBlockGroup;

    public bool inTargetSelection;

    public void Init(Action<List<Dictionary<string, object>>> onConfirmButtonClick)
    {
        ConfirmButton.onClick.AddListener(() => { onConfirmButtonClick(GetTargets()); });
        gameObject.SetActive(false);
    }

    public void OnCloseClick()
    {
        base.Close();
    }

    public List<Dictionary<string, object>> GetTargets()
    {
        //AttackBlockWindow.SetActive(false);
        //inTargetSelection = false;
        if (LeftHandAttackGroup.ActiveToggles().Count == 0 && LeftHandBlockGroup.ActiveToggles().Count < 2) return null;
        if (RightHandAttackGroup.ActiveToggles().Count == 0 && RightHandBlockGroup.ActiveToggles().Count < 2) return null;

        return GetTargetsData();
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

    private List<Dictionary<string, object>> GetTargetsData()
    {
        Dictionary<string, object> leftHandActions = new Dictionary<string, object>();
        Dictionary<string, object> rightHandActions = new Dictionary<string, object>();

        bool leftIsAttack = LeftHandAttackGroup.ActiveToggles().Count > 0;
        bool rightIsAttack = RightHandAttackGroup.ActiveToggles().Count > 0;

        rightHandActions.Add("kind", rightIsAttack ? "attack" : "block");
        rightHandActions.Add("hand", "right");
        leftHandActions.Add("kind", leftIsAttack ? "attack" : "block");
        leftHandActions.Add("hand", "left");

        if (leftIsAttack)
        {
            leftHandActions.Add("body_part", LeftHandAttackGroup.ActiveToggles().Find(a => a.isOn).gameObject.GetComponent<CustomToggle>().ToggleBodyTarget.ToString());
        }
        else
        {
            var leftBlockGroupToggles = LeftHandBlockGroup.ActiveToggles().FindAll(a => a.isOn);
            leftHandActions.Add("targets", leftBlockGroupToggles[0].GetComponent<CustomToggle>().ToggleBodyTarget.ToString());
            leftHandActions.Add("targets", leftBlockGroupToggles[1].GetComponent<CustomToggle>().ToggleBodyTarget.ToString());
        }

        if (rightIsAttack)
        {
            rightHandActions.Add("body_part", RightHandAttackGroup.ActiveToggles().Find(a => a.isOn).gameObject.GetComponent<CustomToggle>().ToggleBodyTarget.ToString());
        }
        else
        {
            var rightBlockGroupToggles = RightHandBlockGroup.ActiveToggles().FindAll(a => a.isOn);
            rightHandActions.Add("targets", rightBlockGroupToggles[0].GetComponent<CustomToggle>().ToggleBodyTarget.ToString());
            rightHandActions.Add("targets", rightBlockGroupToggles[1].GetComponent<CustomToggle>().ToggleBodyTarget.ToString());
        }

        List<Dictionary<string, object>> handActions = new List<Dictionary<string, object>>();
        handActions.Add(leftHandActions);
        handActions.Add(rightHandActions);
        return handActions;
    }
}
