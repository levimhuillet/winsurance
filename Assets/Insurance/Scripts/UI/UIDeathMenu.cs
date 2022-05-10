using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIDeathMenu : MenuBase {
    [SerializeField] Button m_returnButton;

    [Serializable]
    public struct Redirection {
        public string LevelID; // the level the redirect applies to
        public string RedirectSceneName; // the scene being redirected to
    }

    [SerializeField] private List<Redirection> m_redirections;

    void OnEnable() {
        m_returnButton.onClick.AddListener(HandleReturnLevelSelect);
    }

    void OnDisable() {
        m_returnButton.onClick.RemoveAllListeners();
    }

    public void Open() {
        base.OpenMenu();
    }

    void HandleReturnLevelSelect() {
        base.CloseMenu();
        AudioManager.instance.StopAudio();
        string destScene = GetRedirect(LevelManager.instance.GetCurrLevelID());
        if (destScene == "LevelSelect") { EventManager.OnReturnLevelSelect.Invoke(); }
        SceneManager.LoadScene(destScene);
    }

    private string GetRedirect(string currLevelID) {
        foreach(Redirection r in m_redirections) {
            if (r.LevelID == currLevelID) {
                return r.RedirectSceneName;
            }
        }

        return "LevelSelect";
    }
}