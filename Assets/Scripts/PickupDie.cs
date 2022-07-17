using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
    }

    public void setNum(int tempNum)
    {
        switch (tempNum){
            case 0:
                num = Number.ONE;
                SR.sprite = sprites[tempNum];
                break;
            case 1:
                num = Number.TWO;               
                SR.sprite = sprites[tempNum];
                break;
            case 2:
                num = Number.THREE;
                SR.sprite = sprites[tempNum];
                break;
            case 3:
                num = Number.FOUR;
                SR.sprite = sprites[tempNum];
                break;
            case 4:
                num = Number.FIVE;
                SR.sprite = sprites[tempNum];
                break;
            case 5:
                num = Number.SIX;
                SR.sprite = sprites[tempNum];
                break;
            default:
                num = Number.ONE;
                SR.sprite = sprites[tempNum];
                break;
            }
    }

}
