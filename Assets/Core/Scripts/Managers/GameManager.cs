using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    private bool m_hasReadInfo;

    public bool IsPaused { get; set; }
    public bool HasReadInfo {
        get { return m_hasReadInfo; }
    }
    public void SetHasReadInfo(bool hasRead) {
        m_hasReadInfo = hasRead;
    }

    public string CurrLevelID {
        get; set;
    }

    #region Unity Callbacks

    private void OnEnable() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (this != instance) {
            Destroy(this.gameObject);
        }

        IsPaused = false;
        m_hasReadInfo = false;
        CurrLevelID = "sample01";
    }

    #endregion

}
