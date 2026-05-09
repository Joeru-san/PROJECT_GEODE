using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SphereCollider))]
public class TutorialGuy : MonoBehaviour
{
    public static bool isPlayerInCollider = false;
    public GameObject playerReference;
    public GameObject companionReference;
    public bool isDebug;

    [SerializeField] float pushBackDistance = 1.5f;
    [SerializeField] float resumeDistance = 2f;

    SphereCollider _sphereCollider;
    Vector3 _lastPositionInsideCollider;
    NavMeshAgent _navMeshAgent;
    bool _isWaitingForPlayer = false;

    void Awake()
    {
        GoToPointQuest.GoToNewPoint += MoveToNewPosition;
    }

    void OnDestroy()
    {
        GoToPointQuest.GoToNewPoint -= MoveToNewPosition;
    }

    void Start()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        isPlayerInCollider = IsPlayerInsideCollider();

        if (isPlayerInCollider)
            _lastPositionInsideCollider = playerReference.transform.position;
    }

    void Update()
    {
        if (playerReference == null) return;

        // Track last safe position
        if (isPlayerInCollider)
            _lastPositionInsideCollider = playerReference.transform.position;

        // Resume check — only runs while waiting
        if (!_isWaitingForPlayer) return;

        float dist = Vector3.Distance(
            playerReference.transform.position,
            companionReference.transform.position
        );

        if(isDebug) Debug.Log($"[{GetType().Name}] dist to companion: {dist:F2} / resume at: {resumeDistance}");

        if (dist <= resumeDistance)
        {
            _isWaitingForPlayer = false;
            ResumeMovement();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInCollider = true;
            if(isDebug) Debug.Log($"[{GetType().Name}] player entered the trigger");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInCollider = false;
            if(isDebug) Debug.Log($"[{GetType().Name}] player left the trigger");
            PushPlayerBack();
            StopMovement();
        }
    }

    void PushPlayerBack()
    {
        if (playerReference == null) return;

        Vector3 pushDirection = (_lastPositionInsideCollider - companionReference.transform.position).normalized;
        Vector3 targetPos = _lastPositionInsideCollider - pushDirection * pushBackDistance;
        targetPos.y = _lastPositionInsideCollider.y;

        playerReference.transform.position = targetPos;

        var rb = playerReference.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = Vector3.zero;

        if(isDebug) Debug.Log($"[{GetType().Name}] player pushed back to {targetPos}");
    }

    void StopMovement()
    {
        if (_navMeshAgent == null || !_navMeshAgent.isOnNavMesh) return;

        _navMeshAgent.isStopped = true;
        _isWaitingForPlayer = true;
        if(isDebug) Debug.Log($"[{GetType().Name}] companion stopped");
    }

    void ResumeMovement()
    {
        if (_navMeshAgent == null || !_navMeshAgent.isOnNavMesh) return;

        _navMeshAgent.isStopped = false;
        if(isDebug) Debug.Log($"[{GetType().Name}] companion resumed");
    }

    bool IsPlayerInsideCollider()
    {
        if (playerReference == null) return false;

        Vector3 center = transform.TransformPoint(_sphereCollider.center);
        float radius = _sphereCollider.radius * transform.lossyScale.x;

        return Vector3.Distance(playerReference.transform.position, center) <= radius;
    }

    void MoveToNewPosition(Vector3 newPosition)
    {
        if (_navMeshAgent == null) return;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(newPosition, out hit, 2f, NavMesh.AllAreas))
        {
            _navMeshAgent.isStopped = false;
            _isWaitingForPlayer = false;
            _navMeshAgent.SetDestination(hit.position);
            if(isDebug) Debug.Log($"[{GetType().Name}] moving companion to sampled position {hit.position}");
        }
        else
        {
            Debug.LogWarning($"[{GetType().Name}] could not find valid NavMesh position near {newPosition}");
        }
    }
}