using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SuikaSpawner : MonoBehaviour
{
    [SerializeField] private BoxCollider2D spawnArea;
    [SerializeField] private Transform spawnY;
    [SerializeField] private GameObject[] tierPrefabs;

    [SerializeField] private int unlockedMaxTier = 0;
    [SerializeField] private float spawnCooldown = 0.2f;

    [SerializeField] private float spawnPadding = 0.3f; // 你想保留的內縮距離（世界座標）
    [SerializeField] private bool includeFruitRadiusInClamp = true;

    public event Action<int> OnNextChanged;

    private InputSystem_Actions input;
    private Camera cam;
    private float nextSpawnTime;

    public int NextTierIndex { get; private set; }

    [SerializeField, Range(0f, 0.2f)]
    private float highTierPenalty = 0.06f;

    private void Awake()
    {
        cam = Camera.main;
        input = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        input.Enable();
        input.Gameplay.Click.performed += OnClick;
    }

    private void OnDisable()
    {
        input.Gameplay.Click.performed -= OnClick;
        input.Disable();
    }

    private void Start()
    {
        RollNext();
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        if (Time.time < nextSpawnTime) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        Vector2 screenPos = input.Gameplay.Point.ReadValue<Vector2>();
        Vector3 world = cam.ScreenToWorldPoint(screenPos);
        world.z = 0f;

        Bounds b = spawnArea.bounds;
        float r = 0f;
        if (includeFruitRadiusInClamp)
        {
            var col = tierPrefabs[NextTierIndex].GetComponent<CircleCollider2D>();
            if (col != null)
                r = col.radius * tierPrefabs[NextTierIndex].transform.lossyScale.x; // 假設等比縮放
        }

        float minX = b.min.x + spawnPadding + r;
        float maxX = b.max.x - spawnPadding - r;

        float x = Mathf.Clamp(world.x, minX, maxX);

        Vector3 spawnPos = new Vector3(x, spawnY.position.y, 0f);

        SpawnSpecific(NextTierIndex, spawnPos);
        RollNext();

        nextSpawnTime = Time.time + spawnCooldown;
    }

    private int RollNextWeighted()
    {
        // unlocked: 0..unlockedMaxTier
        int n = unlockedMaxTier + 1;

        // weight(tier) = 1 - tier * penalty, 最小保底 0.05（避免完全抽不到）
        float total = 0f;
        float[] w = new float[n];
        for (int t = 0; t < n; t++)
        {
            w[t] = Mathf.Max(0.05f, 1f - t * highTierPenalty);
            total += w[t];
        }

        float r = UnityEngine.Random.value * total;
        for (int t = 0; t < n; t++)
        {
            r -= w[t];
            if (r <= 0f) return t;
        }
        return n - 1;
    }

    private void RollNext()
    {
        NextTierIndex = RollNextWeighted();
        OnNextChanged?.Invoke(NextTierIndex);
    }

    public void SpawnSpecific(int tierIndex, Vector3 pos)
    {
        Instantiate(tierPrefabs[tierIndex], pos, Quaternion.identity);
    }

    public void UnlockTier(int tier)
    {
        unlockedMaxTier = Mathf.Max(unlockedMaxTier, tier);
    }
}