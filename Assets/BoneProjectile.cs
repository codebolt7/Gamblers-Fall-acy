using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneProjectile : MonoBehaviour
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
        transform.Rotate(Vector3.forward * 200f * Time.deltaTime);
        GetComponent<Rigidbody2D>().velocity = velocity;
    }

    public void Init(Player player, float attackDmg)
    {
        this.player = player;
        this.attackDmg = attackDmg;
        velocity = (player.transform.position - transform.position).normalized * 7f;
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
