using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float attackDmg;
    [SerializeField] float speed = 5;

    [Header("Animations")]
    [SerializeField] const float frameDelay = 0.1f;
    [SerializeField] Sprite[] anim;

    Vector2 velocity;
    private int moveFrameIndex;
    private float moveTimer;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        moveTimer = 0;
        timer = 5;
    }

    // Update is called once per frame
    void Update()   
    {
        timer -= Time.deltaTime;
        Debug.Log(timer);
        if (timer < 0)
        {
            Destroy(gameObject);
        }

        GetComponent<Rigidbody2D>().velocity = velocity;
        BaseAnimate();
    }

    private void BaseAnimate(){
        GetComponent<SpriteRenderer>().sprite = anim[moveFrameIndex];

        moveTimer += Time.deltaTime;
        if (moveTimer > frameDelay)
        {
            moveFrameIndex = (moveFrameIndex + 1) % anim.Length;
            moveTimer = 0;
        }
    }

    public void Init(Vector2 dir, float proSpeed, float damageMult){
        //GetComponent<Rigidbody2D>().velocity
        velocity = dir*proSpeed;
        attackDmg = damageMult;
        transform.right = dir;

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // if (collision.gameObject == player)
        // {
        //     Debug.Log("Enemy HIT!");
        //     player.GetComponent<Player>().GetDamaged(attackDmg);
        // }
        if (collision.TryGetComponent(out EnemyBat enemyB))
        {
            enemyB.GetDamaged(attackDmg, 1);
        }
        else if (collision.TryGetComponent(out EnemySkeleton enemyS))
        {
            enemyS.GetDamaged(attackDmg);
            //Need to add knockback function
            Debug.Log("Test");
        }else if (collision.gameObject.layer == 6){
            Destroy(gameObject);
        }


    }
}
