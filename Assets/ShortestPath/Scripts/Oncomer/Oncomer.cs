using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Oncomer : MonoBehaviour {
    public enum Type {
        // Insurance
        Flood,
        Tempest,
        Wildfire,
        None,

        // Framework
        Spider,
        Salamander
    }

    [SerializeField]
    private bool m_debugPath = false;
    [SerializeField]
    private GameObject m_debugPrefab;
    [SerializeField]
    private GameObject m_debugHolder;
    [SerializeField]
    private Type m_type; // only serialized for manual spawning

    private float m_maxHealth; // or equivalent measurement of Oncomer trait that is modified by towers
    private float m_currHealth; // or equivalent measurement of Oncomer trait that is modified by towers
    private List<TileData.WalkType> m_canWalkOn;
    private List<Vector2> m_waypoints;
    private float m_speed;
    private float m_dmg;
    private bool m_movesDiagonal;
    private int m_currWaypointIndex;

    public OncomerData OncomerData {
        get; set;
    }

    private static float WAYPOINT_BUFFER = 0.05f;

    private void Awake() {
        if (m_type == Type.Spider || m_type == Type.Salamander) {
            // Framework Case
            this.OncomerData = GameDB.instance.GetOncomerData(m_type);

            ApplyOncomerData();

            CalculatePath();
        }
    }

    // Used by Nexus when instantiating
    public void ManualAwake() {
        ApplyOncomerData();

        CalculatePath();

        LevelManager.instance.RegisterOncomer();
    }

    public void ManualAwake(Nexus.SevereEffects severeEffects) {
        ApplyOncomerData();

        ApplySevereEffects(severeEffects);

        CalculatePath();

        LevelManager.instance.RegisterOncomer();
    }


    private void Update() {
        if (GameManager.instance.IsPaused) {
            return;
        }

        MoveThroughPoints();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "station") {
            // TODO: get this number from oncomer data
            LevelManager.instance.DamageStation(m_dmg, m_type);

            Destroy(this.gameObject);
        }
    }

    private void OnDestroy() {
        LevelManager.instance.RemoveOncomer();
    }

    private void MoveThroughPoints() {
        if (m_waypoints == null) {
            return;
        }

        if (m_currWaypointIndex < m_waypoints.Count) {
            Vector2 currPoint = m_waypoints[m_currWaypointIndex];
            MoveToward(currPoint);
        }
        else {
            // Reached destination (should have been destroyed on collision earlier, however)
            Debug.Log("Warning: Oncomer did not collide with target");
            Destroy(this.gameObject);
        }
    }

    private void MoveToward(Vector2 point) {
        Vector2 distance = (point - (Vector2)this.transform.position);
        Vector2 dir = (point - (Vector2)this.transform.position).normalized;

        if (distance.magnitude > WAYPOINT_BUFFER) {
            this.transform.Translate(dir * m_speed * Time.deltaTime);
            distance = (point - (Vector2)this.transform.position);
        }
        else {
            // move the rest of the way
            this.transform.Translate(distance);

            // increment to the next waypoint
            m_currWaypointIndex++;
        }
    }

    private void ApplyOncomerData() {
        m_type = this.OncomerData.Type;
        GetComponent<SpriteRenderer>().sprite = this.OncomerData.Sprite;
        m_canWalkOn = this.OncomerData.CanWalkOn;
        m_speed = this.OncomerData.Speed;
        m_dmg = this.OncomerData.Dmg;
        m_maxHealth = this.OncomerData.MaxHealth;
        m_currHealth = m_maxHealth;
        m_movesDiagonal = this.OncomerData.MovesDiagonal;
    }

    private void ApplySevereEffects(Nexus.SevereEffects effects) {
        m_maxHealth *= (1 + effects.HealthBonusMult);
        m_currHealth = m_maxHealth;
        m_dmg *= (1 + effects.DamageBonusMult);
    }

    private void CalculatePath() {
        m_currWaypointIndex = 0;
        List<Vector2> tryWaypoints = TilemapManager.instance.CalculatePath(m_canWalkOn, this.transform.position, m_movesDiagonal);

        if (tryWaypoints == null) {
            Debug.Log("No possible path!");
            return;
        }

        m_waypoints = tryWaypoints;

        if (m_debugPath) {
            foreach (Vector2 waypoint in m_waypoints) {
                var debugWaypoint = Instantiate(m_debugPrefab, m_debugHolder.transform);
                debugWaypoint.transform.position = waypoint;
            }
            m_debugHolder.transform.parent = null;
        }
    }

    #region Projectile

    public void ApplyDamage(float damage) {
        m_currHealth -= damage;

        if (m_currHealth <= 0) {
            // Handle removal of enemy
            Destroy(this.gameObject);
        }
    }

    #endregion

    public Type GetOncomerType() {
        return m_type;
    }
}
