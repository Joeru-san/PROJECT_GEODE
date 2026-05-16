using DG.Tweening;
using UnityEngine;

public class CentralHub : MonoBehaviour, IDamageable
{
    #region IDamageable attributes
    [field: SerializeField] public float MaxHealth {get; set;}
    [field: SerializeField] public float currentHealth {get; set;}
    [field: SerializeField] public float attackCoolDown {get; set;}
    [field: SerializeField] public float attackDamage {get; set;}
    [field: SerializeField] public float healRate {get; set;}
    #endregion
    
    public float recoverDelay = 3f;

    float _timeSinceLastDamage = 0f;
    bool _isRecovering = false;

    HitFlash _selfHitFlash;

    void Awake()
    {
        _selfHitFlash = GetComponent<HitFlash>();
    }

    void Start()
    {
        currentHealth = MaxHealth;
    }

    void Update()
    {
        _timeSinceLastDamage += Time.deltaTime;

        if (_timeSinceLastDamage >= recoverDelay && currentHealth < MaxHealth && !_isRecovering)
        {
            _isRecovering = true;
            RecoverToMaxHealthOverTime();
        }
    }

    #region IDamageable methods
    public void Die()
    {
        Debug.Log("HAI PERSO");
    }

    public void TakeDamage(float damageAmount)
    {
        _timeSinceLastDamage = 0f;
        _isRecovering = false;
        _selfHitFlash.Flash();


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
            .SetId("healSequence")
            .OnComplete(() => {
                _isRecovering = false;
            });
    }
    #endregion
}
