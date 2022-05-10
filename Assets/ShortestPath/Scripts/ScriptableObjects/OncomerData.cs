using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class OncomerData : ScriptableObject
{
    [SerializeField]
    private Oncomer.Type m_type;
    [SerializeField]
    private Sprite m_sprite;
    [SerializeField]
    private List<TileData.WalkType> m_canWalkOn;
    [SerializeField]
    private float m_speed;
    [SerializeField]
    private float m_dmg;
    [SerializeField]
    private float m_maxHealth;
    [SerializeField]
    private bool m_movesDiagonal = false;

    public Oncomer.Type Type {
        get { return m_type; }
    }
    public Sprite Sprite {
        get { return m_sprite; }
    }
    public List<TileData.WalkType> CanWalkOn {
        get { return m_canWalkOn; }
    }
    public float Speed {
        get { return m_speed; }
    }
    public float Dmg {
        get { return m_dmg; }
    }
    public float MaxHealth {
        get { return m_maxHealth; }
    }
    public bool MovesDiagonal {
        get { return m_movesDiagonal; }
    }
}
