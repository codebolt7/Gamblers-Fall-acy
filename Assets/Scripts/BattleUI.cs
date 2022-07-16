using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using FMODUnity;

public class BattleUI : MonoBehaviour
{
    int maxHealth = 8;
    int initHealth = 4;

    // Every UI element that will be changed
    private IMGUIContainer currentHealth, healthbarFill;
    private IMGUIContainer[] dice = new IMGUIContainer[7]; // 0th element is null just so indices match dice num
    private bool[] diceVals = {false, false, false, false, false, false, false};
    private IMGUIContainer[] abilities = new IMGUIContainer[4];
    private Button[] abilityButtons = new Button[4];
    private IMGUIContainer[] abilityCharges = new IMGUIContainer[4];
    private int[] chargeVals = {0, 0, 0, 0};

    // Sprites
    [SerializeField] private Sprite[] healthNums = new Sprite[9];
    [SerializeField] private Sprite[] diceSprites = new Sprite[12];
    [SerializeField] private Sprite[] abilitySprites = new Sprite[8];
    [SerializeField] private Sprite[] chargeNums = new Sprite[10];

    private IMGUIContainer grabbedDie;
    bool dieGrabbed = false;
    int grabbedDieVal = 0;
    float dieShakeTimer = 0f;

    Controls controls;
    void Awake()
    {
        controls = new Controls();

        controls.Player.Dice1.performed += _ => DiceGrab(1);
        controls.Player.Dice2.performed += _ => DiceGrab(2);
        controls.Player.Dice3.performed += _ => DiceGrab(3);
        controls.Player.Dice4.performed += _ => DiceGrab(4);
        controls.Player.Dice5.performed += _ => DiceGrab(5);
        controls.Player.Dice6.performed += _ => DiceGrab(6);
    }

    void test()
    {
        Debug.Log("ouchie");
    }

    void DiceGrab(int diceNum)
    {
        Debug.Log("wowow");
        if (diceVals[diceNum])
        {
            if (dieGrabbed)
                UpdateDice(grabbedDieVal, true);
            UpdateDice(diceNum, false);

            RuntimeManager.CreateInstance("event:/SFX/Menu_Scroll").start(); //sfx 
            
            dieGrabbed = true;
            grabbedDie.style.backgroundImage = new StyleBackground(diceSprites[diceNum * 2 - 1]);
            grabbedDie.style.opacity = 100;
            grabbedDieVal = diceNum;
        }
    }

    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        currentHealth = rootVisualElement.Q<IMGUIContainer>("CurrentHealth");
        healthbarFill = rootVisualElement.Q<IMGUIContainer>("Fill");

        dice[0] = null;
        dice[1] = rootVisualElement.Q<IMGUIContainer>("Dice1");
        dice[1].RegisterCallback<MouseDownEvent>(ev => DiceGrab(1));
        dice[2] = rootVisualElement.Q<IMGUIContainer>("Dice2");
        dice[2].RegisterCallback<MouseDownEvent>(ev => DiceGrab(2));
        dice[3] = rootVisualElement.Q<IMGUIContainer>("Dice3");
        dice[3].RegisterCallback<MouseDownEvent>(ev => DiceGrab(3));
        dice[4] = rootVisualElement.Q<IMGUIContainer>("Dice4");
        dice[4].RegisterCallback<MouseDownEvent>(ev => DiceGrab(4));
        dice[5] = rootVisualElement.Q<IMGUIContainer>("Dice5");
        dice[5].RegisterCallback<MouseDownEvent>(ev => DiceGrab(5));
        dice[6] = rootVisualElement.Q<IMGUIContainer>("Dice6");
        dice[6].RegisterCallback<MouseDownEvent>(ev => DiceGrab(6));

        abilities[0] = rootVisualElement.Q<IMGUIContainer>("Dash");
        abilityCharges[0] = rootVisualElement.Q<IMGUIContainer>("DashCharge");
        abilities[1] = rootVisualElement.Q<IMGUIContainer>("Fireball");
        abilityCharges[1] = rootVisualElement.Q<IMGUIContainer>("FireballCharge");
        abilities[2] = rootVisualElement.Q<IMGUIContainer>("Repel");
        abilityCharges[2] = rootVisualElement.Q<IMGUIContainer>("RepelCharge");
        abilities[3] = rootVisualElement.Q<IMGUIContainer>("Shield");
        abilityCharges[3] = rootVisualElement.Q<IMGUIContainer>("ShieldCharge");

        abilityButtons[0] = rootVisualElement.Q<Button>("DashButton");
        abilityButtons[0].RegisterCallback<MouseUpEvent>(ev => ResolveHeldDie(0));
        abilityButtons[1] = rootVisualElement.Q<Button>("FireballButton");
        abilityButtons[1].RegisterCallback<MouseUpEvent>(ev => ResolveHeldDie(1));
        abilityButtons[2] = rootVisualElement.Q<Button>("RepelButton");
        abilityButtons[2].RegisterCallback<MouseUpEvent>(ev => ResolveHeldDie(2));
        abilityButtons[3] = rootVisualElement.Q<Button>("ShieldButton");
        abilityButtons[3].RegisterCallback<MouseUpEvent>(ev => ResolveHeldDie(3));

