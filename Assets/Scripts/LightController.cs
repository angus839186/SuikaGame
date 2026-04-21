using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] private GameLight sceneLight;
    [SerializeField] private GameLight highLight;
    [SerializeField] private GameLight groundLight;

    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip closeClip;

    private void Start()
    {
        StartLight();
    }

    public void StartLight()
    {
        if (sceneLight != null) sceneLight.SetLight(0.05f);
        if (highLight != null) highLight.SetLight(0f);
        if (groundLight != null) groundLight.SetLight(0f);
        AudioManager.instance.PlaySound(closeClip);
    }

    public void GameStartLight()
    {
        if (sceneLight != null) sceneLight.SetLight(1f);
        if (highLight != null) highLight.SetLight(0f);
        if (groundLight != null) groundLight.SetLight(0f);
        AudioManager.instance.PlaySound(openClip);
    }

    public void CloseAllLight()
    {
        if (sceneLight != null) sceneLight.SetLight(0f);
        if (highLight != null) highLight.SetLight(0f);
        if (groundLight != null) groundLight.SetLight(0f);
        AudioManager.instance.PlaySound(closeClip);
    }

    public void EndGameLight()
    {
        if (sceneLight != null) sceneLight.SetLight(0.05f);
        if (highLight != null) highLight.SetLight(0.6f);
        if (groundLight != null) groundLight.SetLight(0.6f);
        AudioManager.instance.PlaySound(openClip);
    }
}
