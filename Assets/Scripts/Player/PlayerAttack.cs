using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerAttack : MonoBehaviour
{
    PlayerMovement _relatedPlayer;
    bool _isAttacking = false;
    bool _isAiming = false;

    public Camera _mainCamera;
    public float gunRange = 5f;
    public float rayDuration = 0.5f;
    public Transform shootPosition;
    public LayerMask layerToHit;

    LineRenderer _lineRend;
    

    void Awake()
    {
        _relatedPlayer = GetComponent<PlayerMovement>();
        _lineRend = GetComponent<LineRenderer>();
        _lineRend.enabled = false;
    }

    void OnAim(InputValue value)
    {
        _isAiming = value.isPressed;
        // CameraController.inst.ChangeCamera();
    }

    void OnAttack()
    {
        if(!_isAttacking && CameraController.inst.activeCamera == CameraController.inst.firstPersonCamera)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    IEnumerator AttackCoroutine()
    {
        _isAttacking = true;

        _lineRend.SetPosition(0, shootPosition.position);
        
        Vector3 rayOrigin = _mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0f));
        RaycastHit outHit;

        if(Physics.Raycast(rayOrigin, _mainCamera.transform.forward, out outHit, gunRange, layerToHit))
        {
            _lineRend.SetPosition(1, outHit.point);
            IDamageable damageable = outHit.transform.gameObject.GetComponentInChildren<IDamageable>();
            if(damageable != null)
            {
                Debug.Log($"[{GetType().Name}] Player {_relatedPlayer.name} hit {damageable} for {_relatedPlayer.attackDamage} damage");
                damageable.TakeDamage(_relatedPlayer.attackDamage);
            } else
            {
                Debug.Log($"[{GetType().Name}] Player {_relatedPlayer.name} didn't get IDamageable component");
            }
        } else
        {
            _lineRend.SetPosition(1, rayOrigin + (_mainCamera.transform.forward * gunRange));
            Debug.Log("No enemy hitted");
        }
        
        StartCoroutine(ShowLineRend());

        yield return new WaitForSeconds(_relatedPlayer.attackCoolDown);

        _isAttacking = false;
    }

    IEnumerator ShowLineRend()
    {
        _lineRend.enabled = true;
        yield return new WaitForSeconds(rayDuration);
        _lineRend.enabled = false;
    }
}
