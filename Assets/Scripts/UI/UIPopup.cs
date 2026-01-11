using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIPopup : MonoBehaviour
{
    [SerializeField] protected Button _closeButton;

    protected virtual void Awake()
    {
        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(() =>
            {
                UIManager.Instance.PopupClose();
            });
        }
    }

    protected virtual void OnDestroy()
    {
        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveAllListeners();
        }
    }   

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
}
