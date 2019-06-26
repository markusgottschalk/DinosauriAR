using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : Screen
{
    [SerializeField]
    private Canvas settingsScreen = default;
    private TextMeshProUGUI placeholderText;
    private TMP_InputField inputFieldText;
    [SerializeField]
    private GameObject inputField = default;

    public UIController UIController;

    private string playerName;

    void Start()
    {
        //settingsScreen = gameObject.GetComponent<Canvas>();
        placeholderText = inputField.GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>();
        inputFieldText = inputField.GetComponent<TMP_InputField>();
    }

    public override void ShowScreen()
    {
        settingsScreen.enabled = true;
    }

    public override void HideScreen()
    {
        settingsScreen.enabled = false;
    }

    public void OnSaveClicked()
    {
        playerName = inputFieldText.text;
        UIController.ChangePlayerName(playerName);
        inputFieldText.text = "";
        placeholderText.text = playerName;
    }

}
