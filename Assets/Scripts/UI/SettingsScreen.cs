using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : UIScreen
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
        placeholderText = inputField.GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>();
        inputFieldText = inputField.GetComponent<TMP_InputField>();

        playerName = UIController.GetPlayerName();
        placeholderText.text = playerName;
    }

    public override void ShowScreen()
    {
        settingsScreen.enabled = true;
    }

    public override void HideScreen()
    {
        settingsScreen.enabled = false;
    }

    /// <summary>
    /// Save the new player name and show it as new placeholder.
    /// </summary>
    public void OnSaveClicked()
    {
        playerName = inputFieldText.text;
        UIController.ChangePlayerName(playerName);
        inputFieldText.text = "";
        placeholderText.text = playerName;
    }

}
