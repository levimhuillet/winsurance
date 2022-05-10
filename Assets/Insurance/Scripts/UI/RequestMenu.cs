using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RequestMenu : MenuBase
{
    [SerializeField] Button m_requestHelpButton;

    public void Open(UnityAction requestHelpAction) {
        m_requestHelpButton.onClick.AddListener(requestHelpAction);

        base.OpenMenu();
    }

    public void Close() {
        m_requestHelpButton.onClick.RemoveAllListeners();

        base.CloseMenu();
    }
}
