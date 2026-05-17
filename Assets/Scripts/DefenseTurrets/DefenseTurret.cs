using UnityEngine;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class DefenseTurret : MonoBehaviour, IDamageable
{
    #region IDamageable attributes
    [field: Header("IDamageable")]
    [field: SerializeField] public float MaxHealth { get; set; }
    [field: SerializeField] public float currentHealth { get; set; }
    [field: SerializeField] public float attackCoolDown { get; set; }
    [field: SerializeField] public float attackDamage { get; set; }
    [field: SerializeField] public float healRate { get; set; }
    #endregion

    public float inactiveTime = 3f;
    public float rayDuration = 0.5f;
    public Transform shootPosition;
    public bool printDebug = false;

    TurretEnemyDetect _relatedCollider;
    LineRenderer _lineRend;
    public bool isAttacking = false;
    IDamageable _currentTarget = null;
    HitFlash _selfHitFlash;

    [Header("UI")]
    [SerializeField] HealthBar healthBar;

    void Awake()
    {
        _relatedCollider = GetComponentInChildren<TurretEnemyDetect>();
        _lineRend = GetComponent<LineRenderer>();
        _lineRend.enabled = false;
    }

    void Start()
    {
        currentHealth = MaxHealth;
    }

    void Update()
    {
        _selfHitFlash = GetComponent<HitFlash>();
        _timeSinceLastDamage += Time.deltaTime;

        if (_timeSinceLastDamage >= recoverDelay && currentHealth < MaxHealth && !_isRecovering)
        {
            _isRecovering = true;
            RecoverToMaxHealthOverTime();
        }

        if (_currentTarget != null && _currentTarget.currentHealth <= 0)
            _currentTarget = null;

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
                _relatedCollider.enemiesInCollider.Dequeue();
            }
        }

        if (_currentTarget != null && !isAttacking)
            StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;

        Transform targetTransform = (_currentTarget as MonoBehaviour)?.transform;
        if (targetTransform != null)
        {
            Vector3 flatTarget = new Vector3(targetTransform.position.x, transform.position.y, targetTransform.position.z);
            transform.DODynamicLookAt(flatTarget, 0.2f);
        }

        yield return new WaitForSeconds(attackCoolDown);

        if (_currentTarget != null && _currentTarget.currentHealth > 0)
        {
            Transform t = (_currentTarget as MonoBehaviour)?.transform;
            if (t != null)
            {
                _lineRend.SetPosition(0, shootPosition.position);
                _lineRend.SetPosition(1, t.position);
                StartCoroutine(ShowLineRend());
            }

            #if UNITY_EDITOR
            if (printDebug) Debug.Log($"[{GetType().Name}] DefenseTurret {name} hit {_currentTarget} for {attackDamage} damage");
            #endif
            _currentTarget.TakeDamage(attackDamage);
        }
        else
        {
            _currentTarget = null;
        }

        isAttacking = false;
    }

    IEnumerator ShowLineRend()
    {
        _lineRend.enabled = true;
        yield return new WaitForSeconds(rayDuration);
        _lineRend.enabled = false;
    }

    public float recoverDelay = 3f;

    float _timeSinceLastDamage = 0f;
    bool _isRecovering = false;

    #region IDamageable methods
    public void Die()
    {
        gameObject.SetActive(false);
        DOVirtual.DelayedCall(inactiveTime, () =>
        {
            gameObject.SetActive(true);
            currentHealth = MaxHealth;
            _selfHitFlash.ResetColors();
        });
    }

    public void TakeDamage(float damageAmount)
    {
        _timeSinceLastDamage = 0f;
        _selfHitFlash?.Flash();
        _isRecovering = false;

        DOTween.Kill(gameObject);
        currentHealth = Mathf.Max(currentHealth - damageAmount, 0f);

        RefreshHealthUI();

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
        Debug.Log($"Starting health recover for {transform.name}");
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