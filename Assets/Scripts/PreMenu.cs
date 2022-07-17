using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PreMenu : MonoBehaviour
{
    IMGUIContainer blackBackground;
    void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        blackBackground = rootVisualElement.Q<IMGUIContainer>("BlackBackground");
        blackBackground.RegisterCallback<MouseDownEvent>(ev => InitMenu());
    }

    void InitMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
