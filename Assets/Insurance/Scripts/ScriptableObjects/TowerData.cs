using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerData", menuName = "Insurance/TowerData")]
public class TowerData : ScriptableObject
{
    [SerializeField]
    private Tower.Type m_type = Tower.Type.Default;

    [SerializeField]
    private Sprite m_sprite;
    [SerializeField]
    private Oncomer.Type[] m_oncomerTargets;
    [SerializeField]
    private Nexus.Type[] m_nexusTargets;
    [SerializeField]
    private float m_shootSpeed = 1f;
    [SerializeField]
    private float m_radius = 3f;
    // TODO: set projectiles with their own data
    [SerializeField]
    private string m_projectileSoundID = "projectile-default";
    [SerializeField]
    private GameObject m_projectilePrefab;
    [SerializeField]
    private float m_projectileDamage;
    [SerializeField]
    private int m_cost;

    public Tower.Type Type {
        get { return m_type; }
    }
    public Sprite Sprite {
        get { return m_sprite; }
    }
    public Oncomer.Type[] OncomerTargets {
        get { return m_oncomerTargets; }
    }
    public Nexus.Type[] NexusTargets {
        get { return m_nexusTargets; }
    }
    public float ShootSpeed {
        get { return m_shootSpeed; }
    }
    public float Radius {
        get { return m_radius; }
    }
    public string ProjectileSoundID {
        get { return m_projectileSoundID; }
    }
    public GameObject ProjectilePrefab {
        get { return m_projectilePrefab; }
    }
    public float ProjectileDamage {
        get { return m_projectileDamage; }
    }
    public int Cost {
        get { return m_cost; }
    }
}
