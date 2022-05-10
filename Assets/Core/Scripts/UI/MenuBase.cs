﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBase : MonoBehaviour {
    [SerializeField]
    private Button[] m_buttons; // buttons on this menu

    // additional buttons to make non-interactable from parent menus when this menu is opened
    [SerializeField]
    private Button[] m_disableButtons;

    public Button[] MenuButtons {
        get { return m_buttons; }
    }

    protected void EnableMenu() {
        // disable specified buttons when menu is opened
        foreach (Button button in m_disableButtons) {
            button.interactable = false;
        }
    }

    protected void DisableMenu() {
        // remove button handlers
        foreach (Button button in MenuButtons) {
            button.onClick.RemoveAllListeners();
        }

        // re-activate buttons from parent menus disabled when this menu was opened
        foreach (Button button in m_disableButtons) {
            button.interactable = true;
        }
    }

    protected void OpenMenu() {
        EnableMenu();
        this.gameObject.SetActive(true);
    }

    protected void CloseMenu() {
        this.gameObject.SetActive(false);
        DisableMenu();
    }
}
