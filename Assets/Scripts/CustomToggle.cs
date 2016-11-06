using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CustomToggle : MonoBehaviour
{
    public ToggleGroupCustom GroupTarget;
    public enum BodyTarget
    {
        head,
        body,
        right_hand,
        left_hand,
        legs
    }

    public BodyTarget ToggleBodyTarget;

    void Awake()
    {
        GroupTarget.Toggles.Add(gameObject.GetComponent<Toggle>());
    }

}
