using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PhotoUI : MonoBehaviour
{
    [Header("上方圖片")]
    [SerializeField] private Image upperImage;

    [Header("下方圖片")]
    [SerializeField] private Image lowerImage;

    [Header("中間劇照")]
    [SerializeField] private Image photoImage;

    [SerializeField] private Animator photoanime;

    [Header("中文版照片")]
    [SerializeField] private ChinesePhoto[] chinesePhotos;

    [Header("英文版照片")]
    [SerializeField] private EnglishPhoto[] englishPhotos;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPhotoIndexChanged += HandlePhotoIndexChanged;
            HandlePhotoIndexChanged(GameManager.Instance.photoIndex);
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPhotoIndexChanged -= HandlePhotoIndexChanged;
        }
    }

    private void HandlePhotoIndexChanged(int index)
    {
        if (chinesePhotos == null || chinesePhotos.Length == 0) return;

        int photoArrayIndex = index - 1;
        if (photoArrayIndex < 0 || photoArrayIndex >= chinesePhotos.Length) return;

        ChinesePhoto photo = chinesePhotos[photoArrayIndex];

        if (upperImage != null)
            upperImage.sprite = photo.upperSprite;

        if (lowerImage != null)
            lowerImage.sprite = photo.lowerSprite;
    }

}