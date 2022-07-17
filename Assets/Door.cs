using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using FMODUnity;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public int doorCost = 21;
    public bool finalDoor = false;
    private bool doorOpen = false;
    [SerializeField] private string nextLevel;
    [SerializeField] private GameObject battleUI;
    [SerializeField] private Sprite[] doorNums = new Sprite[10];
    [SerializeField] private Sprite[] doorSprites = new Sprite[4];

    // Start is called before the first frame update
    void Start()
    {
        Physics.queriesHitTriggers = true;
        UpdateDoorCounter(0);
        if (finalDoor) GetComponent<SpriteRenderer>().sprite = doorSprites[2];
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
            SceneManager.LoadScene(nextLevel);
    }
}
