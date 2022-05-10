using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIInfoScreen : MenuBase
{
    [SerializeField] private Button m_returnButton;

    void Awake() {
        m_returnButton.onClick.AddListener(HandleReturn);
    }

    void OnDestroy() {
        m_returnButton.onClick.RemoveAllListeners();
    }

    public void HandleReturn() {
        GameManager.instance.SetHasReadInfo(true);
        SceneManager.LoadScene("MainMenu");
    }
}
