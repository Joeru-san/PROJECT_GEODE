using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class DefenseTurret : MonoBehaviour, IDamageable
{
    #region IDamageable
    [field: Header("IDamageable")]
    [field: SerializeField] public float MaxHealth { get; set; }
    [field: SerializeField] public float currentHealth { get; set; }
    [field: SerializeField] public float attackCoolDown { get; set; }
    [field: SerializeField] public float attackDamage { get; set; }
    #endregion

    public bool printDebug = false;

    TurretEnemyDetect _relatedCollider;
    bool _isAttacking = false;
    IDamageable _currentTarget = null;

    public void Die()
    {
        Destroy(this.gameObject);
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Awake()
    {
        _relatedCollider = GetComponent<TurretEnemyDetect>();
    }

    void Start()
    {
        currentHealth = MaxHealth;
    }

    void Update()
    {
        // Clear current target if it's dead
        if (_currentTarget != null && _currentTarget.currentHealth <= 0)
        {
            _currentTarget = null;
        }

        // Pick a new target from the queue if we don't have one
        if (_currentTarget == null)
        {
            while (_relatedCollider.enemiesInCollider.Count > 0)
            {
                IDamageable candidate = _relatedCollider.enemiesInCollider.Peek();
                if (candidate != null && candidate.currentHealth > 0)
                {
                    _currentTarget = candidate;
                    break;
                }
                // Discard dead or null entries
                _relatedCollider.enemiesInCollider.Dequeue();
            }
        }

        if (_currentTarget != null && !_isAttacking)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    IEnumerator AttackCoroutine()
    {
        _isAttacking = true;

        yield return new WaitForSeconds(attackCoolDown);

        if (_currentTarget != null && _currentTarget.currentHealth > 0)
        {
            #if UNITY_EDITOR
            if(printDebug) Debug.Log($"[{GetType().Name}] DefenseTurret {name} hit {_currentTarget} for {attackDamage} damage");
            #endif
            _currentTarget.TakeDamage(attackDamage);
        }
        else
        {
            // Target died or left during cooldown — clear so Update picks the next one
            _currentTarget = null;
        }

        _isAttacking = false;
    }
}