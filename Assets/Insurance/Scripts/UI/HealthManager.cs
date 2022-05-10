using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
    public static HealthManager Instance;

    [SerializeField] bool DEBUGGING = false;

    [SerializeField]
    private Slider m_baseSlider;

    #region Health Grouping

    struct HealthGroup {
        public float Base;
        public float Flood;
        public float Fire;
        public float Storm;
        public float Umbrella;
    }
    struct AllHealth {
        public HealthGroup Curr;
        public HealthGroup Total;
    }

    private AllHealth m_allHealth;

    #endregion // Health Grouping

    private float m_totalWidth;
    private float m_baseLeftAnchorOffset;

    private Dictionary<UIInsuranceMenu.InsuranceType, UIInsuranceMenu.Coverage> m_currCoveragesDict;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }
    }

    public void InitFields(float startBaseHealth, float startFloodHealth, float startFireHealth, float startStormHealth, float startUmbrellaHealth) {
        m_allHealth = new AllHealth();

        // init health
        m_allHealth.Curr.Base = m_allHealth.Total.Base = startBaseHealth;
        m_allHealth.Curr.Flood = m_allHealth.Total.Flood = startFloodHealth;
        m_allHealth.Curr.Fire = m_allHealth.Total.Fire = startFireHealth;
        m_allHealth.Curr.Storm = m_allHealth.Total.Storm = startStormHealth;
        m_allHealth.Curr.Umbrella = m_allHealth.Total.Umbrella = startUmbrellaHealth;

        // TODO: check if still needed
        InsuranceManager.Instance.InitSliderVals();

        m_baseSlider.value = m_allHealth.Curr.Base > 0 ? 1 : 0;
    }

    public void ModifyHealth(float change, Oncomer.Type type) {
        if (change < 0) {
            DamageHealth(-change, type);
        }

        // Update sliders
        m_baseSlider.value = m_allHealth.Curr.Base / m_allHealth.Total.Base;

        float floodVal = m_allHealth.Curr.Flood / m_allHealth.Total.Flood;
        float fireVal = m_allHealth.Curr.Fire / m_allHealth.Total.Fire;
        float stormVal = m_allHealth.Curr.Storm / m_allHealth.Total.Storm;
        float umbrellaVal = m_allHealth.Curr.Umbrella / m_allHealth.Total.Umbrella;
        InsuranceManager.Instance.ModifyInsuranceHealth(floodVal, fireVal, stormVal, umbrellaVal);
    }

    public void SetHealth(UIInsuranceMenu.InsuranceType type, float maxCoverage) {
        switch (type) {
            case UIInsuranceMenu.InsuranceType.Flood:
                m_allHealth.Curr.Flood = m_allHealth.Total.Flood = maxCoverage;
                break;
            case UIInsuranceMenu.InsuranceType.Fire:
                m_allHealth.Curr.Fire = m_allHealth.Total.Fire = maxCoverage;
                break;
            case UIInsuranceMenu.InsuranceType.Storm:
                m_allHealth.Curr.Storm = m_allHealth.Total.Storm = maxCoverage;
                break;
            case UIInsuranceMenu.InsuranceType.Umbrella:
                m_allHealth.Curr.Umbrella = m_allHealth.Total.Umbrella = maxCoverage;
                break;
            default:
                break;
        }
    }

    #region Helper Methods

    private void DamageHealth(float dmg, Oncomer.Type type) {
        m_currCoveragesDict = InsuranceManager.Instance.GetInsuranceSelections();

        // divvy up damage according to insurance
        switch (type) {
            case Oncomer.Type.Flood:
                CoverageSequence(UIInsuranceMenu.InsuranceType.Flood, dmg);
                break;
            case Oncomer.Type.Tempest:
                CoverageSequence(UIInsuranceMenu.InsuranceType.Storm, dmg);
                break;
            case Oncomer.Type.Wildfire:
                CoverageSequence(UIInsuranceMenu.InsuranceType.Fire, dmg);
                break;
            case Oncomer.Type.None:
                //TODO: test this case?
                CoverageSequence(UIInsuranceMenu.InsuranceType.None, dmg);
                break;
            default:
                break;
        }
    }

    private void CoverageSequence(UIInsuranceMenu.InsuranceType insuranceType, float dmg) {
        if (DEBUGGING) { Debug.Log("Starting coverage sequence for " + insuranceType + " of damage " + dmg); }
        if (DEBUGGING) { Debug.Log("Checking for relevant insurance..."); }

        float fullDmg = dmg;
        if (insuranceType != UIInsuranceMenu.InsuranceType.Umbrella && m_currCoveragesDict.ContainsKey(insuranceType)) {
            if (DEBUGGING) { Debug.Log("Has relevant coverage: " + insuranceType); }
            UIInsuranceMenu.Coverage relevantCoverage = m_currCoveragesDict[insuranceType];

            if (DEBUGGING) { Debug.Log("Applying deductible of value " + relevantCoverage.Deductible); }
            // apply deductible to base health
            float deductibleAmt = Mathf.Min(relevantCoverage.Deductible, dmg);
            dmg -= deductibleAmt;
            dmg += TryCoverDamage(deductibleAmt, UIInsuranceMenu.InsuranceType.None);
            if (DEBUGGING) { Debug.Log("Remaining damage: " + dmg); }
            if (dmg <= 0) { return; }

            // apply as much as possible to relevant insurance
            if (DEBUGGING) { Debug.Log("applying remaining " + dmg + " dmg to insurance of type " + insuranceType); }
            fullDmg = dmg;
            dmg = 0;
            dmg += TryCoverDamage(fullDmg, insuranceType);
            if (DEBUGGING) { Debug.Log("Remaining damage: " + dmg); }
            if (dmg <= 0) { return; }

            // UMBRELLA insurance

            if (DEBUGGING) { Debug.Log("Checking for umbrella insurance..."); }

            if (m_currCoveragesDict.ContainsKey(UIInsuranceMenu.InsuranceType.Umbrella)) {
                if (DEBUGGING) { Debug.Log("Has umbrella insurance"); }
                UIInsuranceMenu.Coverage umbrellaCoverage = m_currCoveragesDict[UIInsuranceMenu.InsuranceType.Umbrella];

                // apply umbrella deductible to base health
                if (DEBUGGING) { Debug.Log("Applying deductible of value " + umbrellaCoverage.Deductible); }
                deductibleAmt = Mathf.Min(umbrellaCoverage.Deductible, dmg);
                dmg -= deductibleAmt;
                dmg += TryCoverDamage(deductibleAmt, UIInsuranceMenu.InsuranceType.None);
                if (DEBUGGING) { Debug.Log("Remaining damage: " + dmg); }
                if (dmg <= 0) { return; }

                // apply rest to umbrella insurance
                if (DEBUGGING) { Debug.Log("applying remaining " + dmg + " dmg to insurance of type Umbrella"); }
                fullDmg = dmg;
                dmg = 0;
                dmg += TryCoverDamage(fullDmg, UIInsuranceMenu.InsuranceType.Umbrella);
                if (DEBUGGING) { Debug.Log("Remaining damage: " + dmg); }
                if (dmg <= 0) { return; }
            }
        }

        // apply remainder to base health
        if (DEBUGGING) { Debug.Log("applying remaining " + dmg + " dmg to insurance of type None (directly to base"); }
        fullDmg = dmg;
        dmg = 0;
        dmg += TryCoverDamage(fullDmg, UIInsuranceMenu.InsuranceType.None);
        if (DEBUGGING) { Debug.Log("Remaining damage: " + dmg); }

        if (DEBUGGING) { Debug.Log("End sequence-------------------"); }

        return;
    }

    private float TryCoverDamage(float dmg, UIInsuranceMenu.InsuranceType insuranceType) {
        float remainder = 0;
        switch (insuranceType) {
            case UIInsuranceMenu.InsuranceType.Fire:
                m_allHealth.Curr.Fire -= dmg;
                if (m_allHealth.Curr.Fire < 0) {
                    // store remainder
                    remainder = -m_allHealth.Curr.Fire;
                    // set insurance health to 0
                    m_allHealth.Curr.Fire = 0;
                }
                break;
            case UIInsuranceMenu.InsuranceType.Flood:
                m_allHealth.Curr.Flood -= dmg;
                if (m_allHealth.Curr.Flood < 0) {
                    // store remainder
                    remainder = -m_allHealth.Curr.Flood;
                    // set insurance health to 0
                    m_allHealth.Curr.Flood = 0;
                }
                break;
            case UIInsuranceMenu.InsuranceType.Storm:
                m_allHealth.Curr.Storm -= dmg;
                if (m_allHealth.Curr.Storm < 0) {
                    // store remainder
                    remainder = -m_allHealth.Curr.Storm;
                    // set insurance health to 0
                    m_allHealth.Curr.Storm = 0;
                }
                break;
            case UIInsuranceMenu.InsuranceType.Umbrella:
                m_allHealth.Curr.Umbrella -= dmg;
                if (m_allHealth.Curr.Umbrella < 0) {
                    // store remainder
                    remainder = -m_allHealth.Curr.Umbrella;
                    // set insurance health to 0
                    m_allHealth.Curr.Umbrella = 0;
                }
                break;
            case UIInsuranceMenu.InsuranceType.None:
                m_allHealth.Curr.Base -= dmg;
                if (m_allHealth.Curr.Base <= 0) {
                    // set base health to 0
                    m_allHealth.Curr.Base = 0;

                    // no coverage left means death.
                    EventManager.OnDeath.Invoke();

                    // no remainder because level is lost
                }
                break;
            default:
                break;
        }

        return remainder;
    }

    #endregion // Helper Methods
}
