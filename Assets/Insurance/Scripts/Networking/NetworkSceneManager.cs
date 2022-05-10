
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class NetworkSceneManager : NetworkBehaviour
{
    public static NetworkSceneManager Instance;

    // statically defined because SceneManager can't get index of unloaded scene
    public static int LEVEL_SELECT_BUILD_INDEX = 3;
    public static int LEVEL_BUILD_INDEX = 5;

    [HideInInspector]
    public NetworkVariable<int> CurrScene = new NetworkVariable<int>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (this != Instance) {
            //Destroy(this.gameObject);
            return;
        }

        EventManager.OnReturnLevelSelect.AddListener(delegate { UpdateScene(LEVEL_SELECT_BUILD_INDEX); });
    }

    private void UpdateScene(int sceneIndex) {
        CurrScene.Value = LEVEL_SELECT_BUILD_INDEX;
    }

}
