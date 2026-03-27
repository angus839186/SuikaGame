using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

public class PhotoAnimationController : MonoBehaviour
{
    [Header("劇照群組")]
    [SerializeField] private Animator PhotoGroupAnimator;
    [SerializeField] private Animator PhotoAnimator;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image stampImage;

    [Header("電視")]
    [SerializeField] private Animator TvAnimator;

    private bool isPhotoOpen;

    private bool isGameStart;

    public event Action<int> OnPhotoOpened;

    public event Action<bool> OnPhotoToggled;

    [Header("UI")]
    [SerializeField] private GameObject photoUiButton;

    [Header("劇照資料")]
    [SerializeField] private PhotoGroup[] photoGroups;

    [SerializeField] private Sprite[] stampObjects;

    private int currentPhotoGroupIndex = -1;

    [Header("語言切換")]
    [SerializeField] Sprite ChineseTutorial;
    [SerializeField] Sprite EnglishTutorial;

    public UnityEvent EventAfterGameStart;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPhotoIndexChanged += HandlePhotoIndexChanged;
            GameManager.Instance.OnLanguageChanged += HandleLanguageChanged;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPhotoIndexChanged -= HandlePhotoIndexChanged;
            GameManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
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

        RefreshBackground();

        if (PhotoAnimator != null)
        {
            PhotoAnimator.gameObject.SetActive(true);
            PhotoAnimator.Play(selectedGroup.animationName, -1, 0f);
        }

        if (TvAnimator != null)
        {
            TvAnimator.Play(selectedGroup.animationName, -1, 0f);
        }
    }

    private void Start()
    {
        RefreshBackground();
    }
    public void HandleLanguageButtonClick()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ToggleLanguage();
        }
    }

    private void HandleLanguageChanged(GameManager.Language language)
    {
        RefreshBackground();
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
            if (isGameStart == false)
            {
                EventAfterGameStart.Invoke();
                isGameStart = true;
            }
        }
        OnPhotoToggled?.Invoke(toggle);
    }

    private void RefreshBackground()
    {
        if (backgroundImage == null || GameManager.Instance == null)
        {
            return;
        }

        bool isBeforeFirstPhotoChange = currentPhotoGroupIndex < 0;

        if (isBeforeFirstPhotoChange)
        {
            backgroundImage.sprite = GameManager.Instance.CurrentLanguage == GameManager.Language.Chinese
                ? ChineseTutorial
                : EnglishTutorial;

            return;
        }

        if (photoGroups == null || currentPhotoGroupIndex < 0 || currentPhotoGroupIndex >= photoGroups.Length)
        {
            return;
        }

        PhotoGroup selectedGroup = photoGroups[currentPhotoGroupIndex];
        backgroundImage.sprite = GameManager.Instance.CurrentLanguage == GameManager.Language.Chinese
            ? selectedGroup.ChineseBackground
            : selectedGroup.EnglishBackground;
    }




}