using UnityEngine;
using System.Collections;

public abstract class BaseWindow : MonoBehaviour
{
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
}
