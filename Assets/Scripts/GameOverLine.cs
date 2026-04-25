using System.Collections.Generic;
using UnityEngine;

public class GameOverLine : MonoBehaviour
{
    [SerializeField] private float gameOverDelay = 1.0f;

    private readonly Dictionary<Fruit, float> fruitTimers = new();

    private void OnTriggerStay2D(Collider2D other)
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.GameEnd) return;

        Fruit fruit = other.GetComponent<Fruit>();
        if (fruit == null) return;
        if (!fruit.IsInThePool) return;

        if (!fruitTimers.ContainsKey(fruit))
        {
            fruitTimers[fruit] = 0f;
        }

        fruitTimers[fruit] += Time.deltaTime;

        if (fruitTimers[fruit] >= gameOverDelay)
        {
            GameManager.Instance.EndGame(GameManager.Instance.IsPhotoCompleted());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Fruit fruit = other.GetComponent<Fruit>();
        if (fruit == null) return;

        fruitTimers.Remove(fruit);
    }
}
