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
    [SerializeField] private SuikaSpawner spawner;

    [SerializeField] private GameObject mergeEffectPrefab;

    [SerializeField] private AudioClip mergeClip;

    [Header("分數")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] int Score;

    [Header("門檻")]
    [SerializeField] TextMeshProUGUI photoScoreThresholdText;
    [SerializeField] int currentPhotoIndex;
    [SerializeField] private int[] photoScoreThresholds;
    [SerializeField] private int[] tierScores;
    public event Action<int> OnPhotoIndexChanged;

    [Header("遊戲結束")]
    [SerializeField] private GameObject PlayAgainButton;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject winEasterEgg;
    [SerializeField] private GameObject loseEasterEgg;

    public bool GameEnd { get; private set; }

    public bool IsGameWin { get; private set; }

    public Action<bool> GameResultAction;

    public Action GameEndAction;

    [SerializeField] UnityEvent EventAfterGameEnd;

    [Header("語言")]
    public Language CurrentLanguage { get; private set; } = Language.Chinese;
    public event Action<Language> OnLanguageChanged;

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
        if (GameEnd) return;

        int nextTierIndex = currentTierIndex + 1;

        Destroy(a);
        Destroy(b);

        if (mergeEffectPrefab != null)
        {
            Instantiate(mergeEffectPrefab, spawnPos, Quaternion.identity);
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(mergeClip);
        }

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

    public void EndGame(bool isWin)
    {
        if (GameEnd) return;

        GameEnd = true;
        IsGameWin = isWin;
        StartCoroutine(GameEndCoroutine(isWin));
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
            EndGame(true);
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

    public void PlayAgain()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    IEnumerator GameEndCoroutine(bool result)
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        GameEndAction.Invoke();
        ToggleEasterEgg(result);
        Debug.Log($"Game Over! Score={Score}");
        EventAfterGameEnd.Invoke();
        yield return new WaitForSeconds(2f);
        PlayAgainButton.SetActive(true);
        yield return null;
    }

    public void ToggleEasterEgg(bool toggle)
    {
        if (toggle)
        {
            if (winEasterEgg != null)
            {
                winEasterEgg.SetActive(true);
            }
        }
        else
        {
            if (loseEasterEgg != null)
            {
                loseEasterEgg.SetActive(true);
            }
        }
    }

}
