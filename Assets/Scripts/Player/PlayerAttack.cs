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

    LineRenderer lineRend;
    

    void Awake()
    {
        _relatedPlayer = GetComponent<PlayerMovement>();
        lineRend = GetComponent<LineRenderer>();
        lineRend.enabled = false;
    }

    void OnAim(InputValue value)
    {
        _isAiming = value.isPressed;
        CameraController.inst.ChangeCamera();
    }

    void OnAttack()
    {
        if(!_isAttacking && _isAiming)
        {
            StartCoroutine(AttackCoroutine());
        }
    }
    void Update()
    {
        print(_isAiming);
    }

    IEnumerator AttackCoroutine()
    {
        _isAttacking = true;

        lineRend.SetPosition(0, shootPosition.position);
        
        Vector3 rayOrigin = _mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit outHit;

        if(Physics.Raycast(rayOrigin, _mainCamera.transform.forward, out outHit, gunRange, layerToHit))
        {
            lineRend.SetPosition(1, outHit.point);
            Debug.Log($"{outHit.transform.name} hitted!");
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
            lineRend.SetPosition(1, rayOrigin + (_mainCamera.transform.forward * gunRange));
            Debug.Log("No enemy hitted");
        }
        
        StartCoroutine(ShowLineRend());

        yield return new WaitForSeconds(_relatedPlayer.attackCoolDown);

        _isAttacking = false;
    }

    IEnumerator ShowLineRend()
    {
        lineRend.enabled = true;
        yield return new WaitForSeconds(rayDuration);
        lineRend.enabled = false;
    }
}
