using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNexusData", menuName = "Insurance/NexusData")]
public class NexusData : ScriptableObject
{
    [SerializeField]
    private Nexus.Type m_type;
    [SerializeField]
    private float m_growthRate;
    [SerializeField]
    private float m_incubationTime;
    [SerializeField]
    private float m_incubateSpeed = 0;
    [SerializeField]
    private float m_returnSpeed = 2f;
    [SerializeField]
    private float m_dmgNormalization;
    [SerializeField]
    private Color m_color;

    public Nexus.Type Type {
        get { return m_type; }
    }
    public float GrowthRate {
        get { return m_growthRate; }
    }
    public float IncubationTime {
        get { return m_incubationTime; }
    }
    public float IncubateSpeed {
        get { return m_incubateSpeed; }
    }
    public float ReturnSpeed {
        get { return m_returnSpeed; }
    }
    public float DmgNormalization {
        get { return m_dmgNormalization; }
    }
    public Color Color {
        get { return m_color; }
    }
}
