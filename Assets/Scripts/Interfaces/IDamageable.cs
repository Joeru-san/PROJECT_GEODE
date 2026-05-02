public interface IDamageable
{
    #region Health Stuff
    float MaxHealth {get; set;}
    float currentHealth {get; set;}
    float healRate {get; set;}
    #endregion

    #region Attack stuff
    float attackCoolDown {get; set;}
    float attackDamage {get; set;}
    #endregion

    void TakeDamage(float damageAmount);

    void Die();

    void RecoverHealth(float healthToRecover);

    void RecoverHealthOverTime(float recoverTime);

    void RecoverToMaxHealthOverTime();
}
