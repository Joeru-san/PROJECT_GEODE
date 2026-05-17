using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;

public class TurretIcon : MonoBehaviour
{
    [SerializeField] Image refImage;
    [SerializeField] DefenseTurret turretReference;


    void Update()
    {
        if(turretReference.isAttacking) // Under attack
        {
            refImage.DOFade(1f, 0.3f);
            refImage.gameObject.transform.DOScale(1.2f, 0.3f).SetLoops(-1, LoopType.Yoyo);
        }else   // Safe
        {
            refImage.DOFade(0f, 0.3f);
            refImage.gameObject.transform.DOKill();
            refImage.gameObject.transform.DOScale(Vector3.one, 0.3f);
        }
    }
}
