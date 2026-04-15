using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PhotoAnimationController : MonoBehaviour
{
    [Header("劇照群組")]
    [SerializeField] private Animator PhotoGroupAnimator;
    [SerializeField] private Animator PhotoAnimator;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image stampImage;

    [Header("電視")]
    [SerializeField] private Animator TvAnimator;
    private Coroutine tvPlayCoroutine;
    [SerializeField] private string tvLoadingAnimationName = "Loading";

    [SerializeField] private AudioClip LoadingClip;

    [SerializeField] GameObject RedButton;

    private bool isPhotoOpen;

    private bool isGameStart;

    private bool NewPhotoGroup;

    public event Action<int> OnPhotoOpened;

    public event Action<bool> OnPhotoToggled;

    [Header("UI")]
    [SerializeField] private GameObject photoUiButton;

    [Header("劇照資料")]
    [SerializeField] private PhotoGroup[] photoGroups;

    [SerializeField] private Sprite[] stampObjects;

    public int currentPhotoGroupIndex = -1;
    private List<int> remainingPhotoGroupIndices = new List<int>();

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
    private void Start()
    {
        InitializeRemainingPhotoGroups();
        RefreshBackground();
    }
    private void HandlePhotoIndexChanged(int index)
    {
        if (stampImage != null && stampObjects != null && index >= 0 && index < stampObjects.Length)
        {
            stampImage.gameObject.SetActive(true);
            stampImage.sprite = stampObjects[index-1];
        }

        if (photoGroups == null || photoGroups.Length == 0)
        {
            Debug.LogWarning("Photo groups not assigned.");
            return;
        }

        if (remainingPhotoGroupIndices.Count == 0)
        {
            Debug.LogWarning("No unused photo groups left.");
            return;
        }

        int randomPoolIndex = UnityEngine.Random.Range(0, remainingPhotoGroupIndices.Count);
        currentPhotoGroupIndex = remainingPhotoGroupIndices[randomPoolIndex];
        remainingPhotoGroupIndices.RemoveAt(randomPoolIndex);

        string animationStateName = (currentPhotoGroupIndex+1).ToString(); 

        RefreshBackground();

        if (PhotoAnimator != null)
        {
            PhotoAnimator.gameObject.SetActive(true);
            PhotoAnimator.Play(animationStateName, -1, 0f);
        }

        if (TvAnimator != null)
        {
            if (tvPlayCoroutine != null)
            {
                StopCoroutine(tvPlayCoroutine);
            }

            tvPlayCoroutine = StartCoroutine(PlayTvAnimationWithLoading(animationStateName));
        }

        NewPhotoGroup = true;
        ToggleRedButton();
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
            NewPhotoGroup = false;
            ToggleRedButton();

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
        OnPhotoToggled?.Invoke(!toggle);
    }

    void ToggleRedButton()
    {
        bool opened = NewPhotoGroup;
        RedButton.SetActive(opened);
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

    private IEnumerator PlayTvAnimationWithLoading(string targetAnimationName)
    {
        TvAnimator.Play(tvLoadingAnimationName, -1, 0f);
        AudioManager.instance.PlaySound(LoadingClip);
        yield return null;

        AnimatorStateInfo loadingStateInfo = TvAnimator.GetCurrentAnimatorStateInfo(0);
        float loadingDuration = loadingStateInfo.length;

        yield return new WaitForSeconds(loadingDuration);

        TvAnimator.Play(targetAnimationName, -1, 0f);
        tvPlayCoroutine = null;
    }


    private void InitializeRemainingPhotoGroups()
    {
        remainingPhotoGroupIndices.Clear();

        if (photoGroups == null)
        {
            return;
        }

        for (int i = 0; i < photoGroups.Length; i++)
        {
            remainingPhotoGroupIndices.Add(i);
        }
    }


}