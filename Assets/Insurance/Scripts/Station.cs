using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Station : MonoBehaviour
{
    private HealthManager m_healthManager;

    public void ApplyDamage(float dmg, Oncomer.Type type) {
        m_healthManager.ModifyHealth(-dmg, type);
    }

    public void InitHealth(HealthManager healthManager, float startBase, float startFlood, float startFire, float startStorm, float startUmbrella) {
        m_healthManager = healthManager;
        m_healthManager.InitFields(startBase, startFlood, startFire, startStorm, startUmbrella);
    }
}
