using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private SuikaSpawner spawner;

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

        // 先刪舊的
        Destroy(a);
        Destroy(b);

        // 生成新的（下一級）
        spawner.SpawnSpecific(nextTierIndex, spawnPos);

        // 解鎖下一級生成
        spawner.UnlockTier(nextTierIndex);

        // 加分：合成出 B=4, C=8, ...
        int add = 1 << (nextTierIndex + 1);
        Score += add;

        // TODO: 你可以在這裡更新 UI
        // scoreText.text = Score.ToString();
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        // TODO: 顯示結算UI、停止Spawner等
        // spawner.enabled = false;
        Debug.Log($"Game Over! Score={Score}");
    }
}