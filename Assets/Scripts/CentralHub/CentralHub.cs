using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("UI")]
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject gameOverPanel;

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
        gameOverPanel.SetActive(true);
    }

    public void TakeDamage(float damageAmount)
    {
        _timeSinceLastDamage = 0f;
        _isRecovering = false;
        _selfHitFlash.Flash();

        RefreshHealthUI();


        DOTween.Kill(gameObject);
        currentHealth = Mathf.Max(currentHealth - damageAmount, 0f);

        if (currentHealth <= 0f)
            Die();
    }

    public void RecoverHealth(float healthToRecover)
    {
        currentHealth = Mathf.Min(currentHealth + healthToRecover, MaxHealth);
        RefreshHealthUI();

    }

    public void RecoverHealthOverTime(float recoverTime)
    {
        float targetHealth = Mathf.Min(currentHealth + healRate * recoverTime, MaxHealth);

        DOTween.To(() => currentHealth, x => currentHealth = x, targetHealth, recoverTime)
            .SetEase(Ease.OutQuad)
            .SetId(gameObject);
    }

    public void RecoverToMaxHealthOverTime()
    {
        float duration = (MaxHealth - currentHealth) / healRate;

        DOTween.Kill(gameObject);
        DOTween.To(() => currentHealth, x => { currentHealth = x; RefreshHealthUI(); }, MaxHealth, duration)
        .SetEase(Ease.OutQuad)
        .SetId(gameObject)
        .OnComplete(() =>
        {
            Debug.Log($"Finished health recover for {transform.name}");
            _isRecovering = false;
        });
    }
    #endregion

    void RefreshHealthUI()
    {
        healthBar?.SetHealth(currentHealth, MaxHealth);
    }
}
