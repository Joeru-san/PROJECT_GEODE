using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TurretEnemyDetect : MonoBehaviour
{
    public Queue<IDamageable> enemiesInCollider = new Queue<IDamageable>();

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
                enemiesInCollider.Enqueue(damageable);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null && enemiesInCollider.Contains(damageable))
            {
                // Rebuild queue without the exiting enemy
                Queue<IDamageable> updated = new Queue<IDamageable>();
                foreach (IDamageable e in enemiesInCollider)
                    if (e != damageable) updated.Enqueue(e);
                enemiesInCollider = updated;
            }
        }
    }
}