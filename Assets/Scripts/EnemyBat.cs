using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBat : MonoBehaviour
{
    private enum State
    {
        Moving,
        Stunned,
        Dead
    }
    private State state;

    [Header("Object Assignment")]
    [SerializeField] Rigidbody2D rb;
    GameObject player;
    SpriteRenderer spriteRenderer;
    [SerializeField] GameObject die;

    [Header("Stats")]
    [SerializeField] float speed = 1;
    [SerializeField] float attackDmg = 1;
    [SerializeField] float maxHP = 5f;
    [SerializeField] float pushbackForce = 30;
    [SerializeField] float stunDuration = 3;

    [Header("Animations")]
    [SerializeField] const float frameDelay = 0.1f;
    [SerializeField] Sprite[] animMove;
    private int moveFrameIndex;
    private float moveTimer;

    private float hp;
    private float stunTimer;
    private bool spawned;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHP;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        spawned = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Moving:
                Move();
                break;
            case State.Stunned:
                Stunned();
                break;
            case State.Dead:
                Dead();
                break;
        }
    }

    public void GetDamaged(float damage, float knockbackMultiplier)
    {
        hp -= damage;
        Debug.Log(gameObject.name + "'s HP: " + hp);
        stunTimer = state == State.Dead ? stunDuration*4 : stunDuration;
        
        state = State.Stunned;
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        rb.velocity = Vector2.zero;
        rb.AddForceAtPosition((transform.position - player.transform.position).normalized * pushbackForce *knockbackMultiplier, player.transform.position);
        if (hp <= 0)
        {
            spriteRenderer.color = new Color(1, 0f, 0f, 0.25f);
            state = State.Dead;
        }
    }

    private void Move()
    {
        rb.velocity = (player.transform.position - transform.position).normalized * speed;
        BaseAnimate(rb.velocity);
    }

    private void Stunned()
    {
        stunTimer -= Time.deltaTime;
        if (stunTimer < 0)
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
            state = State.Moving;
        }
    }

    // private void knockback(){
    //     rb.AddForceAtPosition((transform.position - player.transform.position).normalized * pushbackForce, player.transform.position);  
    // }

    private void Dead()
    {   
        if(!spawned){
            GameObject deathyDie = Instantiate(die);
            deathyDie.transform.position = transform.position;
            deathyDie.transform.SetParent(transform.parent);
            spawned = true;
        }

        stunTimer -= Time.deltaTime;
        if (stunTimer < 0)
        {   

            Destroy(gameObject);
        }
    }

    private void BaseAnimate(Vector2 direction)
    {
        spriteRenderer.sprite = animMove[moveFrameIndex];
        spriteRenderer.flipX = direction.x > 0;

        moveTimer += Time.deltaTime;
        if (moveTimer > frameDelay)
        {
            moveFrameIndex = (moveFrameIndex + 1) % animMove.Length;
            moveTimer = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == State.Dead || state == State.Stunned)
        {
            return;
        }
        if (collision.gameObject == player)
        {
            Debug.Log("Enemy HIT!");
            player.GetComponent<Player>().GetDamaged(attackDmg);
        }
    }
}
