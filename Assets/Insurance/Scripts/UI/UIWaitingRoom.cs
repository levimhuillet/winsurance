using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;

public class UIWaitingRoom : MonoBehaviour
{
    [SerializeField] private Button m_continueButton;

    private void OnEnable() {
        m_continueButton.onClick.AddListener(HandleContinue);
    }

    private void OnDisable() {
        m_continueButton.onClick.RemoveAllListeners();
    }

    private void Update() {
        m_continueButton.interactable = (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer);
    }

    private void HandleContinue() {
        GameManager.instance.IsPaused = false;
        SceneManager.LoadScene("LevelHolder");
    }
}
