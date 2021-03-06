using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PickupDie : MonoBehaviour
{
    public enum Number
    {
        ONE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
    }

    [SerializeField] Sprite[] sprites;
    private SpriteRenderer SR;
    private int temp;
    public Number num;
    private float dieDecay;
    private float timer = 0;


    void Awake(){
        temp = Random.Range(0, 6);
        SR = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        setNum(temp);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 2){
            SR.color = new Color(1, 1, 1, SR.color.a - (timer/400)*(SR.color.a));
        }
        if(timer >= 7)
        {
            Destroy(gameObject);
        }
    }

    public void setNum(int tempNum)
    {
        switch (tempNum){
            case 0:
                num = Number.ONE;
                RuntimeManager.CreateInstance("event:/SFX/DiceRollSoft").start();
                SR.sprite = sprites[tempNum];
                break;
            case 1:
                num = Number.TWO;
                RuntimeManager.CreateInstance("event:/SFX/DiceRollSemisoft").start();
                SR.sprite = sprites[tempNum];
                break;
            case 2:
                num = Number.THREE;
                RuntimeManager.CreateInstance("event:/SFX/DiceRollSemisoft2").start();
                SR.sprite = sprites[tempNum];
                break;
            case 3:
                num = Number.FOUR;
                RuntimeManager.CreateInstance("event:/SFX/DiceRollSemisharp").start();
                SR.sprite = sprites[tempNum];
                break;
            case 4:
                num = Number.FIVE;
                RuntimeManager.CreateInstance("event:/SFX/DiceRollSharp").start();
                SR.sprite = sprites[tempNum];
                break;
            case 5:
                num = Number.SIX;
                RuntimeManager.CreateInstance("event:/SFX/DiceRollSharp").start();
                SR.sprite = sprites[tempNum];
                break;
            default:
                num = Number.ONE;
                RuntimeManager.CreateInstance("event:/SFX/DiceRollSoft").start();
                SR.sprite = sprites[tempNum];
                break;
            }
    }

}
