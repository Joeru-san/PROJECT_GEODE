#if UNITY_EDITOR
using UnityEngine;

public class ColliderGizmos : MonoBehaviour
{
    void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.color = Color.green;
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(box.center, box.size);
        Gizmos.matrix = old;
        Gizmos.color = Color.white;
    }
}
#endif
