using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;

    private InputAction moveAction;
    private Vector2 moveAmt;

    private InputAction jumpAction;
    public float jumpForce;

    private Rigidbody playerRB;

    [Header("Player Speeds")]
    public float walkSpeed = 5;
    public float rotateSpeed = 5;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundMask;

    [Header("Player Variables")]
    public static bool isDead = false;
    public static bool changeCameraWasPressedThisFrame {get; private set;} = false;

    void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        if(moveAction == null)
        {
            Debug.LogError("Can't assign move action from input system!");
        }

        jumpAction = InputSystem.actions.FindAction("Jump");

        if(jumpAction == null)
        {
            Debug.LogError("Can't assign jump action from input system!");
        }

        playerRB = gameObject.GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        moveAmt = moveAction.ReadValue<Vector2>();

        if (jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }

        if(gameObject.transform.position.y < -20 && !isDead) 
        {
            isDead = true;
            StartCoroutine(Death(1f));
        }

        changeCameraWasPressedThisFrame = inputActions.FindAction("ChangeCamera").WasPressedThisFrame();
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            Walking();
        }
    }

    void Walking()
    {
        Vector3 moveDirection = (transform.forward * moveAmt.y + transform.right * moveAmt.x).normalized;
        playerRB.MovePosition(playerRB.position + moveDirection * walkSpeed * Time.deltaTime);
    }

    void Jump()
    {
        playerRB.linearVelocity = Vector3.zero;
        playerRB.AddForce(new Vector3(0f, 5f, 0f), ForceMode.Impulse);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
    }

    IEnumerator Death(float animationDuration)
    {
        playerRB.linearVelocity = Vector3.zero;
        playerRB.angularVelocity = Vector3.zero;
        playerRB.isKinematic = true;

        yield return new WaitForSeconds(animationDuration);

        transform.position = checkpoint.GetActiveCheckPointPosition();
        playerRB.position = checkpoint.GetActiveCheckPointPosition();

        yield return null;

        playerRB.isKinematic = false;
        playerRB.linearVelocity = Vector3.zero;
        playerRB.angularVelocity = Vector3.zero;

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
