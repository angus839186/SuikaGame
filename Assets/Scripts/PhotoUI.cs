using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PhotoUI : MonoBehaviour, IPointerClickHandler
{
    [Header("背景照片")]
    [SerializeField] private Image backgroundImage;

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

    [SerializeField] private GameObject photoUiButton;



    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPhotoIndexChanged += HandlePhotoIndexChanged;
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
        Sprite backgroundSprite = null;

        switch (GameManager.Instance.CurrentLanguage)
        {
            case GameManager.Language.Chinese:
                if (chinesePhotos == null || photoArrayIndex >= chinesePhotos.Length) return;

                ChinesePhoto chinesePhoto = chinesePhotos[photoArrayIndex];
                controller = chinesePhoto.photoanime;
                backgroundSprite = chinesePhoto.backgroundSprite;
                break;

            case GameManager.Language.English:
                if (englishPhotos == null || photoArrayIndex >= englishPhotos.Length) return;

                EnglishPhoto englishPhoto = englishPhotos[photoArrayIndex];
                controller = englishPhoto.photoanime;
                backgroundSprite = englishPhoto.bakcgroundSprite;
                break;
        }

        if (backgroundSprite != null)
        {
            backgroundImage.sprite = backgroundSprite;
        }

        if (PhotoAnime != null)
        {
            PhotoAnime.runtimeAnimatorController = controller;
            PhotoAnime.Rebind();
            PhotoAnime.Update(0f);
            if (PhotoAnime.runtimeAnimatorController != null)
            {
                PhotoAnime.Play(0, 0, 0f);
            }
        }
    }

    public void ShowPhoto()
    {
        if (PhotoGroupAnime == null) return;

        if (suikaSpawner != null)
            suikaSpawner.SetSpawnEnabled(false);

        isPhotoOpen = true;
        PhotoGroupAnime.ResetTrigger("Close");
        PhotoGroupAnime.SetTrigger("Open");
        photoUiButton.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isPhotoOpen || PhotoGroupAnime == null) return;

        isPhotoOpen = false;
        PhotoGroupAnime.ResetTrigger("Open");
        PhotoGroupAnime.SetTrigger("Close");
        photoUiButton.SetActive(true);

        if (suikaSpawner != null)
            suikaSpawner.SetSpawnEnabled(true);
    }



}