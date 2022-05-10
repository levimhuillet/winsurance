using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsuranceSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private InsuranceTimer timer;
    [SerializeField] private UIInsuranceMenu.InsuranceType m_type;

    private bool isDirty;


    public Slider Slider {
        get { return slider; }
    }
    public InsuranceTimer Timer {
        get { return timer; }
    }
    public UIInsuranceMenu.InsuranceType Type {
        get { return m_type; }
    }
    public string CurrCoverageTitle {
        get; set;
    }

    public void SetDirty(bool dirty) {
        isDirty = dirty;
    }

    public bool IsDirty {
        get { return isDirty; }
    }
}
