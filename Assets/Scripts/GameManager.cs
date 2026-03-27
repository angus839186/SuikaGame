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
    [SerializeField] private int[] tierScores;

    [Header("劇照分數門檻")]

    [SerializeField]
    public int currentPhotoIndex;
    [SerializeField] private int[] photoScoreThresholds;

    public bool IsGameOver { get; private set; }

    public event Action<int> OnPhotoIndexChanged;

    public Language CurrentLanguage { get; private set; } = Language.Chinese;

    [Header("End UI")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject EndingEasterEgg;

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
        if (EndingEasterEgg != null)
        {
            EndingEasterEgg.SetActive(false);
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

    public void GameOver()
    {
        if (IsGameOver || IsGameWin) return;

        IsGameOver = true;

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        Debug.Log($"Game Over! Score={Score}");
    }

    public void GameWin()
    {
        if (IsGameOver || IsGameWin) return;

        IsGameWin = true;

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        if (EndingEasterEgg != null)
        {
            EndingEasterEgg.SetActive(true);
        }

        Debug.Log($"Game Win! Score={Score}");
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
            GameWin();
        }
    }

}

[Serializable]
public class ChinesePhoto
{
    public RuntimeAnimatorController photoanime;

    public Sprite backgroundSprite;
}

[Serializable]
public class EnglishPhoto
{
    public RuntimeAnimatorController photoanime;

    public Sprite bakcgroundSprite;
}
