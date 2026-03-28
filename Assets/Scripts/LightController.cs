using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using JetBrains.Annotations;

public class LightController : MonoBehaviour
{
    [SerializeField] GameManager gm;
    public PhotoAnimationController photoAnimationController;
    private Coroutine currentFadeCoroutine;

    [SerializeField] private GameLight sceneLight;
    [SerializeField] private GameLight highLight;
    [SerializeField] private GameLight groundLight;


    void Awake()
    {
        gm = GetComponent<GameManager>();
    }

    void OnEnable()
    {
        gm.GameEndAction += GameEndLight;
    }
    void OnDisable()
    {
        gm.GameEndAction -= GameEndLight;
    }
    void Start()
    {
        sceneLight.FadeLightIntensity(0, 1f);
    }
    public void OpenSceneLight()
    {
        sceneLight.FadeLightIntensity(1f, 1f);
    }

    public void GameEndLight()
    {
        sceneLight.FadeLightIntensity(0.05f, 0.2f);
        highLight.FadeLightIntensity(3f, 0.4f);
        groundLight.FadeLightIntensity(0.4f, 0.4f);
    }
}