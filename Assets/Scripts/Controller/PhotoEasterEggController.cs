using System;
using UnityEngine;

public class PhotoEasterEggController : MonoBehaviour
{
    [SerializeField] private PhotoAnimationController photoAnimationController;
    [SerializeField] private GameObject[] easterEggs;

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

        GameObject easterEgg = easterEggs[index];
        if (easterEgg != null)
        {
            easterEgg.SetActive(true);
        }
    }
}
