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

    public event Action<int> OnNextChanged;

    private InputSystem_Actions input;
    private Camera cam;
    private float nextSpawnTime;

    public int NextTierIndex { get; private set; }

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
        float x = Mathf.Clamp(world.x, b.min.x, b.max.x);

        Vector3 spawnPos = new Vector3(x, spawnY.position.y, 0f);

        SpawnSpecific(NextTierIndex, spawnPos);
        RollNext();

        nextSpawnTime = Time.time + spawnCooldown;
    }

    private void RollNext()
    {
        NextTierIndex = UnityEngine.Random.Range(0, unlockedMaxTier + 1);
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