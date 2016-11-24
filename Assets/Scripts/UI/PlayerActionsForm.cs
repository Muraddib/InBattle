using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerActionsForm : UIForm
{
    public Button MoveButton;
    public Button AttackBlockButton;
    public Button BackPackButton;

    public void Init(Action onMoveButtonClick, Action onAttackBlockButtonClick, Action onBackPackButtonClick)
    {
        MoveButton.onClick.AddListener(new UnityAction(onMoveButtonClick));
        AttackBlockButton.onClick.AddListener(new UnityAction(onAttackBlockButtonClick));
        BackPackButton.onClick.AddListener(new UnityAction(onBackPackButtonClick));
    }

}
