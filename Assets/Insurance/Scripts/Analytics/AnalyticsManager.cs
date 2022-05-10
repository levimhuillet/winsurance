using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WinsuranceAnalytics;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    private string userID;

    private void OnEnable() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this) {
            Destroy(this.gameObject);
        }

        userID = Random.Range(1000, 9999).ToString();
    }

    private void Start() {
        EventManager.OnPurchaseInsuranceComplete.AddListener(HandlePurchaseInsuranceComplete);
        EventManager.OnLevelStart.AddListener(HandleLevelStart);
        EventManager.OnDeath.AddListener(HandleDeath);
        EventManager.OnLevelComplete.AddListener(HandleLevelComplete);
        EventManager.OnLevelQuit.AddListener(HandleLevelQuit);
    }

    private void OnDisable() {
        Debug.Log("closed");
        WinsuranceAnalytics.Close();
    }

    #region Event Handlers

    private void HandlePurchaseInsuranceComplete() {
        WinsuranceAnalytics.ReportEvent(userID,
            "purchase-insurance-complete",
            LevelManager.instance.GetCurrLevelID(),
            HealthManager.Instance.GetBaseHealthPercentage(),
            LevelManager.instance.GetInsuranceTowerRatio(),
            InsuranceManager.Instance.GetOptimality());
    }
    private void HandleLevelStart() {
        WinsuranceAnalytics.ReportEvent(userID,
            "level-start",
            LevelManager.instance.GetCurrLevelID(),
            "n/a",
            "n/a",
            "n/a");
    }

    private void HandleDeath() {
        WinsuranceAnalytics.ReportEvent(userID,
            "level-death",
            LevelManager.instance.GetCurrLevelID(),
            HealthManager.Instance.GetBaseHealthPercentage(),
            LevelManager.instance.GetInsuranceTowerRatio(),
            InsuranceManager.Instance.GetOptimality());
    }

    private void HandleLevelComplete() {
        WinsuranceAnalytics.ReportEvent(userID,
            "level-complete",
            LevelManager.instance.GetCurrLevelID(),
            HealthManager.Instance.GetBaseHealthPercentage(),
            LevelManager.instance.GetInsuranceTowerRatio(),
            InsuranceManager.Instance.GetOptimality());
    }

    private void HandleLevelQuit() {
        WinsuranceAnalytics.ReportEvent(userID,
            "level-quit",
            LevelManager.instance.GetCurrLevelID(),
            HealthManager.Instance.GetBaseHealthPercentage(),
            LevelManager.instance.GetInsuranceTowerRatio(),
            InsuranceManager.Instance.GetOptimality());
    }

    #endregion // Event Handlers
}