        grabbedDie = rootVisualElement.Q<IMGUIContainer>("GrabbedDie");

        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    #region UI Update Functions
    // Change health to newHealthVal (range: 0-8)
    private void UpdateHealth(int newHealthVal)
    {
        if (newHealthVal > 8)
            Debug.Log("Health value of " + newHealthVal + " is above maximum health!");
        else
        {
            if (newHealthVal < 0) newHealthVal = 0; // clamp to 0

            healthbarFill.style.width = 176 * newHealthVal / maxHealth;
            currentHealth.style.backgroundImage = new StyleBackground(healthNums[newHealthVal]);
        }
    }

    private void HealthShake()
    {

    }

    private void ResolveHeldDie(int abilityNum)
    {
        Debug.Log("hey");
        if (dieGrabbed)
            if (chargeVals[abilityNum] >= 9) // if ability is fully charged, reject die
            {
                dieShakeTimer = 0.5f;
                // error sound?
            }
            else
            {  
                if (Random.Range(0, 2) * 2 <= 1)
                    RuntimeManager.CreateInstance("event:/SFX/DicePlace2").start(); 
                else
                    RuntimeManager.CreateInstance("event:/SFX/DicePlace").start(); 
                
                UpdateAbilities(abilityNum, Mathf.Clamp(chargeVals[abilityNum] + grabbedDieVal, 0, 9));
                dieGrabbed = false;
                grabbedDie.style.opacity = 0;
            }
                
        
    }

    // Update whether specified die is in player's inventory.
    // diceNum: number on the die (range: 1-6)
    // diceFilled: whether die is in inventory or not
    private void UpdateDice(int diceNum, bool diceFilled)
    {
        if (diceNum < 1 || diceNum > 6)
            Debug.Log("No die exists of value " + diceNum);
        else
        {
            if (diceFilled)
                dice[diceNum].style.backgroundImage = new StyleBackground(diceSprites[diceNum * 2 - 1]);
            else
                dice[diceNum].style.backgroundImage = new StyleBackground(diceSprites[diceNum * 2 - 2]);

            diceVals[diceNum] = diceFilled;
        }
    }

    // Add or remove charge of an ability
    // abilityNum: enum for the abilities going from left to right: dash = 0, fireball = 1, repel = 2, shield = 3
    // newCharge: new charge value (range: 0-9)
    private void UpdateAbilities(int abilityNum, int newCharge)
    {
        if (newCharge < 0 || newCharge > 9)
            Debug.Log("Specified charge value " + newCharge + " out of range for ability " + abilityNum);
        else
        {
            if (newCharge == 0)
                abilities[abilityNum].style.backgroundImage = new StyleBackground(abilitySprites[abilityNum * 2 + 1]);
            else
                abilities[abilityNum].style.backgroundImage = new StyleBackground(abilitySprites[abilityNum * 2]);       

            Debug.Log(newCharge);
            StartCoroutine(AbilityChargeLerp(abilityNum, newCharge, 166 + abilityNum * 160, 125 + abilityNum * 160, 0.15f, false)); // god i love uidocument
        }
    }

    IEnumerator AbilityChargeLerp(int abilityNum, int newCharge, int startPos, int newPos, float time, bool final)
    {
        float timeElapsed = 0;
        float t = 0;
        while(timeElapsed < time)
        {
            t = timeElapsed/time;
            abilityCharges[abilityNum].style.left = Mathf.Lerp(startPos, newPos, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // update charge sprite
        abilityCharges[abilityNum].style.left = newPos;
        abilityCharges[abilityNum].style.backgroundImage = new StyleBackground(chargeNums[newCharge]);
        chargeVals[abilityNum] = newCharge;

        if (!final)
            yield return AbilityChargeLerp(abilityNum, newCharge, newPos, startPos, time, true);
    }
    #endregion

    void Start()
    {
        UpdateHealth(initHealth);
        UpdateDice(1, true);
        UpdateDice(2, true);
        UpdateDice(3, true);
        UpdateDice(4, true);
        UpdateDice(5, true);
        UpdateDice(6, true);
        UpdateAbilities(0, 2);
    }

    void Update()
    {
        // i'm sorry about all these magic numbers
        if (dieGrabbed)
        {
            grabbedDie.style.left = Mouse.current.position.ReadValue().x - 30;
            grabbedDie.style.bottom = Mouse.current.position.ReadValue().y + 30 - 640;
            if (dieShakeTimer > 0)
            {
                grabbedDie.style.left = new StyleLength(Random.insideUnitSphere.x * 10f + grabbedDie.style.left.value.value);
                grabbedDie.style.bottom = new StyleLength(Random.insideUnitSphere.y * 10f + grabbedDie.style.bottom.value.value);
                dieShakeTimer -= Time.deltaTime;
            }
            else
                dieShakeTimer = 0f;
        }
    }
}
