using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum TargetType {
        Oncomer,
        Nexus
    }

    [SerializeField]
    private float m_speed;

    private const float CELL_OFFSET = 0.5f;

    public GameObject TargetObj {
        get; set;
    }
    public float Damage {
        get; set;
    }
    public TargetType TType {
        get; set;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject == TargetObj) {
            // affect the target
            if (TType == TargetType.Oncomer) {
                TargetObj.GetComponent<Oncomer>().ApplyDamage(this.Damage);
            }
            else if (TType == TargetType.Nexus) {
                TargetObj.GetComponent<Nexus>().ApplyDamage(this.Damage);
            }
            
            // destroy this projectile
            Destroy(this.gameObject);
        }
    }

    private void Update() {
        if (TargetObj == null) {
            // target has been destroyed before this projectile reached it
            Destroy(this.gameObject);
            return;
        }

        float finalOffset = 0;
        if (TType != TargetType.Nexus) {
            finalOffset = CELL_OFFSET;
        }

        // calculate trajectory
        Vector3 targetPos = new Vector3(
            TargetObj.transform.position.x + finalOffset,
            TargetObj.transform.position.y + finalOffset,
            TargetObj.transform.position.z
            );
        Vector2 dir = (targetPos - this.transform.position).normalized;

        // move towards target
        this.transform.Translate(dir * m_speed * Time.deltaTime);
    }
}
