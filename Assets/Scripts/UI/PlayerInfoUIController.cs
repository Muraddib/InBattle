using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Linq;

public class PlayerInfoUIController : MonoBehaviour
{
    public Text PlayerInfoNameText;
    public Text PlayerInfoLevelText;
    public Text PlayerInfoRatingText;
    public Text PlayerInfoHealthText;
    public Image PlayerRaceImage;
    public Image PlayerAvatarImage;
    public Image PlayerHealthbarImage;
    public RectTransform InfoPanelRoot;
    private bool initialized;

    void Awake()
    {
        GameController.Instance.PlayerInfoUpdated += Instance_PlayerInfoUpdated;
    }

    private void SetPlayerRaceImage()
    {
        PlayerRaceImage.sprite =
            GameController.Instance.RaceIcons.RaceIcons.FirstOrDefault(
                a => a.RaceID.ToString().ToLower() == GameController.Instance.PlayerInfo.race).RaceIcon;
    }

    private void SetPlayerAvatartImage()
    {
        PlayerAvatarImage.sprite =
            GameController.Instance.AvatarImages.PlayerAvatarsList.FirstOrDefault(
                a => a.RaceID.ToString().ToLower() == GameController.Instance.PlayerInfo.race && a.GenderID.ToString().ToLower()==GameController.Instance.PlayerInfo.gender).AvatarCroppedImage;
    }

    private void SetPlayerName()
    {
        PlayerInfoNameText.text = GameController.Instance.PlayerInfo.name;
    }

    private void SetPlayerLevel()
    {
        PlayerInfoLevelText.text = GameController.Instance.PlayerInfo.level.ToString();
    }

    private void SetPlayerRating()
    {
        PlayerInfoRatingText.text = GameController.Instance.PlayerInfo.rating.ToString();
    }

    private void SetPlayerHealth()
    {
        PlayerInfoHealthText.text = GameController.Instance.PlayerInfo.hp + "/" + GameController.Instance.PlayerInfo.max_hp;
        Debug.Log(InfoPanelRoot.sizeDelta.x);
        Debug.Log(GameController.Instance.PlayerInfo.hp);
        Debug.Log(GameController.Instance.PlayerInfo.max_hp);
        PlayerHealthbarImage.rectTransform.offsetMax = new Vector2((InfoPanelRoot.sizeDelta.x-(((float)GameController.Instance.PlayerInfo.hp / (float)GameController.Instance.PlayerInfo.max_hp)*InfoPanelRoot.sizeDelta.x))*-1f, 0f);
    }

    private void Instance_PlayerInfoUpdated(object sender, System.EventArgs e)
    {
        if (!initialized)
        {
            initialized = true;
            SetPlayerRaceImage();
            SetPlayerAvatartImage();
            SetPlayerName();
        }
        SetPlayerLevel();
        SetPlayerRating();
        SetPlayerHealth();
    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.D))
//        {
//            Debug.Log("dec hp 10");
//            GameController.Instance.PlayerInfo.hp -= 10;
//            Instance_PlayerInfoUpdated(this, EventArgs.Empty);
//        }
//        if (Input.GetKeyDown(KeyCode.I))
//        {
//            Debug.Log("inc hp 10");
//            GameController.Instance.PlayerInfo.hp += 10;
//            Instance_PlayerInfoUpdated(this, EventArgs.Empty);
//        }
//    }
}
