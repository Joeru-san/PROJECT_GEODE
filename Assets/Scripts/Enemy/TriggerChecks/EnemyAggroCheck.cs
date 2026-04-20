using UnityEngine;

public class EnemyAggroCheck : MonoBehaviour
{
    private Enemy _enemy;

    void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("DefenseTurret"))
        {
            _enemy.SetAggroStatus(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("DefenseTurret"))
        {
            _enemy.SetAggroStatus(false);
        }
    }
}
