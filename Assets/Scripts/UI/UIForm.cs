using UnityEngine;
using System.Collections;

public abstract class UIForm : MonoBehaviour
{
    public UIFormIDs FormID;

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
}
