using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    public PhotoAnimationController photoAnimationController;
    private Coroutine currentFadeCoroutine;

    [SerializeField] private Light2D SceneLight;
    void Start()
    {
        FadeLightIntensity(SceneLight, 1f, 0f, 1f);
    }
    public void OpenSceneLight()
    {
        FadeLightIntensity(SceneLight, 0f, 1f, 1f);
    }

    public void FadeLightIntensity(Light2D targetLight, float fromIntensity, float toIntensity, float duration)
    {
        if (targetLight == null)
        {
            Debug.LogWarning("FadeLightIntensity failed: targetLight is null.");
            return;
        }

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeLightIntensityCoroutine(targetLight, fromIntensity, toIntensity, duration));
    }

    private IEnumerator FadeLightIntensityCoroutine(Light2D targetLight, float fromIntensity, float toIntensity, float duration)
    {
        targetLight.intensity = fromIntensity;

        if (duration <= 0f)
        {
            targetLight.intensity = toIntensity;
            currentFadeCoroutine = null;
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            targetLight.intensity = Mathf.Lerp(fromIntensity, toIntensity, t);
            yield return null;
        }

        targetLight.intensity = toIntensity;
        currentFadeCoroutine = null;
    }
}