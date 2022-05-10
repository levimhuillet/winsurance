using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILevelCompleteMenu : MenuBase
{
    [SerializeField] Button m_returnButton;
    [SerializeField] Button m_nextButton;

    void OnEnable() {
        m_returnButton.onClick.AddListener(HandleReturnLevelSelect);
        m_nextButton.onClick.AddListener(HandleNext);
    }

    void OnDisable() {
        m_returnButton.onClick.RemoveAllListeners();
        m_nextButton.onClick.RemoveAllListeners();
    }

    public void Open() {
        base.OpenMenu();
    }

    void HandleReturnLevelSelect() {
        base.CloseMenu();
        AudioManager.instance.StopAudio();
        EventManager.OnReturnLevelSelect.Invoke();
        SceneManager.LoadScene("LevelSelect");
    }

    void HandleNext() {
        base.CloseMenu();
        AudioManager.instance.StopAudio();
        // TODO: determine next level
        SceneManager.LoadScene("LevelSelect");
        EventManager.OnReturnLevelSelect.Invoke();
    }
}
