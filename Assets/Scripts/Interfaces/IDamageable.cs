public interface IDamageable
{
    void TakeDamage(float damageAmount);

    void Die();

    #region Health Stuff
    float MaxHealth {get; set;}
    float currentHealth {get; set;}
    #endregion

    #region Attack stuff
    float attackCoolDown {get; set;}
    float attackDamage {get; set;}
    #endregion
}
