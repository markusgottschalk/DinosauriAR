using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralAppScreen : Screen
{
    public UIController UIController;

    private Canvas generalAppScreen;
    [SerializeField]
    private GameObject snackbar;
    [SerializeField]
    private GameObject backButton;

    void Start()
    {
        generalAppScreen = gameObject.GetComponent<Canvas>();
    }

    void Update()
    {
        
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
    }

    public void HideBackButton()
    {
        backButton.SetActive(false);
    }
}
