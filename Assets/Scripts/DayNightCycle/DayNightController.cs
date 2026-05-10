using System;
using UnityEngine;

public class DayNightController : MonoBehaviour
{
    public static Action OnDayStateChange;
    public Light sun;
    public float dayDurationSeconds = 240f;

    [Header("Lighting Settings")]
    public Gradient ambientColor;   // Controls the color of the ambient light throughout the day
    public Gradient fogColor;   // Controls the color of the scene's fog throughout the day
    public Gradient skyboxTint; // Controls the tint of the procedural skybox throughout the day

    public float timeOfDay; // A value from 0 (midnight) to 1 (next midnight).
    public static bool isNight = false;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    [Header("Day/Night Thresholds")]
    public float nightStartTime = 0.7f;
    public float dayStartTime = 0.2f;

    void Start()
    {
        isNight = timeOfDay >= nightStartTime || timeOfDay < dayStartTime;
        timeOfDay = dayStartTime + 0.2f;
    }

    void Update()
    {
        if (sun == null) return;

        UpdateTimeOfDay();
        UpdateSunRotation();
        UpdateLighting(timeOfDay);
        CheckDayNightState();
    }

    private void UpdateTimeOfDay()
    {
        timeOfDay += Time.deltaTime / dayDurationSeconds;
        timeOfDay %= 1f;
    }

    private void UpdateSunRotation()
    {
        float sunRotationAngle = timeOfDay * 360f;
        sun.transform.rotation = Quaternion.Euler(sunRotationAngle, 0f, 0f);
    }

    private void UpdateLighting(float currentTime)
    {
        RenderSettings.ambientLight = ambientColor.Evaluate(currentTime);
        RenderSettings.fogColor = fogColor.Evaluate(currentTime);

        if (RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_Tint"))
        {
            RenderSettings.skybox.SetColor("_Tint", skyboxTint.Evaluate(currentTime));
        }

        DynamicGI.UpdateEnvironment();
    }

    private void CheckDayNightState()
    {
        bool shouldBeNight = timeOfDay >= nightStartTime || timeOfDay < dayStartTime;

        if (isNight != shouldBeNight)
        {
            isNight = shouldBeNight;
            if (isNight) OnDayStateChange?.Invoke();
            Debug.Log($"[{GetType().Name}] Changed day state in: " + (isNight ? "night" : "day"));
        }
    }
}
