using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class Tower : MonoBehaviour {
    public enum Type {
        Default,
        NexusOnly,
        OncomerOnly
    }

    // The tower's state of readiness
    private enum State {
        Armed,
        Reloading
    }

    [SerializeField]
    private Type m_type;
    [SerializeField]
    private Oncomer.Type[] m_oncomerTargetTypes;
    [SerializeField]
    private Nexus.Type[] m_nexusTargetTypes;
    [SerializeField]
    private float shootSpeed = 1f;
    [SerializeField]
    private float radius = 3f;
    [SerializeField]
    private string projectileSoundID = "projectile-default";
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float projectileDamage;

    [SerializeField]
    private CircleCollider2D m_radiusCollider;

    private int m_cost;

    private float reloadTimer = 0f;
    private List<GameObject> m_targets;
    private State currState;
    private AudioSource m_audioSrc;

    public GameObject Tracer;

    private AudioData m_audioData;

    private const float CELL_OFFSET = 0.5f;

    // Handles triggers of this tower's radius collider
    public void HandleTriggerEnter2D(Collider2D collider) {
        // when an intruder enters this tower's range, add it to the list of targets if it is targetable by the tower
        if (collider.gameObject.tag == "target") {
            if (CanTarget(collider.gameObject)) {
                m_targets.Add(collider.gameObject);
            }
        }
    }

    public void HandleTriggerExit2D(Collider2D collider) {
        // when an intruder exits this tower's range, remove it from the list of targets
        if (collider.gameObject.tag == "target") {
            m_targets.Remove(collider.gameObject);
        }
    }

    private void Awake() {
        m_targets = new List<GameObject>();
        currState = State.Armed;
        m_radiusCollider.radius = this.radius;
        m_audioSrc = this.GetComponent<AudioSource>();
    }

    void FixedUpdate() {
        if (GameManager.instance.IsPaused) {
            return;
        }

        // perform actions according to tower's state
        switch (currState) {
            case State.Reloading:
                // run down the reload timer
                reloadTimer -= Time.deltaTime;
                if (reloadTimer <= 0) {
                    // tower has re-armed itself
                    currState = State.Armed;
                }
                break;
            case State.Armed:
                // search for target to shoot at
                if (m_targets.Count == 0) {
                    // no targets means no shooting
                    return;
                }
                Shoot();
                break;
            default:
                break;
        }

    }

    private void Shoot() {
        // TODO: check for nexus
        GameObject chosenTarget = getClosestTarget(transform.position);
        if (chosenTarget == null) {
            return;
        }

        // Create projectile
        Vector3 toPos = chosenTarget.transform.position;
        Vector3 shootDir = (toPos - transform.position).normalized;

        // Produce launching sound
        PlayLaunchSound();

        // Instantiate projectile
        GameObject projectileObj = Instantiate(projectilePrefab);
        projectileObj.transform.position = this.transform.position + new Vector3(CELL_OFFSET, CELL_OFFSET, 0);

        // Assign projectile target
        Projectile projectileComp = projectileObj.GetComponent<Projectile>();
        projectileComp.TargetObj = chosenTarget;
        projectileComp.Damage = projectileDamage;
        projectileComp.TType = chosenTarget.GetComponent<Nexus>() == null ? Projectile.TargetType.Oncomer : Projectile.TargetType.Nexus;

        // tower must now reload
        currState = State.Reloading;
        reloadTimer = shootSpeed;
    }

    private void PlayLaunchSound() {
        AudioClip clip = m_audioData.Clip;
        m_audioSrc.volume = m_audioData.Volume;
        m_audioSrc.PlayOneShot(clip);
    }

    // Note: currently gets the target which first entered the tower's radius
    private GameObject getClosestTarget(Vector3 Position) {
        // clear enemies which have been destroyed
        m_targets = m_targets.FindAll(t => t != null);

        if (m_targets.Count == 0) {
            return null;
        }

        float closestDis = 1000f;
        // TODO: specify priority (currently closest)
        GameObject closestTarget = m_targets[0];
        foreach (GameObject t in m_targets) {
            if (Vector3.Distance(t.transform.position, Position) < closestDis) {
                closestDis = Vector3.Distance(t.transform.position, Position);
                closestTarget = t.gameObject;
            }
        }

        return closestTarget;
    }

    void CreateWeaponTracer(Vector3 fromPos, Vector3 toPos, float width) {
        Vector3[] vt = new Vector3[4];
        Vector3 dir = (fromPos - toPos).normalized;
        vt[0] = fromPos + width * new Vector3(dir.y, -dir.x);
        vt[1] = toPos + width * new Vector3(dir.y, -dir.x);
        vt[2] = toPos + width * new Vector3(-dir.y, dir.x);
        vt[3] = fromPos + width * new Vector3(-dir.y, dir.x);

        GameObject tracerObject = Instantiate(Tracer, new Vector3(0, 0, -1), Quaternion.identity);
        tracerObject.GetComponent<Trace>().duration = .1f;
        tracerObject.GetComponent<Trace>().vertices = vt;
    }

    public void SetFields(TowerData data) {
        m_type = data.Type;
        this.GetComponent<SpriteRenderer>().sprite = data.Sprite;
        m_oncomerTargetTypes = data.OncomerTargets;
        m_nexusTargetTypes = data.NexusTargets;
        shootSpeed = data.ShootSpeed;
        radius = data.Radius;

        // TODO: set projectiles with their own data
        projectileSoundID = data.ProjectileSoundID;
        projectilePrefab = data.ProjectilePrefab;
        projectileDamage = data.ProjectileDamage;

        m_cost = data.Cost;

        m_audioData = GameDB.instance.GetAudioData(projectileSoundID);
    }

    private bool CanTarget(GameObject potentialTarget) {
        Oncomer oncomer = potentialTarget.GetComponent<Oncomer>();
        if (oncomer != null) {
            foreach (Oncomer.Type type in m_oncomerTargetTypes) {
                if (oncomer.GetOncomerType() == type) {
                    return true;
                }
            }
            return false;
        }
        else {
            Nexus nexus = potentialTarget.GetComponent<Nexus>();
            if (nexus != null) {
                foreach (Nexus.Type type in m_nexusTargetTypes) {
                    if (nexus.GetNexusType() == type) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
