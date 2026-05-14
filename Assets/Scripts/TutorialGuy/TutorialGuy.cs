using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SphereCollider))]
public class TutorialGuy : MonoBehaviour
{
    public static bool isPlayerInCollider = false;
    public GameObject playerReference;
    public GameObject companionReference;
    public Animator animator;
    public bool isDebug;

    [SerializeField] float pushBackDistance = 1.5f;
    [SerializeField] float resumeDistance = 2f;

    SphereCollider _sphereCollider;
    Vector3 _lastPositionInsideCollider;
    NavMeshAgent _navMeshAgent;
    Vector3 _actualPointToReach;
    bool _isWaitingForPlayer = false;

    public ParticleSystem questRay;

    public static TutorialGuy inst;

    void Awake()
    {
        if (inst != null && inst != this)
        {
            Destroy(gameObject);
            return;
        }
        inst = this;
        DontDestroyOnLoad(gameObject);

        GoToPointQuest.GoToNewPoint += MoveToNewPosition;
        QuestManager.OnAllQuestEnded += FinishTutorial;
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
        
        questRay.Stop();

        if (isPlayerInCollider)
            _lastPositionInsideCollider = playerReference.transform.position;
    }

    void Update()
    {
        if (playerReference == null) return;

        if (isPlayerInCollider)
            _lastPositionInsideCollider = playerReference.transform.position;

        if (!_navMeshAgent.pathPending
            && _navMeshAgent.path.status == NavMeshPathStatus.PathComplete
            && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            animator.SetBool("isWalking", false);
            questRay.Stop();
        }

        if (!_isWaitingForPlayer) return;

        float dist = Vector3.Distance(
            playerReference.transform.position,
            companionReference.transform.position
        );

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

    void StopMovement()
    {
        if (_navMeshAgent == null || !_navMeshAgent.isOnNavMesh) return;

        _navMeshAgent.isStopped = true;
        _isWaitingForPlayer = true;
        animator.SetBool("isWalking", false);
        if(isDebug) Debug.Log($"[{GetType().Name}] companion stopped");
    }

    void ResumeMovement()
    {
        if (_navMeshAgent == null || !_navMeshAgent.isOnNavMesh) return;

        _navMeshAgent.isStopped = false;
        animator.SetBool("isWalking", true);
        if(isDebug) Debug.Log($"[{GetType().Name}] companion resumed");
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
            _actualPointToReach = hit.position;
            animator.SetBool("isWalking", true);

            questRay.transform.position = hit.position;
            questRay.Play();
            
            if(isDebug) Debug.Log($"[{GetType().Name}] moving companion to sampled position {hit.position}");
        }
        else
        {
            Debug.LogWarning($"[{GetType().Name}] could not find valid NavMesh position near {newPosition}");
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
    bool IsPlayerInsideCollider()
    {
        if (playerReference == null) return false;

        Vector3 center = transform.TransformPoint(_sphereCollider.center);
        float radius = _sphereCollider.radius * transform.lossyScale.x;

        return Vector3.Distance(playerReference.transform.position, center) <= radius;
    }

    void FinishTutorial()
    {
        _sphereCollider.enabled = false;
        transform.LookAt(playerReference.transform);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_actualPointToReach, GoToPointQuest.distance);
    }
}