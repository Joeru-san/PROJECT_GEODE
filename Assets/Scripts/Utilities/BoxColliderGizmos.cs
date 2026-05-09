#if UNITY_EDITOR
using UnityEngine;
public class BoxColliderGizmos : MonoBehaviour
{
    public Color gizmoColor = Color.green;

    void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;
        Gizmos.color = gizmoColor;
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(box.center, box.size);
        Gizmos.matrix = old;
        Gizmos.color = Color.white;
    }
}
#endif