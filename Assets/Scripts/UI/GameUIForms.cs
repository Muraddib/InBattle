using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class GameUIForms
{
    [Header("Battle UIForms")]
    public UIFormInfo[] BattleUIForms;

    [Header("Battle UI Root")]
    public GameObject BattleUIRoot;
}

