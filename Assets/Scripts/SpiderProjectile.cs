using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderProjectile : MonoBehaviour
{
    Player player;
    float attackDmg;
    Vector2 velocity;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 5;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            Destroy(gameObject);
        }
        GetComponent<Rigidbody2D>().velocity = velocity;
    }

    public void Init(Player player, float attackDmg, Vector2 direction)
    {
        this.player = player;
        this.attackDmg = attackDmg;
        velocity = direction * 3f;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            Debug.Log("Enemy HIT!");
            player.GetComponent<Player>().GetDamaged(attackDmg);
        }
    }
}
