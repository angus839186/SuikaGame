using UnityEngine;

public class GameOverLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.GameEnd) return;


        var fruit = other.GetComponent<Fruit>();
        if (fruit == null) return;

        if (fruit.IsInThePool)
        {
            if (GameManager.Instance.IsPhotoCompleted())
            {
                GameManager.Instance.EndGame(true);
            }
            else
            {
                GameManager.Instance.EndGame(false);
            }
        }
    }
}
