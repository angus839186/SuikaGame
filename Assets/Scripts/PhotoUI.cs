using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PhotoUI : MonoBehaviour, IPointerClickHandler
{
    [Header("上方圖片")]
    [SerializeField] private Image upperImage;

    [Header("下方圖片")]
    [SerializeField] private Image lowerImage;

    [Header("中文版照片")]
    [SerializeField] private ChinesePhoto[] chinesePhotos;

    [Header("英文版照片")]
    [SerializeField] private EnglishPhoto[] englishPhotos;

    [Header("群組動畫")]
    [SerializeField] private Animator PhotoGroupAnime;

    [Header("照片動畫")]
    [SerializeField] private Animator PhotoAnime;

    [Header("西瓜生成器")]
    [SerializeField] private SuikaSpawner suikaSpawner;

    private bool isPhotoOpen;



    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPhotoIndexChanged += HandlePhotoIndexChanged;
            HandlePhotoIndexChanged(GameManager.Instance.currentPhotoIndex);
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
        if (GameManager.Instance == null) return;

        int photoArrayIndex = index;
        if (photoArrayIndex < 0) return;

        RuntimeAnimatorController controller = null;
        Sprite upperSprite = null;
        Sprite lowerSprite = null;

        switch (GameManager.Instance.CurrentLanguage)
        {
            case GameManager.Language.Chinese:
                if (chinesePhotos == null || photoArrayIndex >= chinesePhotos.Length) return;

                ChinesePhoto chinesePhoto = chinesePhotos[photoArrayIndex];
                controller = chinesePhoto.photoanime;
                upperSprite = chinesePhoto.upperSprite;
                lowerSprite = chinesePhoto.lowerSprite;
                break;

            case GameManager.Language.English:
                if (englishPhotos == null || photoArrayIndex >= englishPhotos.Length) return;

                EnglishPhoto englishPhoto = englishPhotos[photoArrayIndex];
                controller = englishPhoto.photoanime;
                upperSprite = englishPhoto.upperSprite;
                lowerSprite = englishPhoto.lowerSprite;
                break;
        }

        if (upperImage != null)
            upperImage.sprite = upperSprite;

        if (lowerImage != null)
            lowerImage.sprite = lowerSprite;

        if (PhotoAnime != null)
        {
            PhotoAnime.runtimeAnimatorController = controller;
            PhotoAnime.Rebind();
            PhotoAnime.Update(0f);
            PhotoAnime.Play(0, 0, 0f);
        }

        ShowPhoto();
    }

    public void ShowPhoto()
    {
        if (PhotoGroupAnime == null) return;

        if (suikaSpawner != null)
            suikaSpawner.SetSpawnEnabled(false);

        isPhotoOpen = true;
        PhotoGroupAnime.ResetTrigger("Close");
        PhotoGroupAnime.SetTrigger("Open");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isPhotoOpen || PhotoGroupAnime == null) return;

        isPhotoOpen = false;
        PhotoGroupAnime.ResetTrigger("Open");
        PhotoGroupAnime.SetTrigger("Close");

        if (suikaSpawner != null)
            suikaSpawner.SetSpawnEnabled(true);
    }



}