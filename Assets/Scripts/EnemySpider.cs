using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class EnemySpider : MonoBehaviour
{
    private enum State
    {
        Moving,
        Stunned,
        Idle,
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
    [SerializeField] float attackChance = 0.5f;
    [SerializeField] float maxDistance = 5;
    [SerializeField] float maxMoveLimit = 3;
    [SerializeField] float projectSpread = 0.5f;
    [SerializeField] Vector2 idleDuration = new Vector2(1, 3);

    [Header("Animations")]
    [SerializeField] const float frameDelay = 0.1f;
    [SerializeField] Sprite idleSprite;
    [SerializeField] Sprite[] animMove;
    [SerializeField] Sprite[] animAttack;
    private int moveFrameIndex;
    private float moveTimer;
    private int attackFrameIndex;
    private float attackTimer;
    private float idleTimer;
    private float moveLimit;

    private Vector2 destination;
    private Vector2 spawnPosition;

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
        DetermineDestination();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Idle:
                Idle();
                break;
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
        RuntimeManager.CreateInstance("event:/SFX/Enemy_Hit").start();
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

    private void Idle()
    {
        if (idleTimer <= 0)
        {
            spriteRenderer.sprite = idleSprite;
            spriteRenderer.flipX = player.transform.position.x > transform.position.x;
            idleTimer = Random.Range(idleDuration.x, idleDuration.y);
        }
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0)
        {
            if (Random.Range(0, 1.0f) <= attackChance)
            {
                state = State.Attacking;
            }
            else
            {
                state = State.Moving;
                DetermineDestination();
            }
        }

    }

    private void Move()
    {
        moveLimit -= Time.deltaTime;
        // rb.velocity = (player.transform.position - transform.position).normalized * speed;
        transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        if (((Vector2)transform.position - destination).magnitude < 0.1f || moveLimit < 0)
        {
            state = State.Idle;
        }
        BaseAnimate(destination - (Vector2)transform.position);
    }

    private void Stunned()
    {
        stunTimer -= Time.deltaTime;
        if (stunTimer < 0)
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
            state = State.Moving;
            moveLimit = maxMoveLimit;
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
                moveLimit = maxMoveLimit;
                DetermineDestination();
            }

            if (attackFrameIndex == 1)
            {
                SpawnAttack();
            }
        }
    }

    private void SpawnAttack()
    {
        // Debug.Log("Skeleton Attacks");
        GameObject projectile0 = Instantiate(boneProjectile);
        GameObject projectile1 = Instantiate(boneProjectile);
        GameObject projectile2 = Instantiate(boneProjectile);
        projectile0.transform.position = transform.position;
        projectile1.transform.position = transform.position;
        projectile2.transform.position = transform.position;
        Vector2 dir0 = (player.transform.position - transform.position).normalized;
        Vector2 dir1 = new Vector2(
            dir0.x * Mathf.Cos(projectSpread) - dir0.y * Mathf.Sin(projectSpread),
            dir0.x * Mathf.Sin(projectSpread) + dir0.y * Mathf.Cos(projectSpread)
        );
        Vector2 dir2 = new Vector2(
            dir0.x * Mathf.Cos(-projectSpread) - dir0.y * Mathf.Sin(-projectSpread),
            dir0.x * Mathf.Sin(-projectSpread) + dir0.y * Mathf.Cos(-projectSpread)
        );
        projectile0.GetComponent<SpiderProjectile>().Init(player.GetComponent<Player>(), attackDmg, dir0);
        projectile1.GetComponent<SpiderProjectile>().Init(player.GetComponent<Player>(), attackDmg, dir1);
        projectile2.GetComponent<SpiderProjectile>().Init(player.GetComponent<Player>(), attackDmg, dir2);
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

    private void DetermineDestination()
    {
        Vector2 relativePos = (Vector2)transform.position - spawnPosition;
        Vector2 absPos = new Vector2(Mathf.Abs(relativePos.x), Mathf.Abs(relativePos.y));
        if (Random.Range(0, absPos.x + absPos.y) <= absPos.x || absPos.x < 1)
        {
            float distance = Random.Range(relativePos.x, maxDistance * Mathf.Sign(relativePos.x));
            destination = (Vector2)transform.position + Vector2.left * distance;
        }
        else
        {
            float distance = Random.Range(relativePos.y, maxDistance * Mathf.Sign(relativePos.y));
            destination = (Vector2)transform.position + Vector2.down * distance;
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

    void OnCollisionStay2D(Collision2D collision)
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
