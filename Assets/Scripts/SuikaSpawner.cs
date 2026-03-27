using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SuikaSpawner : MonoBehaviour
{
    private InputSystem_Actions input;
    private Camera cam;

    [SerializeField] private BoxCollider2D clickArea;
    [SerializeField] private Transform spawnY;
    [SerializeField] private GameObject[] fruitPrefabs;

    [SerializeField] private int unlockedFruitTier = 0;
    [SerializeField] private float spawnCooldown = 0.2f;

    [SerializeField] private float spawnPadding = 0.3f;

    [SerializeField] private float randomDropOffsetX = 0.15f;
    private bool includeFruitRadiusInClamp = true;
    public GameObject heldFruit;

    public event Action<int> OnNextChanged;
    private float nextSpawnTime;

    public int currentFruitIndex { get; private set; }
    public int nextFruitIndex { get; private set; }

    private bool isPressing;

    [SerializeField, Range(0f, 0.2f)]
    private float highTierPenalty = 0.06f;

    public bool canSpawn;

    [SerializeField] private PhotoAnimationController photoAnimationController;

    private void Awake()
    {
        cam = Camera.main;
        input = new InputSystem_Actions();

    }

    private void OnEnable()
    {
        input.Enable();
        input.Gameplay.Click.started += OnPressStarted;
        input.Gameplay.Click.canceled += OnPressCanceled;
        if(photoAnimationController != null)
        {
            photoAnimationController.OnPhotoToggled += SetSpawnEnabled;
        }
    }

    private void OnDisable()
    {
        if(photoAnimationController != null)
        {
            photoAnimationController.OnPhotoToggled -= SetSpawnEnabled;
        }
        input.Gameplay.Click.started -= OnPressStarted;
        input.Gameplay.Click.canceled -= OnPressCanceled;
        input.Disable();
    }

    private void Start()
    {
        currentFruitIndex = RollNextWeighted();
        nextFruitIndex = RollNextWeighted();
        OnNextChanged?.Invoke(nextFruitIndex);

        SpawnHeldFruit();
        SnapHeldToPointerX();
        SetSpawnEnabled(true);
    }

    private void Update()
    {
        if (!canSpawn) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        if (heldFruit == null) return;

        if (Mouse.current != null)
        {
            SnapHeldToPointerX();
            return;
        }

        if (Touchscreen.current != null)
        {
            if (isPressing)
                SnapHeldToPointerX();
            return;
        }

        if (isPressing)
            SnapHeldToPointerX();
    }

    private void OnPressStarted(InputAction.CallbackContext ctx)
    {
        if (!canSpawn)
        {
            isPressing = false;
            return;
        }

        isPressing = IsPointerInClickArea();
    }

    private void OnPressCanceled(InputAction.CallbackContext ctx)
    {
        if (!canSpawn)
        {
            isPressing = false;
            return;
        }

        if (!isPressing) return;
        isPressing = false;

        if (!IsPointerInClickArea()) return;

        TryDropHeldFruit();
    }

    private bool IsPointerInClickArea()
    {
        if (cam == null || clickArea == null) return false;

        Vector2 screenPos = input.Gameplay.Point.ReadValue<Vector2>();
        Vector3 world = cam.ScreenToWorldPoint(screenPos);
        world.z = 0f;

        return clickArea.OverlapPoint(world);
    }

    private void TryDropHeldFruit()
    {
        if (Time.time < nextSpawnTime) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        if (heldFruit == null) return;

        float offset = UnityEngine.Random.Range(-randomDropOffsetX, randomDropOffsetX);
        float x = ClampXForTier(heldFruit.transform.position.x + offset, currentFruitIndex);

        Vector3 p = heldFruit.transform.position;
        p.x = x;
        heldFruit.transform.position = p;

        var rb = heldFruit.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        heldFruit = null;

        currentFruitIndex = nextFruitIndex;
        nextFruitIndex = RollNextWeighted();
        OnNextChanged?.Invoke(nextFruitIndex);

        SpawnHeldFruit();
        SnapHeldToPointerX();

        nextSpawnTime = Time.time + spawnCooldown;
    }

    private void SpawnHeldFruit()
    {
        var prefab = fruitPrefabs[currentFruitIndex];
        if (prefab == null) return;

        Vector3 pos = new Vector3(clickArea.bounds.center.x, spawnY.position.y, 0f);
        heldFruit = Instantiate(prefab, pos, Quaternion.identity);

        var rb = heldFruit.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }
    }

    private void SnapHeldToPointerX()
    {
        Vector2 screenPos = input.Gameplay.Point.ReadValue<Vector2>();
        Vector3 world = cam.ScreenToWorldPoint(screenPos);
        world.z = 0f;

        float x = ClampXForTier(world.x, currentFruitIndex);

        Vector3 p = heldFruit.transform.position;
        p.x = x;
        p.y = spawnY.position.y;
        heldFruit.transform.position = p;
    }

    private float ClampXForTier(float rawX, int tierIndex)
    {
        Bounds b = clickArea.bounds;

        float r = 0f;
        if (includeFruitRadiusInClamp)
        {
            var col = fruitPrefabs[tierIndex].GetComponent<CircleCollider2D>();
            if (col != null)
                r = col.radius * fruitPrefabs[tierIndex].transform.lossyScale.x;
        }

        float minX = b.min.x + spawnPadding + r;
        float maxX = b.max.x - spawnPadding - r;

        return Mathf.Clamp(rawX, minX, maxX);
    }

    private int RollNextWeighted()
    {
        int n = unlockedFruitTier + 1;

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

    public GameObject SpawnSpecific(int tierIndex, Vector3 pos)
    {
        var obj = Instantiate(fruitPrefabs[tierIndex], pos, Quaternion.identity);

        var rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        return obj;
    }

    public void UnlockTier(int tier)
    {
        unlockedFruitTier = Mathf.Max(unlockedFruitTier, tier);
    }

    public void SetSpawnEnabled(bool enabled)
    {
        canSpawn = !enabled;
        heldFruit.SetActive(canSpawn);
        isPressing = false;
    }
}