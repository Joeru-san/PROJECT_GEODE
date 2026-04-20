public interface IDamageable
{
    void TakeDamage(float damageAmount);

    void Die();

    float MaxHealth {get; set;}
    float currentHealth {get; set;}
}
