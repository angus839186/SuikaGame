using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PhotoAnimationController : MonoBehaviour, IPointerClickHandler
{
    [Header("劇照群組")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Animator PhotoGroupAnimator;
    [Header("劇照動畫")]
    [SerializeField] private Animator PhotoAnimator;
    [Header("電視")]
    [SerializeField] private Animator TvAnimator;

    private bool isPhotoOpen;

    public event Action<bool> OnPhotoOpened;

    [SerializeField] private GameObject photoUiButton;

    [Header("劇照資料")]
    [SerializeField] private PhotoGroup[] photoGroups;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPhotoIndexChanged += HandlePhotoIndexChanged;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPhotoIndexChanged -= HandlePhotoIndexChanged;
        }
    }

    private void HandlePhotoIndexChanged(int index)
    {
    }

    public void TogglePhoto(bool toggle)
    {
        isPhotoOpen = toggle;
        if (isPhotoOpen)
        {
            PhotoGroupAnimator.ResetTrigger("Close");
            PhotoGroupAnimator.SetTrigger("Open");
            photoUiButton.SetActive(false);
        }
        else
        {
            PhotoGroupAnimator.ResetTrigger("Open");
            PhotoGroupAnimator.SetTrigger("Close");
            photoUiButton.SetActive(true);
        }
        OnPhotoOpened?.Invoke(isPhotoOpen);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isPhotoOpen)
        {
            TogglePhoto(false);
        }
    }



}