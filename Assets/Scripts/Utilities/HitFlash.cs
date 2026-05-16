using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] float flashDuration = 0.2f;
    [SerializeField] Color flashColor;

    Renderer[] renderers;
    Color[] originalColors;
    Coroutine flashCoroutine;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].material.color;
    }

    public void Flash()
    {
        if (flashCoroutine != null && gameObject.activeInHierarchy)
            StopCoroutine(flashCoroutine);

        if(gameObject.activeInHierarchy) flashCoroutine = StartCoroutine(DoFlash());
    }

    public void ResetColors()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];
    }

    private IEnumerator DoFlash()
    {
        float halfDuration = flashDuration / 2f;
        float elapsed = 0f;

        // Fade IN
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;

            for (int i = 0; i < renderers.Length; i++)
                renderers[i].material.color = Color.Lerp(originalColors[i], flashColor, t);

            yield return null;
        }

        elapsed = 0f;

        // Fade OUT
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;

            for (int i = 0; i < renderers.Length; i++)
                renderers[i].material.color = Color.Lerp(flashColor, originalColors[i], t);

            yield return null;
        }

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];
    }
}