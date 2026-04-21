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
            _enemy.currentTarget = other.gameObject;
            _enemy.SetAggroStatus(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("DefenseTurret"))
        {
            _enemy.currentTarget = null;
            _enemy.SetAggroStatus(false);
        }
    }

    void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.color = Color.orange;
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(box.center, box.size);
        Gizmos.matrix = old;
        Gizmos.color = Color.white;
    }
}
