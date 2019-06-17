using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : Screen
{
    private Canvas settingsScreen;
    private string placeholderText;
    private string inputFieldText;

    public UIController UIController;

    private string playerName;

    void Start()
    {
        settingsScreen = gameObject.GetComponent<Canvas>();
        placeholderText = gameObject.GetComponent<InputField>().placeholder.GetComponent<Text>().text;
        inputFieldText = gameObject.GetComponent<InputField>().text;
    }

    public override void ShowScreen()
    {
        settingsScreen.enabled = true;
    }

    public override void HideScreen()
    {
        settingsScreen.enabled = false;
    }

    public void ChangeName(string newName)
    {
        playerName = newName;
    }

    public void OnSaveClicked()
    {
        UIController.PlayerName = playerName;
        inputFieldText = "";
        placeholderText = playerName;
    }

}
