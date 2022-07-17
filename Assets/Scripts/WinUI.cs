using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class WinUI : MonoBehaviour
{   
    [SerializeField] private GameObject door;
    private IMGUIContainer titleButton;
    [SerializeField] private Sprite[] buttonSprites = new Sprite[2];

    void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        titleButton = rootVisualElement.Q<IMGUIContainer>("TitleButton");
        titleButton.RegisterCallback<MouseDownEvent>(ev => OnTitleButtonDown());
        titleButton.RegisterCallback<ClickEvent>(ev => OnTitleButtonClick());
    }

    private void OnTitleButtonDown()
    {
        titleButton.style.backgroundImage = new StyleBackground(buttonSprites[1]);
    }

    private void OnTitleButtonClick()
    {
        titleButton.style.backgroundImage = new StyleBackground(buttonSprites[0]);
        StartCoroutine(door.GetComponent<Door>().LevelEndTransition(1));
    }

    void Update()
    {
        if (!Mouse.current.leftButton.IsPressed()){
            titleButton.style.backgroundImage = new StyleBackground(buttonSprites[0]);
        }
    }
}
