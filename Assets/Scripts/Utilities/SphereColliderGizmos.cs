#if UNITY_EDITOR
using UnityEngine;
public class SphereColliderGizmos : MonoBehaviour
{
    public Color gizmoColor = Color.green;

    void OnDrawGizmos()
    {
        SphereCollider sphere = GetComponent<SphereCollider>();
        if (sphere == null) return;
        Gizmos.color = gizmoColor;
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(sphere.center, sphere.radius);
        Gizmos.matrix = old;
        Gizmos.color = Color.white;
    }
}
#endif