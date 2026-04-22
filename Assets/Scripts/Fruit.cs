using UnityEngine;

public class Fruit : MonoBehaviour
{
    public enum FruitState
    {
        Dropping,
        InThePool
    }

    [SerializeField] private string wallLayerName = "Wall";
    [SerializeField] private AudioClip groundHitClip;
    [SerializeField] private AudioClip fruitHitClip;

    [Range(0, 9)]
    public int tierIndex;
    private const int MaxTier = 9;


    public FruitState State { get; private set; } = FruitState.Dropping;
    public bool IsInThePool => State == FruitState.InThePool;

    public int PoolTierIndex { get; private set; }

    private bool isMerging;
    private Rigidbody2D rb;
    private FruitPoolManager poolManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetPool(FruitPoolManager manager, int poolTierIndex)
    {
        poolManager = manager;
        PoolTierIndex = poolTierIndex;
    }

    public void ResetAsHeld(Vector3 position)
    {
        isMerging = false;
        State = FruitState.Dropping;

        transform.position = position;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }
    }

    public void ResetAsDropped(Vector3 position, bool useContinuousCollision = false)
    {
        isMerging = false;
        State = FruitState.Dropping;

        transform.position = position;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            rb.collisionDetectionMode = useContinuousCollision
                ? CollisionDetectionMode2D.Continuous
                : CollisionDetectionMode2D.Discrete;
        }
    }

    public void SetInThePool()
    {
        State = FruitState.InThePool;
    }

    public void ReturnToPool()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        if (poolManager != null)
        {
            poolManager.ReturnFruit(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (State == FruitState.Dropping)
        {
            bool hitWall = collision.collider.gameObject.layer == LayerMask.NameToLayer(wallLayerName);
            Fruit otherFruitForHit = collision.collider.GetComponent<Fruit>();
            bool hitFruit = otherFruitForHit != null;

            if (hitWall || hitFruit)
            {
                SetInThePool();

                if (AudioManager.instance != null)
                {
                    if (hitWall)
                        AudioManager.instance.PlaySound(fruitHitClip);
                    else
                        AudioManager.instance.PlaySound(groundHitClip);
                }
            }
        }

        if (GameManager.Instance == null) return;
        if (GameManager.Instance.GameEnd) return;

        Fruit other = collision.collider.GetComponent<Fruit>();
        if (other == null) return;

        if (!gameObject.activeInHierarchy || !other.gameObject.activeInHierarchy) return;
        if (State != FruitState.InThePool || other.State != FruitState.InThePool) return;

        if (other.tierIndex != tierIndex) return;
        if (isMerging || other.isMerging) return;
        if (GetInstanceID() > other.GetInstanceID()) return;

        isMerging = true;
        other.isMerging = true;

        Vector3 spawnPos = (transform.position + other.transform.position) * 0.5f;

        if (tierIndex >= MaxTier)
        {
            GameManager.Instance.MergeMaxTier(spawnPos, this, other);
            return;
        }

        GameManager.Instance.Merge(tierIndex, spawnPos, this, other);


    }
}
