using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Range(0, 9)]
    public int tierIndex;

    public bool HasEnteredBucket { get; private set; } = false;

    private bool isMerging;

    public void MarkEnteredBucket()
    {
        HasEnteredBucket = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver) return;

        var other = collision.collider.GetComponent<Fruit>();
        if (other == null) return;

        if (other.tierIndex != tierIndex) return;

        if (isMerging || other.isMerging) return;

        if (tierIndex >= 9) return;

        if (GetInstanceID() > other.GetInstanceID()) return;

        isMerging = true;
        other.isMerging = true;

        Vector3 spawnPos = (transform.position + other.transform.position) * 0.5f;

        GameManager.Instance.Merge(tierIndex, spawnPos, this.gameObject, other.gameObject);
    }
}