using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Image fillImage;

    [SerializeField] Color fullColor = Color.green;
    [SerializeField] Color lowColor = Color.red;

    public void SetHealth(float current, float max)
    {
        slider.value = current / max;
        fillImage.color = Color.Lerp(lowColor, fullColor, slider.value);
    }
}