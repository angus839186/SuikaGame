using UnityEngine;

public class InBucketLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var fruit = other.GetComponent<Fruit>();
        if (fruit != null)
        {
            fruit.MarkEnteredBucket();
        }
    }
}