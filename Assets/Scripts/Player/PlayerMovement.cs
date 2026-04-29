using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IDamageable
{
    private Rigidbody _playerRB;
    private Vector2 _moveAmt;
    public static Action<PlayerInput> OnShowInventory;

    [Header("Player Speeds")]
    public float walkSpeed = 5;
    public float rotateSpeed = 5;

    public static bool isDead = false;

    #region IDamageable
    [field: Header("IDamageable")]
    [field: SerializeField] public float MaxHealth { get; set; }
    [field: SerializeField] public float currentHealth { get; set; }
    [field: SerializeField] public float attackCoolDown { get; set; }
    [field: SerializeField] public float attackDamage { get; set; }

    public void Die()
    {
        isDead = true;
        StartCoroutine(Death(1f)); 
    }
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    #endregion

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundMask;

    void Awake()
    {
        _playerRB = gameObject.GetComponent<Rigidbody>();
        currentHealth = MaxHealth;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    

    void Update()
    {
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
            Vector3 moveDirection = (transform.forward * _moveAmt.y + transform.right * _moveAmt.x).normalized;
            
            _playerRB.MovePosition(_playerRB.position + moveDirection * walkSpeed * Time.fixedDeltaTime);
        }
    }

    void OnMove(InputValue value)
    {
        _moveAmt = value.Get<Vector2>();
    }

    void OnJump()
    {
        if(IsGrounded())
        {
            _playerRB.linearVelocity = Vector3.zero;
            _playerRB.AddForce(new Vector3(0f, 5f, 0f), ForceMode.Impulse);
        }
    }

    void OnChangeCamera()
    {
        CameraController.inst.ChangeCamera();
    }

    void OnInventory()
    {
        OnShowInventory?.Invoke(GetComponent<PlayerInput>());
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

        transform.position = checkpoint.GetActiveCheckPointPosition();
        _playerRB.position = checkpoint.GetActiveCheckPointPosition();

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
}
