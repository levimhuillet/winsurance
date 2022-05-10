using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusCollider : MonoBehaviour
{
    private Tower m_parentTower;

    void Awake() {
        m_parentTower = this.transform.parent.GetComponent<Tower>();

        if (m_parentTower == null) {
            Debug.Log("Warning: tower's radus could not find it's corresponding tower!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        m_parentTower.HandleTriggerEnter2D(collider);
    }

    private void OnTriggerExit2D(Collider2D collider) {
        m_parentTower.HandleTriggerExit2D(collider);
    }
}
