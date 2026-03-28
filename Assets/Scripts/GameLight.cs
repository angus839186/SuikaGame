using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using JetBrains.Annotations;

public class GameLight : MonoBehaviour
{
    private Coroutine currentFadeCoroutine;

    [SerializeField] private Light2D Light;

    public void FadeLightIntensity(float toIntensity, float duration)
    {
        if (Light == null)
        {
            Debug.LogWarning("FadeLightIntensity failed: light2D is null.");
            return;
        }

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(
            FadeLightIntensityCoroutine(Light.intensity, toIntensity, duration)
        );
    }

    private IEnumerator FadeLightIntensityCoroutine(float fromIntensity, float toIntensity, float duration)
    {
        Light.intensity = fromIntensity;

        if (duration <= 0f)
        {
            Light.intensity = toIntensity;
            currentFadeCoroutine = null;
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            Light.intensity = Mathf.Lerp(fromIntensity, toIntensity, t);
            yield return null;
        }

        Light.intensity = toIntensity;
        currentFadeCoroutine = null;
    }

}