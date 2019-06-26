using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralAppScreen : Screen
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

    void Start()
    {
        //generalAppScreen = gameObject.GetComponent<Canvas>();
        snackbarText = snackbar.GetComponentInChildren<TextMeshProUGUI>(true);
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

}
