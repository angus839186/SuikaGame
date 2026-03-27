using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private SuikaSpawner spawner;

    public TextMeshProUGUI scoreText;

    public TextMeshProUGUI photoScoreThresholdText;

    [Header("分數")]

    public int Score;

    [Header("門檻")]

    [SerializeField]
    public int currentPhotoIndex;
    [SerializeField] private int[] photoScoreThresholds;
    [SerializeField] private int[] tierScores;

    public bool IsGameOver { get; private set; }

    public Action<bool> GameResult;

    public event Action<int> OnPhotoIndexChanged;

    public Language CurrentLanguage { get; private set; } = Language.Chinese;

    public event Action<Language> OnLanguageChanged;

    [Header("End UI")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject winEasterEgg;
    [SerializeField] private GameObject loseEasterEgg;

    public bool IsGameWin { get; private set; }


    public enum Language
    {
        Chinese,
        English
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        UpdateScoreUI();
    }

    public void Merge(int currentTierIndex, Vector3 spawnPos, GameObject a, GameObject b)
    {
        if (IsGameOver || IsGameWin) return;


        int nextTierIndex = currentTierIndex + 1;

        Destroy(a);
        Destroy(b);

        GameObject newFruit = spawner.SpawnSpecific(nextTierIndex, spawnPos);
        Fruit fruit = newFruit.GetComponent<Fruit>();
        if (fruit != null)
        {
            fruit.SetInThePool();
        }
        spawner.UnlockTier(nextTierIndex);

        if (tierScores != null && nextTierIndex >= 0 && nextTierIndex < tierScores.Length)
            Score += tierScores[nextTierIndex];

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

    public void ToggleGameResult(bool toggle)
    {
        if (toggle)
        {
            if (IsGameOver || IsGameWin) return;

            IsGameWin = true;

            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
            }

            if (winEasterEgg != null)
            {
                winEasterEgg.SetActive(true);
            }
        }
        else
        {
            if (IsGameOver || IsGameWin) return;

            IsGameOver = true;

            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
            }
            if(loseEasterEgg != null)
            {
                loseEasterEgg.SetActive(true);
            }
            Debug.Log($"Game Over! Score={Score}");

        }
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

        if (currentPhotoIndex >= photoScoreThresholds.Length)
        {
            ToggleGameResult(true);
        }
    }
    public void SetLanguage(Language language)
    {
        if (CurrentLanguage == language) return;

        CurrentLanguage = language;
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

}
