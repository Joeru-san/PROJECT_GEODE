using System;
using UnityEngine;
using AdvancedProceduralSkybox;

public class DayNightController : MonoBehaviour
{
    public static Action OnDayStateChange;

    [Header("Light Sources")]
    public Light sun;
    public Light moon;

    [Header("Time Settings")]
    public float dayDurationSeconds = 240f;

    [Range(0f, 1f)]
    public float timeOfDay = 0f;

    /*
        Timeline:

        0.00 = Sunrise
        0.25 = Midday
        0.50 = Sunset
        0.75 = Midnight
        1.00 = Sunrise again
    */

    [Header("Day/Night Thresholds")]

    [Range(0f, 1f)]
    public float dayStartTime = 0f;

    [Range(0f, 1f)]
    public float nightStartTime = 0.5f;

    [Header("Environment Lighting")]
    public Gradient ambientColor;
    public Gradient fogColor;
    public Gradient skyboxTint;

    [Header("Moon Lighting")]
    public Gradient moonLightColor;

    [Header("Light Intensity")]
    public AnimationCurve sunIntensity;
    public AnimationCurve moonIntensity;

    [Header("Skybox Fog")]
    public Color skyboxFogColor = Color.grey;
    public float skyboxFogPower = 1f;
    public float skyboxFogOffset = 0f;

    [Header("Stars")]
    public Transform starsPivot;

    public static DayNightController inst;

    public static bool isNight = false;

    void Awake()
    {
        if (inst != null && inst != this)
        {
            Destroy(gameObject);
            return;
        }

        inst = this;
        DontDestroyOnLoad(gameObject);

        QuestManager.OnAllQuestEnded += ResetDay;
    }

    void Start()
    {
        isNight = timeOfDay >= nightStartTime;
        RenderSettings.sun = isNight ? moon : sun;

        UpdateLightRotations();
        UpdateLighting(timeOfDay);
    }

    void Update()
    {
        if (sun == null)
            return;

        UpdateTimeOfDay();
        UpdateLightRotations();
        UpdateLighting(timeOfDay);
        CheckDayNightState();
    }

    void UpdateTimeOfDay()
    {
        timeOfDay += Time.deltaTime / dayDurationSeconds;
        timeOfDay %= 1f;
    }

    void UpdateLightRotations()
    {
        float t = (timeOfDay + 0.5f) % 1f;

        float sunAngle = t * 360f;

        // FIXED MOON
        float moonAngle = sunAngle - 180f;

        sun.transform.rotation = Quaternion.Euler(
            sunAngle,
            170f,
            0f
        );

        if (moon != null)
        {
            moon.transform.rotation = Quaternion.Euler(
                moonAngle,
                170f,
                0f
            );
        }

        if (starsPivot != null)
        {
            starsPivot.rotation = Quaternion.Euler(
                0f,
                sunAngle,
                0f
            );
        }
    }

    void UpdateLighting(float t)
    {
        // Ambient & fog
        RenderSettings.ambientLight = ambientColor.Evaluate(t);
        RenderSettings.fogColor = fogColor.Evaluate(t);

        // Sun intensity
        if (sunIntensity != null)
        {
            sun.intensity = sunIntensity.Evaluate(t);
        }

        // Moon intensity & color
        if (moon != null)
        {
            if (moonIntensity != null)
            {
                moon.intensity = moonIntensity.Evaluate(t);
            }

            if (moonLightColor != null)
            {
                moon.color = moonLightColor.Evaluate(t);
            }
        }

        // Skybox
        Material skybox = RenderSettings.skybox;

        if (skybox != null)
        {
            // Sun direction
            SkyboxProperty.SetSunDirection(
                skybox,
                sun.transform.forward
            );

            // Moon matrix
            if (moon != null)
            {
                SkyboxProperty.SetMoonMatrix(
                    skybox,
                    moon.transform
                );
            }

            // Stars matrix
            if (starsPivot != null)
            {
                SkyboxProperty.SetStarMatrix(skybox, starsPivot);
            }

            // Fog
            SkyboxProperty.SetFogColor(skybox, skyboxFogColor);

            SkyboxProperty.SetFogPower(skybox, skyboxFogPower);

            SkyboxProperty.SetFogOffset(skybox, skyboxFogOffset);

            // Tint
            if (skybox.HasProperty("_Tint"))
            {
                skybox.SetColor(
                    "_Tint",
                    skyboxTint.Evaluate(t)
                );
            }
        }

        DynamicGI.UpdateEnvironment();
    }

    void CheckDayNightState()
    {
        bool shouldBeNight = timeOfDay >= nightStartTime;

        if (isNight == shouldBeNight)
            return;

        isNight = shouldBeNight;

        RenderSettings.sun = isNight ? moon : sun;

        OnDayStateChange?.Invoke();

        Debug.Log(
            $"[{GetType().Name}] Day state → " +
            $"{(isNight ? "Night" : "Day")}"
        );
    }

    public void ResetDay()
    {
        dayDurationSeconds = 240f;
        timeOfDay = dayStartTime;

        UpdateLightRotations();
        UpdateLighting(timeOfDay);
    }
}