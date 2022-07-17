using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    private IMGUIContainer background;
    private IMGUIContainer playButton, settingsButton, creditsButton, fullscreenButton;
    private IMGUIContainer settingsMenu, musicSlider, masterSlider, creditsMenu;
    private IMGUIContainer lore, tutorial, tutorialSlide, tutorialPageNum, backButton, forwardButton;

    private bool settingsMenuOpen = false;
    private bool creditsMenuOpen = false;
    private bool musicSliderHeld = false;
    private bool masterSliderHeld = false;
    private bool tutorialVisible = false;
    private int currentTutorialSlide = 0;

    [SerializeField] private Sprite[] buttonSprites = new Sprite[10];
    [SerializeField] private Sprite[] tutorialSlideSprites = new Sprite[6];
    [SerializeField] private Sprite[] tutorialPageNumSprites = new Sprite[6];

    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        background = rootVisualElement.Q<IMGUIContainer>("Background");

        playButton = rootVisualElement.Q<IMGUIContainer>("PlayButton");
        settingsButton = rootVisualElement.Q<IMGUIContainer>("SettingsButton");
        creditsButton = rootVisualElement.Q<IMGUIContainer>("CreditsButton");
        fullscreenButton = rootVisualElement.Q<IMGUIContainer>("FullscreenButton");

        settingsMenu = rootVisualElement.Q<IMGUIContainer>("SettingsMenu");
        creditsMenu = rootVisualElement.Q<IMGUIContainer>("CreditsMenu");
        musicSlider = rootVisualElement.Q<IMGUIContainer>("MusicSlider");
        masterSlider = rootVisualElement.Q<IMGUIContainer>("MasterSlider");

        lore = rootVisualElement.Q<IMGUIContainer>("Lore");
        tutorial = rootVisualElement.Q<IMGUIContainer>("Tutorial");
        tutorialSlide = rootVisualElement.Q<IMGUIContainer>("TutorialSlide");
        tutorialPageNum = rootVisualElement.Q<IMGUIContainer>("TutorialPageNum");
        backButton = rootVisualElement.Q<IMGUIContainer>("BackButton");
        forwardButton = rootVisualElement.Q<IMGUIContainer>("ForwardButton");

        playButton.RegisterCallback<MouseDownEvent>(ev => OnPlayButtonDown());
        settingsButton.RegisterCallback<MouseDownEvent>(ev => OnSettingsButtonDown());
        creditsButton.RegisterCallback<MouseDownEvent>(ev => OnCreditsButtonDown());

        playButton.RegisterCallback<ClickEvent>(ev => OnPlayButtonClick());
        settingsButton.RegisterCallback<ClickEvent>(ev => OnSettingsButtonClick());
        creditsButton.RegisterCallback<ClickEvent>(ev => OnCreditsButtonClick());
        fullscreenButton.RegisterCallback<ClickEvent>(ev => OnFullscreenButtonClick());

        musicSlider.RegisterCallback<MouseDownEvent>(ev => MusicSliderSelected());
        masterSlider.RegisterCallback<MouseDownEvent>(ev => MasterSliderSelected());

        backButton.RegisterCallback<ClickEvent>(ev => IncrementTutorialSlide(-1));
        forwardButton.RegisterCallback<ClickEvent>(ev => IncrementTutorialSlide(1));
    }

    private void OnPlayButtonDown()
    {
        playButton.style.backgroundImage = new StyleBackground(buttonSprites[1]);
    }

    private void OnPlayButtonClick()
    {
        StartCoroutine(FadeTutorial(true, 1, lore));
        tutorial.pickingMode = PickingMode.Position;
        playButton.style.backgroundImage = new StyleBackground(buttonSprites[0]);
    }

    private void OnSettingsButtonDown()
    {
        settingsButton.style.backgroundImage = new StyleBackground(buttonSprites[3]);
    }

    private void OnSettingsButtonClick()
    {
        if (settingsMenuOpen)
        {
            settingsMenu.style.opacity = 0;
            settingsMenu.pickingMode = PickingMode.Ignore;
            settingsButton.style.backgroundImage = new StyleBackground(buttonSprites[2]);
            settingsMenuOpen = false;
        }
        else
        {
            settingsMenu.style.opacity = 100;
            settingsMenu.pickingMode = PickingMode.Position;
            settingsMenuOpen = true;
        }
    }

    private void OnCreditsButtonDown()
    {
        creditsButton.style.backgroundImage = new StyleBackground(buttonSprites[5]);
    }

    private void OnCreditsButtonClick()
    {
        if (creditsMenuOpen)
        {
            creditsMenu.style.opacity = 0;
            creditsMenu.pickingMode = PickingMode.Ignore;
            creditsButton.style.backgroundImage = new StyleBackground(buttonSprites[4]);
            creditsMenuOpen = false;
        }
        else
        {
            creditsMenu.style.opacity = 100;
            creditsMenu.pickingMode = PickingMode.Position;
            creditsMenuOpen = true;
        }
    }

    private void OnFullscreenButtonClick()
    {
        // toggle fullscreen
    }

    private void MusicSliderSelected() 
    {
        musicSliderHeld = true;
    }

    private void MasterSliderSelected() 
    {
        masterSliderHeld = false;
    }

    private IEnumerator FadeTutorial(bool open, float time, IMGUIContainer element)
    {
        float finalVal, startVal;
        if (open)
        {
            startVal = 640;
            finalVal = 0;
        }
        else
        {
            startVal = 0;
            finalVal = 640;
        }

        float timeElapsed = 0;
        float t = 0;
        while(timeElapsed < time)
        {
            t = timeElapsed/time;
            t = Mathf.Sin((t * Mathf.PI) / 2);
            lore.style.top = Mathf.Lerp(startVal, finalVal, t);
            Debug.Log(tutorial.style.top);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        element.style.top = finalVal;

        timeElapsed = 0;
        while(timeElapsed < 5)
        {
            timeElapsed+= Time.deltaTime;
            yield return null;
        }

        timeElapsed = 0;
        while(timeElapsed < time)
        {
            t = timeElapsed/time;
            t = Mathf.Sin((t * Mathf.PI) / 2);
            tutorial.style.top = Mathf.Lerp(startVal, finalVal, t);
            lore.style.top = Mathf.Lerp(finalVal, -startVal, t);
            Debug.Log(tutorial.style.top);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        tutorialVisible = true;
        
    }

    private void IncrementTutorialSlide(int amount)
    {
        if (currentTutorialSlide + amount < 0)
        {
            
        }
        else if (currentTutorialSlide + amount > 5)
            SceneManager.LoadScene("UI Test");
        else
        {
            currentTutorialSlide += amount;
            tutorialSlide.style.backgroundImage = new StyleBackground(tutorialSlideSprites[currentTutorialSlide]);
            tutorialPageNum.style.backgroundImage = new StyleBackground(tutorialPageNumSprites[currentTutorialSlide]);
            if (currentTutorialSlide == 5)
                forwardButton.style.backgroundImage = new StyleBackground(buttonSprites[6]);
            else
                forwardButton.style.backgroundImage = new StyleBackground(buttonSprites[9]);
            if (currentTutorialSlide == 0)
                backButton.style.opacity = 0;
            else
                backButton.style.opacity = 100;
        }
        
    }

    void Update()
    {
        if (!Mouse.current.leftButton.IsPressed()){
            playButton.style.backgroundImage = new StyleBackground(buttonSprites[0]);
            if (!settingsMenuOpen) settingsButton.style.backgroundImage = new StyleBackground(buttonSprites[2]);
            if (!creditsMenuOpen) creditsButton.style.backgroundImage = new StyleBackground(buttonSprites[4]);
            musicSliderHeld = false;
            masterSliderHeld = false;
        }
    }
}
