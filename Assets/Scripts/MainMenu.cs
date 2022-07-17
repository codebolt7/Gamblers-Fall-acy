using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using FMODUnity;

public class MainMenu : MonoBehaviour
{
    private Music instance;
    [SerializeField] private GameObject music;
    [SerializeField] private GameObject tutorialMusic;

    private IMGUIContainer background;
    private IMGUIContainer playButton, settingsButton, creditsButton, fullscreenButton;
    private IMGUIContainer settingsMenu, musicSlider, masterSlider, creditsMenu;
    private IMGUIContainer blackBackground, lore, tutorial, tutorialSlide, tutorialPageNum, backButton, forwardButton;

    private bool settingsMenuOpen = false;
    private bool creditsMenuOpen = false;
    private bool musicSliderHeld = false;
    private bool masterSliderHeld = false;
    private bool tutorialVisible = false;
    private bool loreSkip = false;
    private int currentTutorialSlide = 0;

    [SerializeField] private Sprite[] buttonSprites = new Sprite[10];
    [SerializeField] private Sprite[] tutorialSlideSprites = new Sprite[6];

    private FMOD.Studio.VCA musicVCA;
    private FMOD.Studio.Bus master;

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

        blackBackground = rootVisualElement.Q<IMGUIContainer>("BlackBackground");
        lore = rootVisualElement.Q<IMGUIContainer>("Lore");
        tutorial = rootVisualElement.Q<IMGUIContainer>("Tutorial");
        tutorialSlide = rootVisualElement.Q<IMGUIContainer>("TutorialSlide");
        tutorialPageNum = rootVisualElement.Q<IMGUIContainer>("TutorialPageNum");
        backButton = rootVisualElement.Q<IMGUIContainer>("BackButton");
        forwardButton = rootVisualElement.Q<IMGUIContainer>("ForwardButton");

        playButton.RegisterCallback<MouseDownEvent>(ev => OnPlayButtonDown());
        settingsButton.RegisterCallback<MouseDownEvent>(ev => OnSettingsButtonDown());
        creditsButton.RegisterCallback<MouseDownEvent>(ev => OnCreditsButtonDown());

        lore.RegisterCallback<ClickEvent>(ev => SkipLore());
        playButton.RegisterCallback<ClickEvent>(ev => OnPlayButtonClick());
        settingsButton.RegisterCallback<ClickEvent>(ev => OnSettingsButtonClick());
        creditsButton.RegisterCallback<ClickEvent>(ev => OnCreditsButtonClick());
        fullscreenButton.RegisterCallback<ClickEvent>(ev => OnFullscreenButtonClick());

        musicSlider.RegisterCallback<MouseDownEvent>(ev => MusicSliderSelected());
        masterSlider.RegisterCallback<MouseDownEvent>(ev => MasterSliderSelected());

        backButton.RegisterCallback<ClickEvent>(ev => IncrementTutorialSlide(-1));
        forwardButton.RegisterCallback<ClickEvent>(ev => IncrementTutorialSlide(1));

        instance = Music.instance;

