using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class NetworkPlayerInstance : NetworkBehaviour
{
    //public NetworkVariable<bool> IsRequesting = new NetworkVariable<bool>();

    [SerializeField] private RequestMenu m_requestMenu;
    [SerializeField] private RequestReponseMenu m_requestResponseMenu;

    [SerializeField] private Button m_waitRoomContinueButton;

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            // TODO: debug why this appears for client when connected to host
            m_waitRoomContinueButton.gameObject.SetActive(true);
            m_waitRoomContinueButton.onClick.AddListener(HandleContinue);
        }

        NetworkSceneManager.Instance.CurrScene.OnValueChanged += new NetworkVariable<int>.OnValueChangedDelegate(HandleSceneChanged);
    }

    private void RecordPlayerArrival() {
        if (NetworkManager.Singleton.IsServer) {
            // removed
        }
        else {
            RecordPlayerArrivalRequestServerRpc();
        }
    }

    [ServerRpc]
    void RecordPlayerArrivalRequestServerRpc(ServerRpcParams rpcParams = default) {
        // removed
    }

    private void HandleContinue() {
        NetworkSceneManager.Instance.CurrScene.Value = NetworkSceneManager.LEVEL_BUILD_INDEX;
    }

    private void HandleSceneChanged(int prevVal, int currVal) {
        Debug.Log("scene changed");
        AudioManager.instance.StopAudio();
        SceneManager.LoadScene(currVal);
    }


    private void Update() {

        /*
        if (NetworkRequestManager.instance.AnyPlayerRequesting.Value) {
            if (!m_requestResponseMenu.gameObject.activeSelf) {
                HandleIncomingRequest();
            }
        }
        else {
            // if menu is still open, close it
            if (m_requestResponseMenu.gameObject.activeSelf) {
                m_requestResponseMenu.Close();
            }
        }
        */
    }

    /*
    #region Own Request Menu

    void ActivateRequestMenu() {
        m_requestMenu.Open(HandleOwnRequestHelp);
    }

    public void HandleOwnRequestHelp() {
        //IsRequesting.Value = true;
        NetworkRequestManager.instance.AnyPlayerRequesting.Value = true;
    }

    void HandleOwnRequestResolved() {
        //IsRequesting.Value = false;
        NetworkRequestManager.instance.AnyPlayerRequesting.Value = false;
    }

    #endregion // Own Request Menu

    #region Incoming Request Response Menu

    void HandleIncomingRequest() {
        m_requestResponseMenu.Open(HandleIncomingHelpButton, HandleIncomingRefuseButton);
    }

    void HandleIncomingHelpButton() {
        m_requestResponseMenu.Close();
        //IsRequesting.Value = false;
        NetworkRequestManager.instance.AnyPlayerRequesting.Value = false;
    }

    void HandleIncomingRefuseButton() {
        m_requestResponseMenu.Close();
        //IsRequesting.Value = false;
        NetworkRequestManager.instance.AnyPlayerRequesting.Value = false;
    }

    #endregion // Incoming Request Reponse Menu

    */
}
