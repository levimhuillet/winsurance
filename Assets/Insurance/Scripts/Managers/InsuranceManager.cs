using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsuranceManager : MonoBehaviour
{
    public static InsuranceManager Instance;

    [SerializeField] private Button m_optionsButton;
    [SerializeField] private UIInsuranceMenu m_optionsMenu;

    [SerializeField]
    private InsuranceSlider m_floodInsuranceSlider;
    [SerializeField]
    private InsuranceSlider m_fireInsuranceSlider;
    [SerializeField]
    private InsuranceSlider m_stormInsuranceSlider;
    [SerializeField]
    private InsuranceSlider m_umbrellaInsuranceSlider;

    private List<UIInsuranceMenu.Coverage> m_availableCoverages;

    // the coverage available in the level
    private Dictionary<UIInsuranceMenu.InsuranceType, UIInsuranceMenu.Coverage> m_availableCoverageTypeMap;

    // the coverage the player chooses to buy
    public struct CoverageDicts
    {
        public Dictionary<UIInsuranceMenu.InsuranceType, UIInsuranceMenu.Coverage> TypeDict;
        public Dictionary<string, UIInsuranceMenu.Coverage> TitleDict;

        public CoverageDicts(Dictionary<UIInsuranceMenu.InsuranceType, UIInsuranceMenu.Coverage> typeDict, Dictionary<string, UIInsuranceMenu.Coverage> titleDict) {
            TypeDict = typeDict;
            TitleDict = titleDict;
        }

        public void ClearDicts() {
            TypeDict.Clear();
            TitleDict.Clear();
        }

        public void AddToDicts(UIInsuranceMenu.Coverage coverage) {
            TypeDict.Add(coverage.Type, coverage);
            TitleDict.Add(coverage.Title, coverage);
        }

        // removes by type
        public void RemoveFromDicts(UIInsuranceMenu.InsuranceType type) {
            UIInsuranceMenu.Coverage toRemove = TypeDict[type];
            TitleDict.Remove(toRemove.Title);
            TypeDict.Remove(toRemove.Type);
        }

        // removes by title
        public void RemoveFromDicts(string title) {
            UIInsuranceMenu.Coverage toRemove = TitleDict[title];
            TypeDict.Remove(toRemove.Type);
            TitleDict.Remove(toRemove.Title);
        }
    }

    private CoverageDicts m_currCoverageDicts;

    private float m_timerTime;
    private InsuranceSlider[] m_allSliders;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }

        m_optionsButton.onClick.AddListener(HandleOptions);

        m_allSliders = new InsuranceSlider[4] {
            m_floodInsuranceSlider,
            m_fireInsuranceSlider,
            m_stormInsuranceSlider,
            m_umbrellaInsuranceSlider
        };

        // Event Handlers
        EventManager.OnPurchaseInsuranceComplete.AddListener(HandlePurchaseInsuranceComplete);

        Dictionary<UIInsuranceMenu.InsuranceType, UIInsuranceMenu.Coverage> currCoverageDictType = new Dictionary<UIInsuranceMenu.InsuranceType, UIInsuranceMenu.Coverage>();
        Dictionary<string, UIInsuranceMenu.Coverage> currCoverageDictTitle = new Dictionary<string, UIInsuranceMenu.Coverage>();
        m_currCoverageDicts = new CoverageDicts(currCoverageDictType, currCoverageDictTitle);
    }

    private void Start() {
        m_timerTime = LevelManager.instance.GetPeriodTime();

    }

    private void Update() {
        if (GameManager.instance.IsPaused) {
            return;
        }

        // tick down insurance timers
        foreach(InsuranceSlider iSlider in m_allSliders) {
            if (m_currCoverageDicts.TypeDict.ContainsKey(iSlider.Type)) {
                iSlider.Timer.Tick();
                if (iSlider.Timer.TimeRemaining <= 0) {
                    if (m_currCoverageDicts.TypeDict[iSlider.Type].AutoRenew) {
                        // try pay coverage
                        if (LevelManager.instance.AttemptPurchase((int)m_currCoverageDicts.TypeDict[iSlider.Type].Premium)) {
                            InsuranceSlider slider = GetSliderByType(iSlider.Type);

                            // start timer
                            slider.Timer.Activate(m_timerTime);

                            InitSliderHelper(slider, iSlider.Type);

                            // reset insurance-level health
                            HealthManager.Instance.SetHealth(iSlider.Type, m_currCoverageDicts.TypeDict[iSlider.Type].MaxCoverage);
                        }
                        else {
                            Debug.Log("not enough funds!");

                            // remove coverage from list
                            m_currCoverageDicts.RemoveFromDicts(iSlider.Type);

                            iSlider.Slider.value = 0;
                        }
                    }
                    else {
                        // remove coverage from list
                        m_currCoverageDicts.RemoveFromDicts(iSlider.Type);

                        iSlider.Slider.value = 0;
                    }
                }
            }
            else if (iSlider.Timer.TimeRemaining != 0) {
                // insurance was in effect, but has since been canceled
                iSlider.Timer.Cancel();

                iSlider.Slider.value = 0;
            }
        }
    }


    public void OpenOptionsMenu() {
        GameManager.instance.IsPaused = true;
        m_optionsMenu.Open();
    }

    public void SetInsuranceSelections(List<UIInsuranceMenu.Coverage> insuranceSelections) {
        // TODO: consolidate this and PayForNewCoverages into one function

        m_currCoverageDicts.ClearDicts();

        foreach (UIInsuranceMenu.Coverage coverage in insuranceSelections) {
            m_currCoverageDicts.AddToDicts(coverage);
            if (GetSliderByType(coverage.Type).CurrCoverageTitle != coverage.Title) {
                GetSliderByType(coverage.Type).SetDirty(true);
            }
            GetSliderByType(coverage.Type).CurrCoverageTitle = coverage.Title;
        }
    }

    public void PayForNewCoverages() {
        foreach (UIInsuranceMenu.InsuranceType key in m_currCoverageDicts.TypeDict.Keys) {
            // check radial timer
            if (GetSliderByType(key).Timer.TimeRemaining == 0 || GetSliderByType(key).IsDirty) {
                // insurance is new
                if (LevelManager.instance.AttemptPurchase((int)m_currCoverageDicts.TypeDict[key].Premium)) {
                    InsuranceSlider slider = GetSliderByType(key);
                    // start timer
                    slider.Timer.Activate(m_timerTime);

                    InitSliderHelper(slider, key);

                    // reset insurance-level health
                    HealthManager.Instance.SetHealth(key, m_currCoverageDicts.TypeDict[key].MaxCoverage);
                }
                else {
                    Debug.Log("not enough funds!");
                }

                GetSliderByType(key).SetDirty(false);
            }
            else {
                // still in effect, so no payment
            }
        }
    }

    public void PayForAllCoverages() {
        foreach (UIInsuranceMenu.InsuranceType key in m_currCoverageDicts.TypeDict.Keys) {
            LevelManager.instance.AttemptPurchase((int)m_currCoverageDicts.TypeDict[key].Premium);
        }
    }

    public float GetInsuranceAmount(UIInsuranceMenu.InsuranceType type) {
        return m_currCoverageDicts.TypeDict.ContainsKey(type) ? m_currCoverageDicts.TypeDict[type].MaxCoverage : 0;
    }

    public Dictionary<UIInsuranceMenu.InsuranceType, UIInsuranceMenu.Coverage> GetInsuranceSelections() {
        return  m_currCoverageDicts.TypeDict;
    }

    public UIInsuranceMenu.Coverage GetCoverageByType(UIInsuranceMenu.InsuranceType type) {
        // initialize the map if it does not exist
        if (Instance.m_availableCoverageTypeMap == null) {
            Instance.m_availableCoverageTypeMap = new Dictionary<UIInsuranceMenu.InsuranceType, UIInsuranceMenu.Coverage>();
            foreach (UIInsuranceMenu.Coverage c in Instance.m_availableCoverages) {
                if (!Instance.m_availableCoverageTypeMap.ContainsKey(c.Type)) {
                    Instance.m_availableCoverageTypeMap.Add(c.Type, c);
                }
            }
        }
        if (Instance.m_availableCoverageTypeMap.ContainsKey(type)) {
            return Instance.m_availableCoverageTypeMap[type];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Coverage " +
                "with type `{0}' is in the level database of available coverages", type
            ));
        }
    }

    public List<UIInsuranceMenu.Coverage> GetAvailableCoverages() {
        // all should start as non-auto-renewing
        for (int i = 0; i < m_availableCoverages.Count; i++) {
            UIInsuranceMenu.Coverage tempCoverage = m_availableCoverages[i];
            tempCoverage.AutoRenew = false;
            m_availableCoverages[i] = tempCoverage;
        }
        return m_availableCoverages;
    }

    public void SetAvailableCoverages(List<UIInsuranceMenu.Coverage> coverages) {
        m_availableCoverages = coverages;
    }

    public void InitSliderVals() {
        // init slider and radial values
        InitSliderHelper(m_floodInsuranceSlider, UIInsuranceMenu.InsuranceType.Flood);
        InitSliderHelper(m_fireInsuranceSlider, UIInsuranceMenu.InsuranceType.Fire);
        InitSliderHelper(m_stormInsuranceSlider, UIInsuranceMenu.InsuranceType.Storm);
        InitSliderHelper(m_umbrellaInsuranceSlider, UIInsuranceMenu.InsuranceType.Umbrella);
    }

    private void InitSliderHelper(InsuranceSlider iSlider, UIInsuranceMenu.InsuranceType type) {
        iSlider.Slider.value = iSlider.Timer.Radial.fillAmount =  m_currCoverageDicts.TypeDict.ContainsKey(type) ? 1 : 0;
    }

    public void ModifyInsuranceHealth(float floodVal, float fireVal, float stormVal, float umbrellaVal) {
        m_floodInsuranceSlider.Slider.value = floodVal;
        m_fireInsuranceSlider.Slider.value = fireVal;
        m_stormInsuranceSlider.Slider.value = stormVal;
        m_umbrellaInsuranceSlider.Slider.value = umbrellaVal;
    }

    private InsuranceSlider GetSliderByType(UIInsuranceMenu.InsuranceType type) {
        switch (type) {
            case UIInsuranceMenu.InsuranceType.Flood:
                return m_floodInsuranceSlider;
            case UIInsuranceMenu.InsuranceType.Fire:
                return m_fireInsuranceSlider;
            case UIInsuranceMenu.InsuranceType.Storm:
                return m_stormInsuranceSlider;
            case UIInsuranceMenu.InsuranceType.Umbrella:
                return m_umbrellaInsuranceSlider;
            default:
                return null;
        }
    }

    public bool CoverageIsActive(UIInsuranceMenu.Coverage coverage) {
        if (m_currCoverageDicts.TitleDict == null) {
            return false;
        }

        return m_currCoverageDicts.TitleDict.ContainsKey(coverage.Title);
    }

    #region Event Handlers

    private void HandleOptions() {
        OpenOptionsMenu();
    }

    void HandlePurchaseInsuranceComplete() {
        // Pay for insurance
        PayForNewCoverages();
    }

    #endregion // Event Handlers
}
