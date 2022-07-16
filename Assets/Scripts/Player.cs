using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Controls controls;
    Rigidbody2D rb;

    [Header("Object Assignment")]
    [SerializeField] GameObject attack;
    [SerializeField] SpriteRenderer spriteTop;
    [SerializeField] SpriteRenderer spriteBottom;
    [SerializeField] ContactFilter2D attackContactFilter;

    [Header("Stats")]
    [SerializeField] float attackDmg = 1f;
    [SerializeField] float speed = 5f;
    [SerializeField] float maxHP = 5f;
    [SerializeField] float attackDistance = 1;
    [SerializeField] float immunityDuration = 2;

    [Header("Object Assignment")]
    [SerializeField] float frameDelay;
    int idleFrameIndex = 0;
    [SerializeField] Sprite[] animIdleTop;
    [SerializeField] Sprite[] animIdleBottom;
    float walkTimer;
    int walkFrameIndex = -1;
    [SerializeField] Sprite[] animWalkTop;
    [SerializeField] Sprite[] animWalkBottom;
    [SerializeField] Sprite[] animAttackTop;
    [SerializeField] Sprite[] animSwingFX;

    private float hp;
    bool attacking;
    bool immunity;

    void Awake()
    {
        controls = new Controls();
        controls.Player.Attack.performed += _ => StartCoroutine(Attack());
        attack.transform.SetParent(transform.parent);
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector2 input = controls.Player.Move.ReadValue<Vector2>();
        rb.velocity = input * speed;

        BaseAnimate(input);
    }

    private void BaseAnimate(Vector2 input)
    {
        walkFrameIndex = input.magnitude > 0.1f ? Mathf.Max(walkFrameIndex, 0) : -1;

        // Walk
        if (walkFrameIndex >= 0)
        {
            spriteBottom.sprite = animWalkBottom[walkFrameIndex];
            spriteBottom.flipX = input.x > 0;
            if (!attacking)
            {
                spriteTop.sprite = animWalkTop[walkFrameIndex];
                spriteTop.flipX = input.x > 0;
            }

            walkTimer += Time.deltaTime;
            if (walkTimer > frameDelay)
            {
                walkFrameIndex = (walkFrameIndex + 1) % animWalkBottom.Length;
                walkTimer = 0;
            }
        }
        // Idle
        else
        {
            spriteBottom.sprite = animIdleBottom[idleFrameIndex];
            if (!attacking)
            {
                spriteTop.sprite = animIdleTop[idleFrameIndex];
                spriteTop.flipX = spriteBottom.flipX;
            }
        }
    }

    private IEnumerator Attack()
    {
        if (attacking) yield break;
        attacking = true;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(controls.Player.MousePosition.ReadValue<Vector2>());
        Vector2 relativePos = (worldPos - (Vector2)transform.position).normalized;

        spriteTop.flipX = relativePos.x > 0;

        attack.transform.position = relativePos * attackDistance + (Vector2)transform.position;
        attack.transform.right = transform.position - attack.transform.position;

        int i = 0;
        foreach (Sprite frame in animAttackTop)
        {
            SpriteRenderer attackSR = attack.GetComponent<SpriteRenderer>();
            if (i == 1)
            {
                attack.SetActive(true);
                attackSR.sprite = animSwingFX[0];

                List<Collider2D> hits = new List<Collider2D>();
                attack.GetComponent<Collider2D>().OverlapCollider(attackContactFilter, hits);
                foreach (Collider2D collider in hits)
                {
                    if (collider.TryGetComponent(out EnemyBat enemy))
                    {
                        enemy.GetDamaged(attackDmg);
                    }
                }
            } 
            else if (i > 1)
            {
                attackSR.sprite = animSwingFX[i - 1];
            }
            i++;

            spriteTop.sprite = frame;
            yield return new WaitForSeconds(frameDelay);
        }

        attack.SetActive(false);
        attacking = false;
    }

    public void GetDamaged(float damage)
    {
        if (immunity) return;
        hp -= damage;
        StartCoroutine(Immunity());
    }

    private IEnumerator Immunity()
    {
        immunity = true;
        spriteTop.color = new Color(1, 1, 1, 0.5f);
        spriteBottom.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(immunityDuration);
        immunity = false;
        spriteTop.color = new Color(1, 1, 1, 1);
        spriteBottom.color = new Color(1, 1, 1, 1);
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }
}
