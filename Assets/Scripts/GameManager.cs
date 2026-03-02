using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private SuikaSpawner spawner;

    public Text scoreText;

    [Header("水果分數")]
    [SerializeField] private int[] tierScores = new int[10];

    public int Score { get; private set; }
    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Merge(int currentTierIndex, Vector3 spawnPos, GameObject a, GameObject b)
    {
        if (IsGameOver) return;

        int nextTierIndex = currentTierIndex + 1;

        Destroy(a);
        Destroy(b);

        spawner.SpawnSpecific(nextTierIndex, spawnPos);
        spawner.UnlockTier(nextTierIndex);

        // ✅ 自訂分數：合成出 nextTierIndex 就加那一格
        if (tierScores != null && nextTierIndex >= 0 && nextTierIndex < tierScores.Length)
            Score += tierScores[nextTierIndex];

        UpdateScoreUI();
    }

    public void UpdateScoreUI()
    {
        scoreText.text = Score.ToString();
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Debug.Log($"Game Over! Score={Score}");
    }
}