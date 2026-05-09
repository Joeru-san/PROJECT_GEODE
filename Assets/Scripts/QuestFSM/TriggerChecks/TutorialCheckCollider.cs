using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SphereCollider))]
public class CompanionCheckPlayerInCollider : MonoBehaviour
{
    public static bool isPlayerInCollider = false;

    public GameObject playerReference;
    public GameObject companionReference;


    void Start()
    {
        if(!isPlayerInCollider)
        {
            Debug.Log($"[{GetType().Name}] moving close to player");

            MoveTowardsPlayer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isPlayerInCollider = true;
            Debug.Log($"[{GetType().Name}] player entered the trigger");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isPlayerInCollider = false ;

            Debug.Log($"[{GetType().Name}] player left the trigger");
            MoveTowardsPlayer();
        }       
    }

    public bool MoveTowardsPlayer()
    {
        if(playerReference != null && companionReference != null && isPlayerInCollider)
        {
            Vector3 direction = (companionReference.transform.position - playerReference.transform.position).normalized;
            Vector3 targetPos = playerReference.transform.position + direction * 1f;
            companionReference.transform.DOMove(targetPos, 2f);
            return true;
        } else
        {
            return false;
        }
    }
}
