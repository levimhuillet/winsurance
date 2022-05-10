using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsuranceTimer : MonoBehaviour
{
    [SerializeField] private Image m_radial;
    private float m_startTime;
    private float m_timeRemaining;

    public Image Radial {
        get { return m_radial; }
    }

    public float TimeRemaining {
        get { return m_timeRemaining; }
    }

    public void Cancel() {
        m_timeRemaining = m_radial.fillAmount = 0;
    }

    public void Activate(float time) {
        m_timeRemaining = m_startTime = time;
    }

    public void Tick() {
        m_timeRemaining -= Time.deltaTime;
        if (m_timeRemaining < 0) {
            m_timeRemaining = 0;
        }

        m_radial.fillAmount = m_timeRemaining / m_startTime;
    }
}
