using UnityEngine;
using UnityEngine.UI;

public class NextPreview : MonoBehaviour
{
    [SerializeField] private FruitSpawner spawner;

    [SerializeField] private Animator FishAnimator;

    private void OnEnable()
    {
        if (spawner != null)
            spawner.OnNextChanged += HandleNextChanged;
    }

    private void OnDisable()
    {
        if (spawner != null)
            spawner.OnNextChanged -= HandleNextChanged;
    }

    private void Start()
    {
        if (spawner != null)
            HandleNextChanged(spawner.nextFruitIndex);
    }

    private void HandleNextChanged(int tier)
    {
        FishAnimator.Play(tier.ToString(), 0, 0f);
    }
}