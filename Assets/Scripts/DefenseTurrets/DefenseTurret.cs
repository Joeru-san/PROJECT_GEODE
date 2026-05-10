using UnityEngine;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
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
    bool _isAttacking = false;
    IDamageable _currentTarget = null;

    void Awake()
    {
        _relatedCollider = GetComponent<TurretEnemyDetect>();
        _lineRend = GetComponent<LineRenderer>();
        _lineRend.enabled = false;
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

        if (_currentTarget != null && !_isAttacking)
            StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        _isAttacking = true;

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

        _isAttacking = false;
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
        });
    }

    public void TakeDamage(float damageAmount)
    {
        _timeSinceLastDamage = 0f;
        _isRecovering = false;

        DOTween.Kill(gameObject);
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
            .SetId(gameObject);
    }

    public void RecoverToMaxHealthOverTime()
    {
        Debug.Log($"Starting health recover for {transform.name}");
        float duration = (MaxHealth - currentHealth) / healRate;

        DOTween.Kill(gameObject);
        DOTween.To(() => currentHealth, x => currentHealth = x, MaxHealth, duration)
            .SetEase(Ease.OutQuad)
            .SetId(gameObject)
            .OnComplete(() =>
            {
                Debug.Log($"Finished health recover for {transform.name}");
                _isRecovering = false;
            });
    }
    #endregion
}