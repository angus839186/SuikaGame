using UnityEngine;
using UnityEngine.UI;

public class NextPreviewUI : MonoBehaviour
{
    [SerializeField] private SuikaSpawner spawner;
    [SerializeField] private Image previewImage;
    [SerializeField] private Sprite[] tierSprites = new Sprite[10]; // A~J

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
        if (previewImage == null) return;

        if (tier < 0 || tier >= tierSprites.Length)
        {
            previewImage.enabled = false;
            return;
        }

        previewImage.sprite = tierSprites[tier];
        previewImage.enabled = previewImage.sprite != null;
    }
}