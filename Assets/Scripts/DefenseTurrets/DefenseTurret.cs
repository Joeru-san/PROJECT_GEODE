using UnityEngine;

public class DefenseTurret : MonoBehaviour, IDamageable
{
    #region IDamageable
    [field: Header("IDamageable")]
    [field: SerializeField] public float MaxHealth {get; set;}
    [field: SerializeField] public float currentHealth {get; set;}
    [field: SerializeField] public float attackCoolDown {get; set;}
    [field: SerializeField] public float attackDamage {get; set;}
    #endregion

    public void Die()
    {
        Destroy(this);
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Start()
    {
        currentHealth = MaxHealth;
    }
}
