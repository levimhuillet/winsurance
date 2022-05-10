
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkObject))]
public class WaitRoomManager : NetworkBehaviour
{
    public static WaitRoomManager Instance;

    public TMP_Text PlayerCountText;

    [HideInInspector]
    public NetworkVariable<int> PlayerCount = new NetworkVariable<int>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (this != Instance) {
            //Destroy(this.gameObject);
        }

        NetworkManager.Singleton.OnClientConnectedCallback += UpdatePlayerCount;

        PlayerCount.OnValueChanged += new NetworkVariable<int>.OnValueChangedDelegate(UpdateCountText);
    }
    private void Start() {
        int waitingRoomIndex = SceneManager.GetSceneByName("WaitingRoom").buildIndex;
        if (NetworkSceneManager.Instance.CurrScene.Value != waitingRoomIndex) {
            NetworkSceneManager.Instance.CurrScene.Value = waitingRoomIndex;
        }
    }

    void OnGUI() {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
            if (SceneManager.GetActiveScene().name != "WaitingRoom") {
                // host aborted session
                AudioManager.instance.StopAudio();
                SceneManager.LoadScene("LevelSelect");
            }
            else {
                StartButtons();
            }
        }
        else {
            StatusLabels();

            //SubmitNewPosition();
        }

        GUILayout.EndArea();
    }

    private void UpdatePlayerCount(ulong clientID) {
        if (IsOwner) {
            WaitRoomManager.Instance.PlayerCount.Value = WaitRoomManager.Instance.PlayerCount.Value + 1;
        }
    }

    private void UpdateCountText(int prevVal, int newVal) {
        WaitRoomManager.Instance.PlayerCountText.text = newVal + "/2 Players";
    }

    static void StartButtons() {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }

    static void StatusLabels() {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }


    static void SubmitNewPosition() {
        /*
        if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change")) {
            if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient) {
                foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                    NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<HelloWorldPlayer>().Move();
            }
            else {
                var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                var player = playerObject.GetComponent<HelloWorldPlayer>();
                player.Move();
            }
        }
        */
    }
}
