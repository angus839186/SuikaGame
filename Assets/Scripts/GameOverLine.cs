using UnityEngine;

public class GameOverLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver) return;

        // 只要是水果碰到就結束
        if (other.GetComponent<Fruit>() != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}