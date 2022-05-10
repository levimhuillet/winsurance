using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Serializable]
    public struct SevereWeatherTrigger {
        public string LevelID;
        public int Period;
        public float Time;
        public Nexus.Type Type;
        public Vector2 Pos;

        public SevereWeatherTrigger(string id, int period, float time, Nexus.Type type, Vector2 pos) {
            LevelID = id;
            Period = period;
            Time = time;
            Type = type;
            Pos = pos;
        }
    }

    [SerializeField] private List<SevereWeatherTrigger> m_manualWeatherTriggers;

    private List<SevereWeatherTrigger> m_activeTriggers;

    [SerializeField] private GameObject m_nexusPrefab;

    private void Start() {
        // parse out relevent triggers to this level
        m_activeTriggers = m_manualWeatherTriggers.FindAll(t => t.LevelID == LevelManager.instance.GetCurrLevelID());
    }

    private void Update() {
        List<SevereWeatherTrigger> toRemove = new List<SevereWeatherTrigger>();

        foreach (SevereWeatherTrigger trigger in m_activeTriggers) {
            if (LevelManager.instance.TriggerConditionsMet(trigger)) {
                InvokeTrigger(trigger);
                toRemove.Add(trigger);
            }
        }

        foreach(SevereWeatherTrigger trigger in toRemove) {
            m_activeTriggers.Remove(trigger);
        }
    }

    private void InvokeTrigger(SevereWeatherTrigger trigger) {
        InstantiateSevereNexus(trigger.Type, trigger.Pos);
    }

    private void InstantiateSevereNexus(Nexus.Type nexusType, Vector2 pos) {
        GameObject nexusObj = Instantiate(m_nexusPrefab);
        nexusObj.transform.position = pos;
        Nexus nexus = nexusObj.GetComponent<Nexus>();

        nexus.SetType(nexusType, true);
        nexus.ManualAwake();
        nexus.MultGrowth(1);
    }
}
