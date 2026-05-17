using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TurretIcon : MonoBehaviour
{
    [SerializeField] Image refImage;
    [SerializeField] DefenseTurret turretReference;

    bool _wasAttacking;

    void Update()
    {
        float targetAlpha = turretReference.enemySpotted ? turretReference.currentHealth / turretReference.MaxHealth : 0f;
        refImage.DOFade(targetAlpha, 0.3f);

        if (turretReference.enemySpotted == _wasAttacking) return;
        _wasAttacking = turretReference.enemySpotted;

        refImage.transform.DOKill();

        if (turretReference.enemySpotted)
            refImage.transform.DOScale(1.2f, 0.3f).SetLoops(-1, LoopType.Yoyo);
        else
            refImage.transform.DOScale(1f, 0.3f);
    }
}