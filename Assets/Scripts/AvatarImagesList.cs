using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[CreateAssetMenu(fileName = "AvatarImagesList" , menuName = "Avatar Image List", order = 1)]
public class AvatarImagesList : ScriptableObject
{
    public PlayerAvatarItem[] PlayerAvatarsList;
}
