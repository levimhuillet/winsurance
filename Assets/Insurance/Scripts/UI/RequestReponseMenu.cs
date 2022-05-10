using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RequestReponseMenu : MenuBase
{
    [SerializeField] Button m_helpButton, m_refuseButton;

    public void Open(UnityAction helpAction, UnityAction refuseAction) {
        m_helpButton.onClick.AddListener(helpAction);
        m_refuseButton.onClick.AddListener(refuseAction);

        base.OpenMenu();
    }

    public void Close() {
        m_helpButton.onClick.RemoveAllListeners();
        m_refuseButton.onClick.RemoveAllListeners();

        base.CloseMenu();
    }
}
