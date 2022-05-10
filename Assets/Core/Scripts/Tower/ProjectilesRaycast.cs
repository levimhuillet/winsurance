using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilesRaycast 
{
    public static void Shoot(Vector3 shootPos, Vector3 shootDir) {
        // TODO: apply layer mask to filter out tiles,
        // otherwise create a projectile with own 2d collider and trigger effects through trigger collisions
        RaycastHit2D raycastHit2D = Physics2D.Raycast(shootPos, shootDir);
        if (raycastHit2D.collider != null)
        {
            Oncomer target = raycastHit2D.collider.GetComponent<Oncomer>();
            if (target != null)
            {
                // target.ApplyProjectileEffects(); // currently non-functional because raycast hits tiles
            }
        }
            }
}
