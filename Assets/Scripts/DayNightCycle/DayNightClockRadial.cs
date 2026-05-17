using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayNightClockRadial : MonoBehaviour
{
    [Header("Radial Fill")]
    [SerializeField] Image radialFill;

    [Header("Celestial Body")]
    [SerializeField] RectTransform orbitPivot; 
    [SerializeField] Image celestialIcon;
    [SerializeField] Sprite sunSprite;
    [SerializeField] Sprite moonSprite;

    [Header("Colors")]
    [SerializeField] Color dayColor;
    [SerializeField] Color nightColor;
    [SerializeField] Color sunriseColor;

    void Update()
    {
        if (DayNightController.inst == null) return;

        float t = DayNightController.inst.timeOfDay;
        float dayStart   = DayNightController.inst.dayStartTime;
        float nightStart = DayNightController.inst.nightStartTime;

        radialFill.fillAmount = t;

        bool isNight      = t >= nightStart || t < dayStart;
        bool isTransition = Mathf.Abs(t - dayStart)   < 0.05f
                         || Mathf.Abs(t - nightStart) < 0.05f;

        radialFill.color = isTransition ? sunriseColor
                         : isNight      ? nightColor
                                        : dayColor;

        orbitPivot.localRotation = Quaternion.Euler(0f, 0f, -t * 360f);

        celestialIcon.sprite = isNight ? moonSprite : sunSprite;

        celestialIcon.rectTransform.localRotation = Quaternion.Euler(0f, 0f, t * 360f);
    }
}