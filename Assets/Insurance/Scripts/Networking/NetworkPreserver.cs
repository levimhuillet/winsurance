using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class NetworkPreserver : NetworkBehaviour
{
    public static NetworkPreserver instance;
    public NetworkVariable<int> CurrScene = new NetworkVariable<int>();
    [SerializeField] private List<GameObject> m_toPreserve;

    private UnityEvent OnReleaseAll;

    private void OnEnable() {
        if (instance == null) {
            instance = this;
        }
        else if (this != instance) {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        foreach (GameObject obj in m_toPreserve) {
            DontDestroyOnLoad(obj);
        }

        OnReleaseAll = new UnityEvent();

        OnReleaseAll.AddListener(HandleReleaseAll);

        SceneManager.sceneLoaded += checkOutOfScope;
    }

    private void checkOutOfScope(Scene scene, LoadSceneMode mode) {
        if (SceneManager.GetActiveScene().name == "LevelSelect") {
            OnReleaseAll.Invoke();
        }
    }

    private void HandleReleaseAll() {
        // Release the preserved
        foreach (GameObject obj in m_toPreserve) {
            Destroy(obj);
        }

        Debug.Log("all objects destroyed");
        Destroy(this.gameObject);
    }
}
