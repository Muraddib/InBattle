using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class OnlineListItem : MonoBehaviour
{
    public Text PlayerNameText;
    public Image PlayerRaceImage;
    public Button ItemButton;
    private Player player;

    private LayoutElement layoutElement;

    void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
    }

    private void SetElementHeight()
    {
        //transform.root.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
        //layoutElement.minHeight = Screen.height/720f*30f;
    }

    public void Init(Action onConfirmButtonClick, Player playerData)
    {
        player = playerData;
        PlayerNameText.text = player.name;
        PlayerRaceImage.sprite = GameController.Instance.RaceIcons.RaceIcons.FirstOrDefault(
                a => a.RaceID.ToString().ToLower() == playerData.race).RaceIcon;
        ItemButton.onClick.AddListener(() => { Debug.Log(player.name); });
        SetElementHeight();
    }
}
