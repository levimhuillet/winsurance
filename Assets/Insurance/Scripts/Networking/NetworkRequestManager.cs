using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkRequestManager : NetworkBehaviour 
{
    public static NetworkRequestManager instance;
    public NetworkVariable<bool> AnyPlayerRequesting = new NetworkVariable<bool>();

    private void OnEnable() {
        if (instance == null) {
            instance = this;
        }
    }
}
