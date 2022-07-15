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

    [Header("Movement")]
    [SerializeField] float speed = 5f;

    [Header("Attack")]
    [SerializeField] float attackDistance = 1;

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

    bool attacking;

    void Awake()
    {
        controls = new Controls();
        controls.Player.Attack.performed += _ => StartCoroutine(Attack());
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
            spriteBottom.flipX = input.x > 0;
            if (!attacking)
            {
                spriteTop.sprite = animIdleTop[idleFrameIndex];
                spriteTop.flipX = input.x > 0;
            }
        }
    }

    private IEnumerator Attack()
    {
        if (attacking) yield break;
        attacking = true;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(controls.Player.MousePosition.ReadValue<Vector2>());
        Vector2 relativePos = (worldPos - (Vector2)transform.position).normalized;

        attack.transform.position = relativePos * attackDistance + (Vector2)transform.position;
        spriteTop.flipX = relativePos.x > 0;

        foreach (Sprite frame in animAttackTop)
        {
            spriteTop.sprite = frame;
            yield return new WaitForSeconds(frameDelay);
        }

        attacking = false;
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
