using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ToggleGroupCustom : MonoBehaviour {

    public enum HandType
    {
        LeftHandAttack,
        RightHandAttack,
        LeftHandBlock,
        RightHandBlock
    }

    public HandType GroupHandType;
    public int MaxActive;
    public List<Toggle> Toggles = new List<Toggle>();

    public List<Toggle> ActiveToggles()
    {
        return Toggles.FindAll(a => a.isOn);
    }

    public void SetTogglesInactive()
    {
        Toggles.ForEach(a=>a.isOn = false);
    }

    public void SetToggleActive(Toggle toggle)
    {
        if (ActiveToggles().Count > MaxActive)
        {
            for (int i = 0; i < Toggles.Count; i++)
            {
                if (Toggles[i] != toggle)
                {
                    Toggles[i].isOn = false;
                }
                if (ActiveToggles().Count == MaxActive) break;
            }
        }
    }
}
