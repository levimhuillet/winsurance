using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Insurance/LevelData")]
public class LevelData : ScriptableObject {

    [SerializeField] private string levelID;
    [SerializeField] private float p_fire;
    [SerializeField] private float p_storm;
    [SerializeField] private float p_flood;
    [SerializeField] private float p_severe;
    [SerializeField] private int numButterflies;
    [SerializeField] private TextAsset gridArrayTA;
    [SerializeField] private int startFunds;
    [SerializeField] private int numPeriods;
    [SerializeField] private int periodFunds;
    [SerializeField] private float periodTime;
    [SerializeField] private float growthPerPeriod;
    [SerializeField] private float startHealth;
    [SerializeField] private List<Vector2> nexusHubsPos;
    [SerializeField] private float presentMultiplier; // how much insurance costs rise if danger is present
    [SerializeField] private float demonstrativeMultiplier; // how much insurance costs rise per demonstrated risk level

    [SerializeField] private Vector2 stationPos;

    [SerializeField] private List<UIInsuranceMenu.Coverage> m_availableCoverages;

    [SerializeField] private Nexus.SevereEffects severeEffects;

    public string ID {
        get { return levelID; }
    }
    public float PFire {
        get { return p_fire; }
    }
    public float PStorm {
        get { return p_storm; }
    }
    public float PFlood {
        get { return p_flood; }
    }
    public float PSevere {
        get { return p_severe; }
    }
    public int NumButterflies {
        get { return numButterflies; }
    }
    public TextAsset GridArrayTA {
        get { return gridArrayTA; }
    }
    public int StartFunds {
        get { return startFunds; }
    }
    public int NumPeriods {
        get { return numPeriods; }
    }
    public int PeriodFunds {
        get { return periodFunds; }
    }
    public float PeriodTime {
        get { return periodTime; }
    }
    public float PeriodGrowth {
        get { return growthPerPeriod; }
    }
    public float StartHealth {
        get { return startHealth; }
    }
    public List<Vector2> NexusHubsPos {
        get { return nexusHubsPos; }
    }
    public Vector2 StationPos {
        get { return stationPos; }
    }
    public List<UIInsuranceMenu.Coverage> AvailableCoverages {
        get { return m_availableCoverages; }
    }
    public Nexus.SevereEffects SevereEffects {
        get { return severeEffects; }
    }
}
