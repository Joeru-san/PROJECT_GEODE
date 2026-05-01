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

    void Start()
    {
        InvokeRepeating("ChangeDayState", dayDurationSeconds / 2, dayDurationSeconds / 2);
    }

    void Update()
    {
        if (sun == null) return;

        // Increment timeOfDay based on the duration.
        // Time.deltaTime / dayDurationSeconds gives us the percentage of the day that has passed in this frame.
        timeOfDay += Time.deltaTime / dayDurationSeconds;
        timeOfDay %= 1; // Use modulo to wrap the value back to 0 when it reaches 1.

        // Rotate the sun based on the time of day.
        // timeOfDay * 360f converts our 0-1 value into a 0-360 degree rotation.
        sun.transform.rotation = Quaternion.Euler(timeOfDay * 360f, 0, 0);

        // Update the environment lighting based on the gradients.
        UpdateLighting(timeOfDay);
    }

    private void UpdateLighting(float currentTime)
    {
        // Sample the gradients at the current time of day.
        RenderSettings.ambientLight = ambientColor.Evaluate(currentTime);
        RenderSettings.fogColor = fogColor.Evaluate(currentTime);

        // Update the procedural skybox tint if the skybox material is assigned.
        if (RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_Tint"))
        {
            RenderSettings.skybox.SetColor("_Tint", skyboxTint.Evaluate(currentTime));
        }

        // Tell Unity's GI to update with the new environmental lighting.
        DynamicGI.UpdateEnvironment();
    }

    void ChangeDayState()
    {
        isNight = !isNight;
        if(isNight) OnDayStateChange?.Invoke();
        Debug.Log($"[{GetType().Name}] Changed day state in: " + (isNight ? "night" : "day"));
    }
}
