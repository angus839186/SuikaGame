using UnityEngine;

public class Fruit : MonoBehaviour
{
    public enum FruitState
    {
        Dropping,
        InThePool
    }

    [SerializeField] private string wallLayerName = "Wall";

    public FruitState State { get; private set; } = FruitState.Dropping;

    public bool IsInThePool => State == FruitState.InThePool;

    [SerializeField] private AudioClip groundHitClip;
    [SerializeField] private AudioClip fruitHitClip;

    [Range(0, 9)]
    public int tierIndex;

    private bool isMerging;

    public void SetInThePool()
    {
        State = FruitState.InThePool;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (State == FruitState.Dropping)
        {
            bool hitWall = collision.collider.gameObject.layer == LayerMask.NameToLayer(wallLayerName);
            bool hitFruit = collision.collider.GetComponent<Fruit>() != null;

            if (hitWall || hitFruit)
            {
                SetInThePool();
                if (AudioManager.instance != null)
                {
                    if (hitWall)
                        AudioManager.instance.PlaySound(fruitHitClip);
                    else if (hitFruit)
                        AudioManager.instance.PlaySound(groundHitClip);
                }
            }
        }

        if (GameManager.Instance == null) return;
        if (GameManager.Instance.GameEnd) return;

        var other = collision.collider.GetComponent<Fruit>();
        if (other == null) return;

        if (other.tierIndex != tierIndex) return;
        if (isMerging || other.isMerging) return;
        if (tierIndex >= 9) return;
        if (GetInstanceID() > other.GetInstanceID()) return;

        isMerging = true;
        other.isMerging = true;

        Vector3 spawnPos = (transform.position + other.transform.position) * 0.5f;
        GameManager.Instance.Merge(tierIndex, spawnPos, gameObject, other.gameObject);
    }
}