        musicVCA = FMODUnity.RuntimeManager.GetVCA("vca:/Music");
        master = FMODUnity.RuntimeManager.GetBus("bus:/");
    }

    void Start()
    {
        if (instance == null)
        {
            music.SetActive(true);
        }

        StartCoroutine(LevelStartTransition(1));
    }

    private void SkipLore()
    {
        RuntimeManager.CreateInstance("event:/SFX/Menu_Scroll").start();
        loreSkip = true;
    }

    private void OnPlayButtonDown()
    {
        RuntimeManager.CreateInstance("event:/SFX/Menu_Scroll").start();
        playButton.style.backgroundImage = new StyleBackground(buttonSprites[1]);
    }

    private void OnPlayButtonClick()
    {
        RuntimeManager.CreateInstance("event:/SFX/Menu_Scroll").start();
        StartCoroutine(FadeTutorial(true, 1, lore));
        tutorial.pickingMode = PickingMode.Position;
        playButton.style.backgroundImage = new StyleBackground(buttonSprites[0]);
    }

    private void OnSettingsButtonDown()
    {
        RuntimeManager.CreateInstance("event:/SFX/Menu_Scroll").start();
        settingsButton.style.backgroundImage = new StyleBackground(buttonSprites[3]);
    }

    private void OnSettingsButtonClick()
    {
        RuntimeManager.CreateInstance("event:/SFX/Menu_Scroll").start();
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
        RuntimeManager.CreateInstance("event:/SFX/Menu_Scroll").start();
        creditsButton.style.backgroundImage = new StyleBackground(buttonSprites[5]);
    }

    private void OnCreditsButtonClick()
    {
        RuntimeManager.CreateInstance("event:/SFX/Menu_Scroll").start();
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
        Screen.fullScreen = !Screen.fullScreen;
        if (Screen.fullScreen)
            fullscreenButton.style.backgroundImage = new StyleBackground(buttonSprites[6]);
        else
            fullscreenButton.style.backgroundImage = new StyleBackground(buttonSprites[7]);
    }

    private void MusicSliderSelected() 
    {
        Debug.Log("god fuck");
        musicSliderHeld = true;
    }

    private void MasterSliderSelected() 
    {
        masterSliderHeld = true;
    }

    private IEnumerator FadeTutorial(bool open, float time, IMGUIContainer element)
    {
        if (instance == null)
            music.SetActive(false);
        else
            instance.gameObject.SetActive(false);
        tutorialMusic.SetActive(true);

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
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        lore.style.top = finalVal;

        timeElapsed = 0;
        while(timeElapsed < 5 && !loreSkip)
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
        {
            RuntimeManager.CreateInstance("event:/SFX/Menu_Scroll").start();
            StartCoroutine(LevelTransition(1));
            
        }
        else
        {
            RuntimeManager.CreateInstance("event:/SFX/Menu_Scroll").start();
            currentTutorialSlide += amount;
            tutorialSlide.style.backgroundImage = new StyleBackground(tutorialSlideSprites[currentTutorialSlide]);
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

    public IEnumerator LevelStartTransition(float time)
    {
        float timeElapsed = 0;
        float t = 0;
        while(timeElapsed < time)
        {
            t = timeElapsed/time;
            t = Mathf.Sin((t * Mathf.PI) / 2);
            blackBackground.style.top = Mathf.Lerp(0, -640, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        blackBackground.style.top = -640;
    }

    private IEnumerator LevelTransition(float time)
    {
        if (instance)
        {
            Destroy(instance.gameObject);
        }
        blackBackground.style.opacity = 100;
        blackBackground.style.top = 0;
        float timeElapsed = 0;
        float t = 0;
        while(timeElapsed < time)
        {
            t = timeElapsed/time;
            t = Mathf.Sin((t * Mathf.PI) / 2);
            tutorial.style.top = Mathf.Lerp(0, -640, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        tutorial.style.top = -640;
        SceneManager.LoadScene("Level 1");
    }

    void Update()
    {
        if (!Mouse.current.leftButton.IsPressed())
        {
            playButton.style.backgroundImage = new StyleBackground(buttonSprites[0]);
            if (!settingsMenuOpen) settingsButton.style.backgroundImage = new StyleBackground(buttonSprites[2]);
            if (!creditsMenuOpen) creditsButton.style.backgroundImage = new StyleBackground(buttonSprites[4]);
            musicSliderHeld = false;
            masterSliderHeld = false;
        }
        if (musicSliderHeld)
        {
            musicSlider.style.left = Mathf.Clamp(Mouse.current.position.ReadValue().x - 272, 178, 404);
            Debug.Log(((float) musicSlider.style.left.value.value - 178) / ((404-178)/2));
            musicVCA.setVolume((float) (musicSlider.style.left.value.value - 178) / ((404-178)/2));
        }
        if (masterSliderHeld)
        {
            masterSlider.style.left = Mathf.Clamp(Mouse.current.position.ReadValue().x - 272, 178, 404);
            Debug.Log(((float) musicSlider.style.left.value.value - 178) / ((404-178)/2));
            master.setVolume((float) (masterSlider.style.left.value.value - 178) / ((404-178)/2));
        }
    }
}
