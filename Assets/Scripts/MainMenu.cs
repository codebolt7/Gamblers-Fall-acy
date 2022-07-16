using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private Button playButton, settingsButton, creditsButton;

    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        playButton = rootVisualElement.Q<Button>("PlayButton");
        settingsButton = rootVisualElement.Q<Button>("SettingsButton");
        creditsButton = rootVisualElement.Q<Button>("CreditsButton");

        playButton.RegisterCallback<ClickEvent>(ev => OnPlayButtonClick());
    }

    private void OnPlayButtonClick()
    {
        SceneManager.LoadScene("Theres a FUCKING GHOST in this Scene");
    }
}
