using DG.Tweening;
using UnityEngine;

public class CentralHub : MonoBehaviour, IDamageable
{
    #region IDamageable attributes
    public float MaxHealth {get; set;}
    public float currentHealth {get; set;}
    public float attackCoolDown {get; set;}
    public float attackDamage {get; set;}
    public float healRate {get; set;}
    #endregion

    #region IDamageable methods
    public void Die()
    {
        Debug.Log("HAI PERSO");
    }

    public void TakeDamage(float damageAmount)
    {
        DOTween.Kill("healSequence");
        currentHealth = Mathf.Max(currentHealth - damageAmount, 0f);

        if (currentHealth <= 0f)
            Die();
    }

    public void RecoverHealth(float healthToRecover)
    {
        currentHealth = Mathf.Min(currentHealth + healthToRecover, MaxHealth);
    }

    public void RecoverHealthOverTime(float recoverTime)
    {
        float targetHealth = Mathf.Min(currentHealth + healRate * recoverTime, MaxHealth);

        DOTween.To(() => currentHealth, x => currentHealth = x, targetHealth, recoverTime)
            .SetEase(Ease.OutQuad)
            .SetId("healSequence");
    }

    public void RecoverToMaxHealthOverTime()
    {
        float duration = (MaxHealth - currentHealth) / healRate;

        DOTween.Kill("healSequence");
        DOTween.To(() => currentHealth, x => currentHealth = x, MaxHealth, duration)
            .SetEase(Ease.OutQuad)
            .SetId("healSequence");
    }
    #endregion
}
