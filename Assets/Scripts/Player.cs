using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Controls controls;
    Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] float speed = 5f;

    [Header("Attack")]
    [SerializeField] GameObject attack;
    [SerializeField] float attackDistance = 1;

    [Header("Animations")]
    [SerializeField] SpriteRenderer spriteTop;
    [SerializeField] SpriteRenderer spriteBottom;

    void Awake()
    {
        controls = new Controls();
        controls.Player.Attack.performed += _ => Attack();
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
        Debug.Log(input);
        spriteBottom.flipX = input.x > 0;
    }

    private void Attack()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(controls.Player.MousePosition.ReadValue<Vector2>());
        Vector2 relativePos = (worldPos - (Vector2)transform.position).normalized;

        attack.transform.position = relativePos * attackDistance + (Vector2)transform.position;
        spriteTop.flipX = relativePos.x > 0;
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
