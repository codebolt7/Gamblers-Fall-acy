using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleUI : MonoBehaviour
{
    int maxHealth = 8;
    int initHealth = 4;

    // Every UI element that will be changed
    private IMGUIContainer currentHealth, healthbarFill;
    private IMGUIContainer[] dice = new IMGUIContainer[7]; // 0th element is null just so indexes match dice num
    private IMGUIContainer[] abilities = new IMGUIContainer[4];
    private IMGUIContainer[] abilityCharges = new IMGUIContainer[4];

    // Sprites
    [SerializeField] private Sprite[] healthNums = new Sprite[9];
    [SerializeField] private Sprite[] diceSprites = new Sprite[12];
    [SerializeField] private Sprite[] abilitySprites = new Sprite[8];
    [SerializeField] private Sprite[] chargeNums = new Sprite[10];

    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        currentHealth = rootVisualElement.Q<IMGUIContainer>("CurrentHealth");
        healthbarFill = rootVisualElement.Q<IMGUIContainer>("Fill");

        dice[0] = null;
        dice[1] = rootVisualElement.Q<IMGUIContainer>("Dice1");
        dice[2] = rootVisualElement.Q<IMGUIContainer>("Dice2");
        dice[3] = rootVisualElement.Q<IMGUIContainer>("Dice3");
        dice[4] = rootVisualElement.Q<IMGUIContainer>("Dice4");
        dice[5] = rootVisualElement.Q<IMGUIContainer>("Dice5");
        dice[6] = rootVisualElement.Q<IMGUIContainer>("Dice6");

        abilities[0] = rootVisualElement.Q<IMGUIContainer>("Dash");
        abilityCharges[0] = rootVisualElement.Q<IMGUIContainer>("DashCharge");
        abilities[1] = rootVisualElement.Q<IMGUIContainer>("Fireball");
        abilityCharges[1] = rootVisualElement.Q<IMGUIContainer>("FireballCharge");
        abilities[2] = rootVisualElement.Q<IMGUIContainer>("Repel");
        abilityCharges[2] = rootVisualElement.Q<IMGUIContainer>("RepelCharge");
        abilities[3] = rootVisualElement.Q<IMGUIContainer>("Shield");
        abilityCharges[3] = rootVisualElement.Q<IMGUIContainer>("ShieldCharge");

        HealthUpdate(initHealth);
        DiceUpdate(2, true);
        AbilityUpdate(0, 9);
    }

    private void HealthUpdate(int newHealthVal)
    {
        healthbarFill.style.width = 176 * newHealthVal / maxHealth;
        currentHealth.style.backgroundImage = new StyleBackground(healthNums[newHealthVal]);
    }

    private void DiceUpdate(int diceNum, bool diceFilled)
    {
        if (diceFilled)
            dice[diceNum].style.backgroundImage = new StyleBackground(diceSprites[diceNum * 2 - 1]);
        else
            dice[diceNum].style.backgroundImage = new StyleBackground(diceSprites[diceNum * 2 - 2]);
    }

    private void AbilityUpdate(int abilityNum, int newCharge)
    {
        
        if (newCharge == 0)
            abilities[abilityNum].style.backgroundImage = new StyleBackground(abilitySprites[abilityNum * 2 + 1]);
        else
            abilities[abilityNum].style.backgroundImage = new StyleBackground(abilitySprites[abilityNum * 2]);

        abilityCharges[abilityNum].style.backgroundImage = new StyleBackground(chargeNums[newCharge]);
    }
}
