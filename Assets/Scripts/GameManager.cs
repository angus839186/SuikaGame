using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum Language
    {
        Chinese,
        English
    }
    [Header("Reference")]
    public static GameManager Instance { get; private set; }
    [SerializeField] private FruitSpawner spawner;

    [SerializeField] private AudioClip mergeClip;

    [Header("分數")]
    public TextMeshProUGUI scoreText;
    public int Score;

    [Header("門檻")]
    public TextMeshProUGUI photoScoreThresholdText;
    public int currentPhotoIndex;
    [SerializeField] private int[] photoScoreThresholds;
    [SerializeField] private int[] tierScores;
    public event Action<int> OnPhotoIndexChanged;

    public bool GameEnd;

    public bool IsGameWin;

    public Action<bool> GameResultAction;

    [SerializeField] UnityEvent EventAfterGameEnd;

    [Header("語言")]
    public Language CurrentLanguage { get; private set; } = Language.Chinese;
    public event Action<Language> OnLanguageChanged;
    public AudioClip switchLanguageClip;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public void Merge(int currentTierIndex, Vector3 spawnPos, Fruit a, Fruit b)
    {
        if (GameEnd) return;

        int nextTierIndex = currentTierIndex + 1;

        if (a != null) a.ReturnToPool();
        if (b != null) b.ReturnToPool();

        // if (effectPoolManager != null)
        // {
        //     effectPoolManager.PlayEffect(spawnPos);
        // }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(mergeClip);
        }

        spawner.SpawnSpecific(nextTierIndex, spawnPos);

        spawner.UnlockTier(nextTierIndex);

        if (tierScores != null && nextTierIndex >= 0 && nextTierIndex < tierScores.Length)
            Score += tierScores[nextTierIndex];

        UpdateScoreUI();
        TryAdvancePhotoIndex();
    }

    public void MergeMaxTier(Vector3 spawnPos, Fruit a, Fruit b)
    {
        if (GameEnd) return;

        if (a != null) a.ReturnToPool();
        if (b != null) b.ReturnToPool();

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(mergeClip);
        }

        UpdateScoreUI();
        TryAdvancePhotoIndex();
    }


    public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = Score.ToString();
        }

        if (photoScoreThresholdText != null)
        {
            if (photoScoreThresholds != null && currentPhotoIndex < photoScoreThresholds.Length)
            {
                int nextThreshold = photoScoreThresholds[currentPhotoIndex];
                photoScoreThresholdText.text = Score + "/" + nextThreshold;
            }
            else
            {
                photoScoreThresholdText.text = Score.ToString();
            }
        }
    }

    public void EndGame(bool isWin)
    {
        if (GameEnd) return;

        GameEnd = true;
        IsGameWin = isWin;

        GameResultAction?.Invoke(IsGameWin);
        EventAfterGameEnd?.Invoke();
    }


    private void TryAdvancePhotoIndex()
    {
        if (photoScoreThresholds == null || photoScoreThresholds.Length == 0) return;

        bool photoIndexChanged = false;

        while (currentPhotoIndex < photoScoreThresholds.Length && Score >= photoScoreThresholds[currentPhotoIndex])
        {
            currentPhotoIndex++;
            photoIndexChanged = true;
            OnPhotoIndexChanged?.Invoke(currentPhotoIndex);
        }

        if (photoIndexChanged)
        {
            UpdateScoreUI();
        }

    }

    public bool IsPhotoCompleted()
    {
        return photoScoreThresholds != null
            && photoScoreThresholds.Length > 0
            && currentPhotoIndex >= photoScoreThresholds.Length;
    }
    public void SetLanguage(Language language)
    {
        if (CurrentLanguage == language) return;

        CurrentLanguage = language;
        AudioManager.instance.PlaySound(switchLanguageClip);
        OnLanguageChanged?.Invoke(CurrentLanguage);
    }

    public void ToggleLanguage()
    {
        if (CurrentLanguage == Language.Chinese)
        {
            SetLanguage(Language.English);
        }
        else
        {
            SetLanguage(Language.Chinese);
        }
    }

    public void PlayAgain()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GameEnd) return;

        const float buttonWidth = 160f;
        const float buttonHeight = 50f;
        const float startX = 540f;
        const float startY = 20f;
        const float spacingY = 10f;

        if (GUI.Button(new Rect(startX, startY, buttonWidth, buttonHeight), "Test Win"))
        {
            EndGame(true);
        }

        if (GUI.Button(new Rect(startX, startY + buttonHeight + spacingY, buttonWidth, buttonHeight), "Test Lose"))
        {
            EndGame(false);
        }
    }
#endif


}
