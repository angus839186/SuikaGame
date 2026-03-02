using UnityEngine;

public class GameOverLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver) return;

        var fruit = other.GetComponent<Fruit>();
        if (fruit == null) return;

        // ✅ 只有掉進池子後，堆回來碰到線才算 GameOver
        if (fruit.HasEnteredBucket)
        {
            GameManager.Instance.GameOver();
        }
    }
}