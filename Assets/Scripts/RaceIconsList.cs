using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[CreateAssetMenu(fileName = "RaceIconsList" , menuName = "Race Icon List", order = 2)]
public class RaceIconsList : ScriptableObject
{
    public RaceIconItem[] RaceIcons;
}
