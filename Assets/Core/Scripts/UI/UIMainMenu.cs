using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MenuBase {
    #region Editor

    [SerializeField]
    private Button m_newGameButton;
    [SerializeField]
    private Button m_quitButton;
    [SerializeField]
    private Button m_aboutButton;

    #endregion

    #region Unity Callbacks

    private void Awake() {
        m_quitButton.onClick.AddListener(HandleQuit);
        m_aboutButton.onClick.AddListener(HandleAbout);

        //m_newGameButton.interactable = GameManager.instance.HasReadInfo;
        m_newGameButton.onClick.AddListener(HandleNewGame);
    }

    private void OnDestroy() {
        m_newGameButton.onClick.RemoveAllListeners();
        m_quitButton.onClick.RemoveAllListeners();
        m_aboutButton.onClick.RemoveAllListeners();
    }

    #endregion

    #region ButtonHandlers

    private void HandleNewGame() {
        //SceneManager.LoadScene("SampleScene");
        //SceneManager.LoadScene("InsuranceSample");
        SceneManager.LoadScene("LevelSelect"); // change to whichever scene is your next
        AudioManager.instance.PlayOneShot("menu-click-default");
    }

    private void HandleQuit() {
        Application.Quit();
        AudioManager.instance.PlayOneShot("menu-click-default");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void HandleAbout() {
        SceneManager.LoadScene("InfoScreen");
        AudioManager.instance.PlayOneShot("menu-click-default");
    }

    #endregion
}
