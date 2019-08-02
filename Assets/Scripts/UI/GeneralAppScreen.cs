using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralAppScreen : UIScreen
{
    public UIController UIController;

    [SerializeField]
    private Canvas generalAppScreen = default;
    [SerializeField]
    private GameObject snackbar = default;
    [SerializeField]
    private GameObject backButton = default;
    [SerializeField]
    private GameObject settingsButton = default;
    private TextMeshProUGUI snackbarText;
    private bool backButtonIsActive;
    [SerializeField]
    private GameObject endExpeditionButton = default;

    /// <summary>
    /// The RawImage that provides rotating hand animation.
    /// </summary>
    [Tooltip("The RawImage that provides rotating hand animation.")]
    [SerializeField] private RawImage m_HandAnimation = null;

    /// <summary>
    /// The duration of the hand animation fades.
    /// </summary>
    private const float k_AnimationFadeDuration = 0.15f;

    void Start()
    {
        snackbarText = snackbar.GetComponentInChildren<TextMeshProUGUI>(true);
        endExpeditionButton.SetActive(false);
        m_HandAnimation.enabled = false;
    }

    public override void ShowScreen()
    {
        generalAppScreen.enabled = true;
    }

    public override void HideScreen()
    {
        generalAppScreen.enabled = false;
    }

    public void OnBackButtonClicked()
    {
        UIController.BackButtonWasClicked();
    }

    public void OnSettingsButtonClicked()
    {
        UIController.OnSettingsButtonClicked();
    }

    public void ChangeSnackbarText(string newText)
    {
        snackbarText.text = newText;
    }

    public void ShowSnackbar()
    {
        snackbar.SetActive(true);
    }

    public void HideSnackbar()
    {
        snackbar.SetActive(false);
    }

    public void ShowBackButton()
    {
        backButton.SetActive(true);
        backButtonIsActive = true;
    }

    public void HideBackButton()
    {
        backButton.SetActive(false);
        backButtonIsActive = false;
    }

    public bool BackButtonIsActive()
    {
        return backButtonIsActive;
    }

    public void ShowSettingsButton(bool active)
    {
        settingsButton.SetActive(active);
    }

    /// <summary>
    /// Shows the end expedition button when the expedition has been finished.
    /// </summary>
    /// <param name="active">The state of the button</param>
    /// <param name="success">The state of the expedition</param>
    public void ShowEndExpeditionButton(bool active, bool success)
    {
        endExpeditionButton.SetActive(active);
        endExpeditionButton.GetComponent<Button>().onClick.AddListener(delegate { ExitExpedition(success); });
    }

    /// <summary>
    /// Exits the expedition when it has been finished. Show new screen with more information if it was successful. Go back to the expeditions screen if it was not.
    /// </summary>
    /// <param name="success"></param>
    public void ExitExpedition(bool success)
    {
        if (success)
        {
            UIController.ARScreen_TengaduruExpeditionEnd();
        }
        else
        {
            UIController.TendaguruExpeditionEnd_Expeditions();
        }
    }

    /// <summary>
    /// Shows the hand animation for <paramref name="seconds"/>.
    /// </summary>
    /// <param name="seconds">Seconds to show the animation</param>
    public void ShowHandAnimation(float seconds)
    {
        if (!m_HandAnimation.enabled)
        {
            StartCoroutine(handAnimationTime(seconds));
        }
    }

    private IEnumerator handAnimationTime(float seconds)
    {
        m_HandAnimation.GetComponent<CanvasRenderer>().SetAlpha(0f);
        m_HandAnimation.CrossFadeAlpha(1f, k_AnimationFadeDuration, false);
        m_HandAnimation.enabled = true;

        yield return new WaitForSeconds(seconds);

        m_HandAnimation.GetComponent<CanvasRenderer>().SetAlpha(1f);
        m_HandAnimation.CrossFadeAlpha(0f, k_AnimationFadeDuration, false);
        m_HandAnimation.enabled = false;
    }
}
