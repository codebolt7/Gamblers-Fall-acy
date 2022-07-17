using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkeleton : MonoBehaviour
{
    private enum State
    {
        Moving,
        Stunned,
        Attacking,
        Dead
    }
    private State state;

    [Header("Object Assignment")]
    [SerializeField] Rigidbody2D rb;
    GameObject player;
    SpriteRenderer spriteRenderer;
    [SerializeField] GameObject boneProjectile;
    [SerializeField] GameObject die;

    [Header("Stats")]
    [SerializeField] float speed = 1;
    [SerializeField] float attackDmg = 1;
    [SerializeField] float maxHP = 5f;
    [SerializeField] float pushbackForce = 30;
    [SerializeField] float stunDuration = 3;
    [SerializeField] float attackChance;

    [Header("Animations")]
    [SerializeField] const float frameDelay = 0.1f;
    [SerializeField] Sprite[] animMove;
    [SerializeField] Sprite[] animAttack;
    private int moveFrameIndex;
    private float moveTimer;
    private int attackFrameIndex;
    private float attackTimer;

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
            case State.Attacking:
                Attacking();
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
        rb.AddForceAtPosition((transform.position - player.transform.position).normalized * pushbackForce*knockbackMultiplier, player.transform.position);
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

    private void Attacking()
    {
        spriteRenderer.sprite = animAttack[attackFrameIndex];
        spriteRenderer.flipX = player.transform.position.x > transform.position.x;

        attackTimer += Time.deltaTime;
        if (attackTimer > frameDelay)
        {
            attackFrameIndex++;
            attackTimer = 0;

            if (attackFrameIndex >= animAttack.Length)
            {
                attackFrameIndex = 0;
                state = State.Moving;
            }

            if (attackFrameIndex == 3)
            {
                SpawnAttack();
            }
        }
    }

    private void SpawnAttack()
    {
        Debug.Log("Skeleton Attacks");
        GameObject projectile = Instantiate(boneProjectile);
        projectile.transform.position = transform.position;
        projectile.GetComponent<BoneProjectile>().Init(player.GetComponent<Player>(), attackDmg);
    }

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
            if (moveFrameIndex == 0 && Random.Range(0, 1.0f) <= attackChance)
            {
                rb.velocity = Vector2.zero;
                state = State.Attacking;
            }
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
