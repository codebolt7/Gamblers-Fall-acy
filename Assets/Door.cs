using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using FMODUnity;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public int doorCost = 21;
    public bool finalDoor = false;
    private bool doorOpen = false;
    private Music instance;
    [SerializeField] private string nextLevel;
    [SerializeField] private GameObject battleUI;
    [SerializeField] private GameObject music;
    [SerializeField] private Sprite[] doorNums = new Sprite[10];
    [SerializeField] private Sprite[] doorSprites = new Sprite[4];

    private IMGUIContainer blackBackground;

    void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        blackBackground = rootVisualElement.Q<IMGUIContainer>("BlackBackground");

        instance = Music.instance;
    }

    private IEnumerator DelayMusic(float time)
    {
        float timeElapsed = 0;
        float t = 0;
        while(timeElapsed < time)
        {
            t = timeElapsed/time;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        music.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        Physics.queriesHitTriggers = true;
        UpdateDoorCounter(0);
        if (finalDoor) GetComponent<SpriteRenderer>().sprite = doorSprites[2];
        StartCoroutine(LevelStartTransition(1));
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit)
            {
                if (hit.transform.name == "Door")
                    battleUI.GetComponent<BattleUI>().ResolveHeldDieDoor(gameObject);
            }
        }
        
    }

    public void UpdateDoorCounter(int decrement)
    {
        doorCost -= decrement;
        if (doorCost < 0) doorCost = 0;
        int ones = doorCost % 10;
        gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = doorNums[ones];
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = doorNums[(int) doorCost / 10];
        if (doorCost == 0)
        {
            if (finalDoor)
                GetComponent<SpriteRenderer>().sprite = doorSprites[3];
            else
                GetComponent<SpriteRenderer>().sprite = doorSprites[1];
            gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            RuntimeManager.CreateInstance("event:/SFX/DoorOpen").start(); 
            doorOpen = true;
        } 
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player" && doorOpen)
        {
            if (finalDoor)
                instance.gameObject.SetActive(false);
            StartCoroutine(LevelEndTransition(1));
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

    public IEnumerator LevelEndTransition(float time)
    {
        float timeElapsed = 0;
        float t = 0;
        while(timeElapsed < time)
        {
            t = timeElapsed/time;
            t = Mathf.Sin((t * Mathf.PI) / 2);
            blackBackground.style.top = Mathf.Lerp(640, 0, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        blackBackground.style.top = 0;
        SceneManager.LoadScene(nextLevel);
    }
}
