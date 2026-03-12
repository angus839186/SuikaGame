using System;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private SuikaSpawner spawner;

    public Text scoreText;

    [Header("Score")]
    [SerializeField] private int[] tierScores = new int[10];

    public int Score { get; private set; }
    public bool IsGameOver { get; private set; }
    public int photoIndex { get; private set; }

    [Header("劇照分數門檻")]
    [SerializeField] private int[] photoScoreThresholds;

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
        OnPhotoIndexChanged?.Invoke(photoIndex);
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

        while (photoIndex < photoScoreThresholds.Length && Score >= photoScoreThresholds[photoIndex])
        {
            photoIndex++;
            OnPhotoIndexChanged?.Invoke(photoIndex);
        }
    }
}

[Serializable]
public class ChinesePhoto
{
    public AnimatorController photoanime;
    public Sprite upperSprite;
    public Sprite lowerSprite;
}

[Serializable]
public class EnglishPhoto
{
    public AnimatorController photoanime;
    public Sprite upperSprite;
    public Sprite lowerSprite;
}
