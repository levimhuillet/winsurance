using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIBummerMenu : MenuBase
{
    [SerializeField] Button m_closeButton;

    void OnEnable() {
        m_closeButton.onClick.AddListener(HandleReturnLevelSelect);
    }

    void OnDisable() {
        m_closeButton.onClick.RemoveAllListeners();
    }

    void HandleReturnLevelSelect() {
        EventManager.OnReturnLevelSelect.Invoke();
        SceneManager.LoadScene("LevelSelect");
    }
}
