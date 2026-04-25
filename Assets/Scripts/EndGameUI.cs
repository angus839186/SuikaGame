using System.Collections;
using TMPro;
using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private LightController lightController;

    [SerializeField] private GameObject winObject;
    [SerializeField] private GameObject loseObject;
    [SerializeField] private AudioSource endingAudio;

    [Header("Root UI")]
    [SerializeField] private GameObject gameOverRoot;
    [SerializeField] private GameObject playAgainButton;

    [SerializeField] private GameObject credit;

    [Header("Result UI")]
    [SerializeField] private GameObject endGameTitle;
    [SerializeField] private GameObject winButton;

    [SerializeField] private TextMeshProUGUI totalText;
    [SerializeField] private TextMeshProUGUI totalGroupText;

    [Header("Animation")]
    [SerializeField] private Animator winAnimator;

    [SerializeField] private Animator loseAnimator;
    [SerializeField] private string winAnimationStateName = "Win";
    [SerializeField] private string loseAnimationStateName = "Fail";


    [Header("Timing")]
    [SerializeField] private float lightsDuration = 1f;
    [SerializeField] private float FadeInDuration = 1f;

    [Header("Audio")]
    [SerializeField] private AudioClip openCanClip;
    [SerializeField] private AudioClip winMusic;
    [SerializeField] private AudioClip loseMusic;

    [SerializeField] private AudioClip lightoffClip;
    [SerializeField] private AudioClip lighton1Clip;
    [SerializeField] private AudioClip lighton2Clip;
    [SerializeField] private AudioClip lighton3Clip;

    private bool isWaitingForWinButton;
    private bool isWinSequencePlaying;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }
    }

    private void OnEnable()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        if (gameManager != null)
        {
            gameManager.GameResultAction += HandleGameResult;
        }
    }

    private void OnDisable()
    {
        if (gameManager != null)
        {
            gameManager.GameResultAction -= HandleGameResult;
        }
    }

    private void Start()
    {
        ResetEndGameUI();
    }

    private void HandleGameResult(bool isWin)
    {
        StopAllCoroutines();

        if (isWin)
        {
            StartCoroutine(PlayWinFlow());
        }
        else
        {
            StartCoroutine(PlayLoseFlow());
        }
        totalText.text = gameManager.Score.ToString();
        if (isWin == false)
        {
            totalGroupText.text = $"{gameManager.currentPhotoIndex} / 34";
        }
    }

    private IEnumerator PlayWinFlow()
    {
        ResetEndGameUI();

        gameOverRoot.SetActive(true);

        lightController.CloseAllLight();
        AudioManager.instance.PlaySound(lightoffClip);

        yield return new WaitForSeconds(lightsDuration);

        AudioManager.instance.PlaySound(lighton1Clip);

        yield return new WaitForSeconds(lightsDuration);

        lightController.EndGameLight();
        AudioManager.instance.PlaySound(lighton2Clip);

        yield return new WaitForSeconds(1f);
        endGameTitle.SetActive(true);
        winObject.SetActive(true);
        winButton.SetActive(true);
        AudioManager.instance.PlaySound(lighton3Clip);

        isWaitingForWinButton = true;
    }

    public void OnWinButtonClicked()
    {
        if (!isWaitingForWinButton) return;
        if (isWinSequencePlaying) return;

        StartCoroutine(PlayWinEndingSequence());
    }

    private IEnumerator PlayWinEndingSequence()
    {
        isWinSequencePlaying = true;
        isWaitingForWinButton = false;

        winButton.SetActive(false);
        AudioManager.instance.PlaySound(openCanClip);

        winAnimator.gameObject.SetActive(true);
        winAnimator.Play(winAnimationStateName, 0, 0f);

        yield return new WaitForSeconds(4f);

        StartCoroutine(FadeIn(credit));

        endingAudio.clip = winMusic;
        endingAudio.Play();

        yield return new WaitForSeconds(2.5f);

        StartCoroutine(FadeIn(playAgainButton));
    }

    private IEnumerator PlayLoseFlow()
    {
        ResetEndGameUI();

        gameOverRoot.SetActive(true);

        lightController.CloseAllLight();
        AudioManager.instance.PlaySound(lightoffClip);

        yield return new WaitForSeconds(lightsDuration);

        AudioManager.instance.PlaySound(lighton1Clip);

        yield return new WaitForSeconds(lightsDuration);

        lightController.EndGameLight();
        AudioManager.instance.PlaySound(lighton2Clip);

        yield return new WaitForSeconds(1f);


        endGameTitle.SetActive(true);

        loseAnimator.gameObject.SetActive(true);
        loseAnimator.Play(loseAnimationStateName, 0, 0f);
        AudioManager.instance.PlaySound(lighton3Clip);

        yield return new WaitForSeconds(2f);

        StartCoroutine(FadeIn(credit));

        endingAudio.clip = loseMusic;
        endingAudio.Play();

        yield return new WaitForSeconds(2.5f);

        StartCoroutine(FadeIn(playAgainButton));
    }

    private IEnumerator FadeIn(GameObject fadeinObject)
    {
        CanvasGroup canvasGroup = fadeinObject.GetComponent<CanvasGroup>();

        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0f;

        float elapsed = 0f;

        while (elapsed < FadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / FadeInDuration);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private void ResetEndGameUI()
    {
        isWaitingForWinButton = false;
        isWinSequencePlaying = false;

        if (gameOverRoot != null)
        {
            gameOverRoot.SetActive(false);
        }

        if (playAgainButton != null)
        {
            playAgainButton.SetActive(false);
        }

        if (endGameTitle != null)
        {
            endGameTitle.SetActive(false);
        }

        if (endGameTitle != null)
        {
            endGameTitle.SetActive(false);
        }

        if (winButton != null)
        {
            winButton.SetActive(false);
        }

        if (winAnimator != null)
        {
            winAnimator.gameObject.SetActive(false);
        }

        if (loseAnimator != null)
        {
            loseAnimator.gameObject.SetActive(false);
        }

        if (credit != null)
        {
            credit.gameObject.SetActive(false);
        }
    }
}
