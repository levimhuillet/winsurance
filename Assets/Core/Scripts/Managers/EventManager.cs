using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour {
    public static EventManager instance;

    // Define game events below
    public static UnityEvent OnPurchaseInsuranceComplete;
    public static UnityEvent OnLevelStart;
    public static UnityEvent OnDeath;
    public static UnityEvent OnLevelComplete;
    public static UnityEvent OnLevelQuit;
    public static UnityEvent OnReturnLevelSelect;

    private void OnEnable() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (this != instance) {
            Destroy(this.gameObject);
        }

        OnPurchaseInsuranceComplete = new UnityEvent();
        OnLevelStart = new UnityEvent();
        OnDeath = new UnityEvent();
        OnLevelComplete = new UnityEvent();
        OnLevelQuit = new UnityEvent();
        OnReturnLevelSelect = new UnityEvent();

        OnPurchaseInsuranceComplete.AddListener(delegate { LogEvent("OnPurchaseInsuranceComplete"); });
        OnLevelStart.AddListener(delegate { LogEvent("OnLevelStart"); });
        OnDeath.AddListener(delegate { LogEvent("OnDeath"); });
        OnLevelComplete.AddListener(delegate { LogEvent("OnLevelComplete"); });
        OnLevelQuit.AddListener(delegate { LogEvent("OnLevelQuit"); });
        OnReturnLevelSelect.AddListener(delegate { LogEvent("OnReturnLevelSelect"); });
    }

    private void LogEvent(string name) {
        Debug.Log("[Event Manager] Invoked " + name);
    }
}
