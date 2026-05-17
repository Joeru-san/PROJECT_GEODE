using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour, IDamageable
{
    private Rigidbody _playerRB;
    private Vector2 _moveAmt;

    public static Action<PlayerInput> OnShowInventory;
    public static Action<PlayerInput> OnControlSchemeChange;
    public static Action<PlayerInput> OnShowMap;

    [Header("Player Speeds")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 10f;
    [Range(0,1)] public float speedTweenDuration = 0.3f;
    public bool _isSprinting = false;
    private float _actualSpeed;

    public static bool isDead = false;

    public static PlayerInput playerInput;

    #region IDamageable attributes
    [field: Header("IDamageable")]
    [field: SerializeField] public float MaxHealth { get; set; }
    [field: SerializeField] public float currentHealth { get; set; }
    [field: SerializeField] public float attackCoolDown { get; set; }
    [field: SerializeField] public float attackDamage { get; set; }
    [field: SerializeField] public float healRate { get; set; }
    #endregion

    public float recoverDelay = 3f;

    float _timeSinceLastDamage = 0f;
    bool _isRecovering = false;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundMask;
    
    [SerializeField] Transform spawnPoint;

    [Header("UI")]
    [SerializeField] HealthBar healthBar;

    void Awake()
    {
        _playerRB = gameObject.GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        currentHealth = MaxHealth;
        _actualSpeed = walkSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnControlsChanged(PlayerInput input)
    {
        OnControlSchemeChange?.Invoke(playerInput);
        
        if (CameraController.inst == null) return; // Not ready yet, skip

        if (input.currentControlScheme == "Gamepad")
            CameraController.inst.ChangeGainAllCameras(100f);
        else
            CameraController.inst.ChangeGainAllCameras(1f);
    }

    void Start()
    {
        playerInput.controlsChangedEvent.AddListener(OnControlsChanged);
        if(spawnPoint == null)
        {
            spawnPoint = transform;
        }
    }

    void Update()
    {
        _timeSinceLastDamage += Time.deltaTime;

        if (_timeSinceLastDamage >= recoverDelay && currentHealth < MaxHealth && !_isRecovering)
        {
            _isRecovering = true;
            RecoverToMaxHealthOverTime();
        }
        
        if(gameObject.transform.position.y < -20 && !isDead) 
        {
            isDead = true;
            StartCoroutine(Death(1f));
        }
    }

    void FixedUpdate()
    {
        if (!isDead && _moveAmt != Vector2.zero)
        {
            Vector3 moveDirection = (_moveAmt != Vector2.zero)
                ? (transform.forward * _moveAmt.y + transform.right * _moveAmt.x).normalized
                : Vector3.zero;

            Vector3 currentVel = _playerRB.linearVelocity;
            Vector3 targetVel = moveDirection * _actualSpeed;
            targetVel.y = currentVel.y; // preserve gravity

            // Lerp only the horizontal velocity for snappy but smooth control
            Vector3 smoothedVel = new Vector3(
                Mathf.Lerp(currentVel.x, targetVel.x, 0.3f),  // tweak 0.3f for feel
                targetVel.y,
                Mathf.Lerp(currentVel.z, targetVel.z, 0.3f)
            );

            _playerRB.linearVelocity = smoothedVel;
        }
    }

    void OnMove(InputValue value)
    {
        _moveAmt = value.Get<Vector2>();
    }

    void OnSprint(InputValue value)
    {
        _isSprinting = value.isPressed;
        float targetSpeed = _isSprinting ? sprintSpeed : walkSpeed;
        TweenSpeed(targetSpeed);
    }

    void TweenSpeed(float targetSpeed)
    {
        DOTween.Kill("PlayerSpeed");

        DOTween.To(() => _actualSpeed, x => _actualSpeed = x, targetSpeed, speedTweenDuration)
            .SetEase(Ease.InOutQuad)
            .SetId("PlayerSpeed");
    }

    void OnJump()
    {
        if(IsGrounded() && !PauseManager.inst.IsPaused)
        {
            _playerRB.linearVelocity = Vector3.zero;
            _playerRB.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
        }
    }

    void OnInventory()
    {
        if(!PauseManager.inst.IsPaused && !ShopUI.isShopPanelOpen)
            OnShowInventory?.Invoke(playerInput);
    }

    void OnMap()
    {
        if(!PauseManager.inst.IsPaused && !ShopUI.isShopPanelOpen && !InventoryUI.isPanelOpen)
            OnShowMap?.Invoke(playerInput);
    }
    
    void RefreshHealthUI()
    {
        healthBar?.SetHealth(currentHealth, MaxHealth);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
    }

    IEnumerator Death(float animationDuration)
    {
        _playerRB.linearVelocity = Vector3.zero;
        _playerRB.angularVelocity = Vector3.zero;
        _playerRB.isKinematic = true;

        yield return new WaitForSeconds(animationDuration);

        transform.position = spawnPoint.transform.position;
        _playerRB.position = spawnPoint.transform.position;

        yield return null;

        _playerRB.isKinematic = false;
        _playerRB.linearVelocity = Vector3.zero;
        _playerRB.angularVelocity = Vector3.zero;

        isDead = false;
    }

    private void OnDrawGizmos()
    {
        if(groundCheck != null)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    #region IDamageable methods
    public void Die()
    {
        isDead = true;
        StartCoroutine(Death(1f)); 
        PauseManager.inst.canPause = false;
    }
    
    public void TakeDamage(float damageAmount)
    {
        _timeSinceLastDamage = 0f;
        _isRecovering = false;
        DOTween.Kill(gameObject);
        currentHealth = Mathf.Max(currentHealth - damageAmount, 0f);
        RefreshHealthUI();
        CameraController.inst.ShakeDamage(damageAmount, currentHealth);
        if (currentHealth <= 0f) Die();
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
            .OnComplete(() => _isRecovering = false);
    }
    #endregion
}
