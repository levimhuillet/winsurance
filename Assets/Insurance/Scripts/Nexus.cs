using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class Nexus : MonoBehaviour
{
    public enum Type {
        Storm,
        FireSwathe,
        Deluvian
    }

    private enum State {
        Incubating,
        Returning,
        Releasing,
        Dissolving
    }

    [SerializeField]
    private Type m_type;
    [SerializeField]
    private float m_scaleReduction;
    [SerializeField]
    private GameObject m_oncomerPrefab;
    [SerializeField]
    private float m_dmgNormalization;

    private bool m_isSevere;

    [Serializable]
    public struct SevereEffects {
        public float BaseSize;
        public float GrowthMult;
        public float HealthBonusMult;
        public float DamageBonusMult;
    }

    private SevereEffects m_severeEffects;

    private float m_baseGrowthRate;
    private float m_incubationTime;
    private float m_incubateSpeed;
    private float m_returnSpeed;

    private float m_size;
    private float m_incubationTimer;
    private float m_adjustedGrowthRate;

    private State m_state;
    private Vector3 m_returnPos;

    private SpriteRenderer m_sr;

    private void Update() {
        if (GameManager.instance.IsPaused) {
            return;
        }

        if (m_state == State.Incubating) {
            m_incubationTimer -= Time.deltaTime;
            m_size += Time.deltaTime * m_adjustedGrowthRate;
            this.transform.localScale = new Vector3((m_size + 1) / m_scaleReduction, (m_size + 1) / m_scaleReduction, 1);

            if (m_incubationTimer <= 0) {
                Return();
            }
        }
        else if (m_state == State.Returning) {
            Vector3 travelVector = (m_returnPos - this.transform.position);
            
            if (travelVector.magnitude <= .05f) {
                this.transform.Translate(travelVector);
                Release();
            }
            else {
                Vector3 dir = travelVector.normalized;
                this.transform.Translate(dir * m_returnSpeed * Time.deltaTime);
            }
            this.transform.localScale = new Vector3((m_size + 1) / m_scaleReduction, (m_size + 1) / m_scaleReduction, 1);
        }
    }

    public void SetType(Nexus.Type type, bool severe) {
        m_type = type;
        m_isSevere = severe;
    }

    public void ManualAwake() {
        m_sr = this.GetComponent<SpriteRenderer>();

        ApplyNexusData();

        m_incubationTimer = m_incubationTime;

        m_size = 1;

        m_state = State.Incubating;

        if (m_isSevere) {
            SevereEffects effects = LevelManager.instance.GetSevereEffects();
            ApplySevereEffects(effects);
        }
    }

    public void MultGrowth(float multGrowth) {
        m_adjustedGrowthRate = m_baseGrowthRate * multGrowth;
    }

    private void ApplyNexusData() {
        NexusData data = GameDB.instance.GetNexusData(m_type);
        m_baseGrowthRate = data.GrowthRate;
        m_incubationTime = data.IncubationTime;
        m_returnSpeed = data.ReturnSpeed;
        m_incubateSpeed = data.IncubateSpeed;
        m_sr.color = data.Color;
        m_dmgNormalization = data.DmgNormalization;
    }

    private void ApplySevereEffects(SevereEffects effects) {
        m_size += effects.BaseSize;
        m_baseGrowthRate *= effects.GrowthMult;
        m_severeEffects = effects;
    }

    private void Return() {
        m_state = State.Returning;

        // move toward nexusHub
        m_returnPos = TilemapManager.instance.GetNexusHubTransform(m_type).position;
    }

    private void Release() {
        int numSpawns = (int)m_size + 1;

        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        float leftBound = -sr.bounds.extents.x;
        float width = sr.bounds.extents.x * 2;
        float step = width / numSpawns;

        for (int i = 0; i < numSpawns; ++i) {

            GameObject oncomerObj = Instantiate(m_oncomerPrefab);
            oncomerObj.SetActive(false);
            Vector3 spawnSpread = new Vector3(leftBound + step * i, 0, 0);
            oncomerObj.transform.position = this.transform.position + spawnSpread;
            Oncomer oncomer = oncomerObj.GetComponent<Oncomer>();

            switch (m_type) {

                case Nexus.Type.Storm:
                    oncomer.OncomerData = GameDB.instance.GetOncomerData(Oncomer.Type.Tempest);
                    break;
                case Nexus.Type.Deluvian:
                    oncomer.OncomerData = GameDB.instance.GetOncomerData(Oncomer.Type.Flood);
                    break;
                case Nexus.Type.FireSwathe:
                    oncomer.OncomerData = GameDB.instance.GetOncomerData(Oncomer.Type.Wildfire);
                    break;
                default:
                    Debug.Log("Unknown type of nexus. Unable to spawn oncomer.");
                    Destroy(oncomerObj);
                    break;
            }

            if (m_isSevere) {
                oncomer.ManualAwake(m_severeEffects);
            }
            else {
                oncomer.ManualAwake();
            }
        }

        Destroy(this.gameObject);
    }

    public Type GetNexusType() {
        return m_type;
    }

    public void ApplyDamage(float damage) {
        m_size -= damage / m_dmgNormalization;

        if (m_size < -1) {
            Destroy(this.gameObject);
        }
    }
}
