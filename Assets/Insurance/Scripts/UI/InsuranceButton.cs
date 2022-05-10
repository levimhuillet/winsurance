using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InsuranceButton : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI m_titleText;
    [SerializeField] private TextMeshProUGUI m_premiumText;
    [SerializeField] private TextMeshProUGUI m_deductibleText;
    [SerializeField] private Toggle m_autoRenewToggle;

    public void SetText(string title, string premium, string deductible) {
        m_titleText.text = title;
        m_premiumText.text = "-$" + premium + " premium";
        m_deductibleText.text = "-$" + deductible + " deductible";
    }

    public void SetAutoRenew(bool renew) {
        m_autoRenewToggle.isOn = renew;
    }

    public Toggle RenewToggle {
        get { return m_autoRenewToggle; }
    }
}
