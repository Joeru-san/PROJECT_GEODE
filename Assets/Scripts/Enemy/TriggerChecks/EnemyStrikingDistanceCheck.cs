using UnityEngine;

public class EnemyStrikingDistanceCheck : MonoBehaviour
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
            _enemy.SetStrikingDistanceBool(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("DefenseTurret"))
        {
            _enemy.SetStrikingDistanceBool(false);
        }
    }
}
