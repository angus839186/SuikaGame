using UnityEngine;

public class GameOverLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGameWin) return;


        var fruit = other.GetComponent<Fruit>();
        if (fruit == null) return;

        if (fruit.IsInThePool)
        {
            GameManager.Instance.GameOver();
        }
    }
}
