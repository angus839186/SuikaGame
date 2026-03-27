using System;
using UnityEngine;

public class PhotoEasterEggController : MonoBehaviour
{
    [SerializeField] private PhotoAnimationController photoAnimationController;
    [SerializeField] private EasterEgg[] easterEggs;

    private void OnEnable()
    {
        if (photoAnimationController != null)
        {
            photoAnimationController.OnPhotoOpened += HandlePhotoOpened;
        }
    }

    private void OnDisable()
    {
        if (photoAnimationController != null)
        {
            photoAnimationController.OnPhotoOpened -= HandlePhotoOpened;
        }
    }

    private void HandlePhotoOpened(int index)
    {
        if (easterEggs == null || index < 0 || index >= easterEggs.Length)
        {
            return;
        }

        EasterEgg easterEgg = easterEggs[index];
        if (easterEgg.instance != null)
        {
            easterEgg.instance.SetActive(true);
            easterEgg.isActive = true;
        }
    }
}

[Serializable]
public class EasterEgg
{
    public GameObject instance;
    public bool isActive;
}
