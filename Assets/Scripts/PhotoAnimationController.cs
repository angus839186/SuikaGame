using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PhotoAnimationController : MonoBehaviour, IPointerClickHandler
{
    [Header("劇照群組")]
    [SerializeField] private Image backgroundImage;

    [SerializeField] private Image stampImage;
    [SerializeField] private Animator PhotoGroupAnimator;
    [Header("劇照動畫")]
    [SerializeField] private Animator PhotoAnimator;
    [Header("電視")]
    [SerializeField] private Animator TvAnimator;

    private bool isPhotoOpen;

    public event Action<int> OnPhotoOpened;

    [SerializeField] private GameObject photoUiButton;

    [Header("劇照資料")]
    [SerializeField] private PhotoGroup[] photoGroups;

    [SerializeField] private Sprite[] stampObjects;

    private string photoGroupAnimationParam = "PhotoIndex";
    private string photoAnimationParam = "PhotoIndex";
    private string tvAnimationParam = "PhotoIndex";

    private int currentPhotoGroupIndex = -1;

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
        if (stampImage != null && stampObjects != null && index >= 0 && index < stampObjects.Length)
        {
            stampImage.sprite = stampObjects[index];
        }

        if (photoGroups == null || photoGroups.Length == 0)
        {
            Debug.LogWarning("Photo groups not assigned.");
            return;
        }

        currentPhotoGroupIndex = UnityEngine.Random.Range(0, photoGroups.Length);
        PhotoGroup selectedGroup = photoGroups[currentPhotoGroupIndex];

        if (backgroundImage != null)
        {
            backgroundImage.sprite = GameManager.Instance.CurrentLanguage == GameManager.Language.Chinese
                ? selectedGroup.ChineseBackground
                : selectedGroup.EnglishBackground;
        }

        if (PhotoGroupAnimator != null)
        {
            PhotoGroupAnimator.SetInteger(photoGroupAnimationParam, selectedGroup.animationIndex);
        }

        if (PhotoAnimator != null)
        {
            PhotoAnimator.SetInteger(photoAnimationParam, selectedGroup.animationIndex);
        }

        if (TvAnimator != null)
        {
            TvAnimator.SetInteger(tvAnimationParam, selectedGroup.animationIndex);
        }
    }


    public void TogglePhoto(bool toggle)
    {
        isPhotoOpen = toggle;

        if (isPhotoOpen)
        {
            PhotoGroupAnimator.ResetTrigger("Close");
            PhotoGroupAnimator.SetTrigger("Open");
            photoUiButton.SetActive(false);

            if (currentPhotoGroupIndex >= 0)
            {
                OnPhotoOpened?.Invoke(currentPhotoGroupIndex);
            }
        }
        else
        {
            PhotoGroupAnimator.ResetTrigger("Open");
            PhotoGroupAnimator.SetTrigger("Close");
            photoUiButton.SetActive(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isPhotoOpen)
        {
            TogglePhoto(false);
        }
    }



}