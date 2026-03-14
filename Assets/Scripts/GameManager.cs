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
    [SerializeField] private int[] tierScores = new int[10];

    [Header("劇照分數門檻")]

    [SerializeField]
    public int currentPhotoIndex;
    [SerializeField] private int[] photoScoreThresholds;

    public bool IsGameOver { get; private set; }

    public event Action<int> OnPhotoIndexChanged;

    public Language CurrentLanguage { get; private set; } = Language.Chinese;

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
        UpdateScoreUI();
    }

    public void Merge(int currentTierIndex, Vector3 spawnPos, GameObject a, GameObject b)
    {
        if (IsGameOver) return;

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
        if (IsGameOver) return;
        IsGameOver = true;
        Debug.Log($"Game Over! Score={Score}");
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
}

[Serializable]
public class ChinesePhoto
{
    public RuntimeAnimatorController photoanime;
    public Sprite upperSprite;
    public Sprite lowerSprite;
}

[Serializable]
public class EnglishPhoto
{
    public RuntimeAnimatorController photoanime;
    public Sprite upperSprite;
    public Sprite lowerSprite;
}
