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
    public float timeOfDay;

    [Header("Day/Night Thresholds")]
    public float nightStartTime = 0.7f;
    public float dayStartTime = 0.2f;

    [Header("Sun Lighting")]
    public Gradient ambientColor;
    public Gradient fogColor;
    public Gradient skyboxTint;

    [Header("Moon Lighting")]
    public Gradient moonLightColor;
    public AnimationCurve sunIntensity;
    public AnimationCurve moonIntensity;

    [Header("Skybox Fog")]
    public Color skyboxFogColor = Color.grey;
    public float skyboxFogPower = 1f;
    public float skyboxFogOffset = 0f;

    public static DayNightController inst;

    [Header("Stars")]
    public Transform starsPivot; // Empty GameObject for star rotation

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

        QuestManager.OnQuestEnded += ResetDay;
    }

    public void ResetDay()
    {
        dayDurationSeconds = 240;
        timeOfDay = 0.4f;
    }

    void Start()
    {
        isNight = timeOfDay >= nightStartTime || timeOfDay < dayStartTime;
        RenderSettings.sun = isNight ? moon : sun; // make sure sun source matches starting time
    }
    
    void Update()
    {
        if (sun == null) return;

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
        float dayLength = nightStartTime - dayStartTime; // 0.5
        float dayProgress = (timeOfDay - dayStartTime) / dayLength; // 0→1 during day

        // Sun: -90° at sunrise, 90° at noon, 270° at sunset (below horizon at night)
        float sunAngle  = dayProgress * 180f - 90f;
        float moonAngle = sunAngle + 180f;

        sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

        if (moon != null)
            moon.transform.rotation = Quaternion.Euler(moonAngle, 170f, 0f);

        if (starsPivot != null)
            starsPivot.Rotate(Vector3.up, Time.deltaTime * 0.5f);
    }

    void UpdateLighting(float t)
    {
        RenderSettings.ambientLight = ambientColor.Evaluate(t);
        RenderSettings.fogColor     = fogColor.Evaluate(t);

        if (sunIntensity != null)
            sun.intensity = sunIntensity.Evaluate(t);

        if (moon != null)
        {
            if (moonIntensity != null)
                moon.intensity = moonIntensity.Evaluate(t);
            if (moonLightColor != null)
                moon.color = moonLightColor.Evaluate(t);
        }

        Material skybox = RenderSettings.skybox;
        if (skybox != null)
        {
            // Sun direction
            SkyboxProperty.SetSunDirection(skybox, sun.transform.forward);

            // Moon matrix
            if (moon != null)
                SkyboxProperty.SetMoonMatrix(skybox, moon.transform);

            // Stars matrix
            if (starsPivot != null)
                SkyboxProperty.SetStarMatrix(skybox, starsPivot);

            // Fog
            SkyboxProperty.SetFogColor(skybox, skyboxFogColor);
            SkyboxProperty.SetFogPower(skybox, skyboxFogPower);
            SkyboxProperty.SetFogOffset(skybox, skyboxFogOffset);

            // Tint fallback for non-package skyboxes
            if (skybox.HasProperty("_Tint"))
                skybox.SetColor("_Tint", skyboxTint.Evaluate(t));
        }

        DynamicGI.UpdateEnvironment();
    }

    void CheckDayNightState()
    {
        bool shouldBeNight = timeOfDay >= nightStartTime || timeOfDay < dayStartTime;

        if (isNight == shouldBeNight) return;

        isNight = shouldBeNight;
        RenderSettings.sun = isNight ? moon : sun;
        OnDayStateChange?.Invoke();

        Debug.Log($"[{GetType().Name}] Day state → {(isNight ? "Night" : "Day")}");
    }
}