using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Player : MonoBehaviour
{
    Controls controls;
    Rigidbody2D rb;

    [Header("Object Assignment")]
    [SerializeField] GameObject attack;
    [SerializeField] GameObject fireballProjectile;
    [SerializeField] GameObject shockwave;
    [SerializeField] GameObject shield;
    [SerializeField] GameObject pickup;
    [SerializeField] GameObject dash;
    [SerializeField] SpriteRenderer spriteTop;
    [SerializeField] SpriteRenderer spriteBottom;
    [SerializeField] ContactFilter2D attackContactFilter;
    [SerializeField] BattleUI batUI;

    [Header("Stats")]
    [SerializeField] float attackDmg = 1f;
    [SerializeField] float speed = 5f;
    [SerializeField] float maxHP = 8f;
    [SerializeField] float attackDistance = 1;
    [SerializeField] float dashDuration = 0.1f;
    [SerializeField] float dashLength = 3.5f;//originally 3.5 units
    [SerializeField] float fireballSpeed = 10.0f;
    [SerializeField] float fireballDamage = 1.5f;
    [SerializeField] float shockwaveKB = 1.5f;
    [SerializeField] float hitImmunityDuration = 2;
    [SerializeField] float dashImmunityDuration = 0.1f;
    [SerializeField] float fortuneImmunityDuration = 2.8f;
    [SerializeField] float immunityDuration = 2;
    [SerializeField] float pickupRange = 1.0f;

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
    [SerializeField] Sprite[] animCastTop;
    [SerializeField] Sprite[] animSwingFX;
    [SerializeField] Sprite[] animShockwaveFX;
    [SerializeField] Sprite[] animShieldFX;
 
    private float hp;
    public bool[] dice;
    bool attacking;
    bool dashing;
    bool casting;
    bool raving;
    bool shielded;
    bool immunity;

    void Awake()
    {
        controls = new Controls();
        controls.Player.Attack.performed += _ => StartCoroutine(Attack());
        controls.Player.Dash.performed += _ => StartCoroutine(Dash());
        controls.Player.Fireball.performed += _ => StartCoroutine(Fireball());
        controls.Player.Shockwave.performed += _ => StartCoroutine(Shockwave());
        controls.Player.Shield.performed += _ => StartCoroutine(Shield());
        dashing = false;
        dice = new bool[6];
        attack.transform.SetParent(transform.parent);
        shockwave.transform.SetParent(transform.parent);
        rb = GetComponent<Rigidbody2D>();
        hp = maxHP;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Pickup();
    }

    private void Move()
    {
        Vector2 input = controls.Player.Move.ReadValue<Vector2>();
        
        if(!dashing) rb.velocity = input * speed;
        
        BaseAnimate(input);
    }

    private void BaseAnimate(Vector2 input)
    {
        walkFrameIndex = input.magnitude > 0.1f ? Mathf.Max(walkFrameIndex, 0) : -1;
        
        //if(casting) return;

        // Walk
        if (walkFrameIndex >= 0)
        {
            spriteBottom.sprite = animWalkBottom[walkFrameIndex];
            spriteBottom.flipX = input.x > 0;
            if (!attacking && !casting)
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
            if (!attacking && !casting)
            {
                spriteTop.sprite = animIdleTop[idleFrameIndex];
                spriteTop.flipX = spriteBottom.flipX;
            }
        }
    }

    private void Pickup(){
        List<Collider2D> hit = new List<Collider2D>();
        pickup.GetComponent<Collider2D>().OverlapCollider(attackContactFilter, hit);
        foreach(Collider2D collider in hit){
            // Debug.Log("SOMETHIznG IS HERE BAWAHWBSJAJSHWJASBAJSHS PEEEING AND UPPOOPING11");
            if(collider.TryGetComponent(out PickupDie pie))
            {   
                
                Debug.Log("heh :3");
                Debug.Log((int)pie.num);
                if(dice[(int)pie.num]) return;
                dice[(int)pie.num] = true;

                if (Random.Range(0, 2) * 2 <= 1)
                    RuntimeManager.CreateInstance("event:/SFX/DicePickup").start();
                else
                    RuntimeManager.CreateInstance("event:/SFX/DicePickup2").start();

                
                Destroy(collider.transform.gameObject);
            }
        }

    }

    private IEnumerator Attack()
    {
        if (attacking) yield break; //The equivalent of return for an IEnumerator
        attacking = true;
        RuntimeManager.CreateInstance("event:/SFX/Player_BasicAttack").start();

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
                    if (collider.TryGetComponent(out EnemyBat enemyB))
                    {
                        enemyB.GetDamaged(attackDmg, 1);
                    }
                    else if (collider.TryGetComponent(out EnemySkeleton enemyS))
                    {
                        enemyS.GetDamaged(attackDmg, 1);
                        Debug.Log("Test");
                    }
                    else if (collider.TryGetComponent(out EnemySpider enemySp))
                    {
                        enemySp.GetDamaged(attackDmg, 1);
                        Debug.Log("Test");
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

    private IEnumerator Dash()
    {
        //Want a 4~frame invulnerable dash that travels 3.5~ in whatever 
        //direction is pressed
        if(batUI.chargeVals[0] <= 0) yield break;
        RuntimeManager.CreateInstance("event:/SFX/Player_Dash").start();
        dashing = true;
        immunityDuration = dashImmunityDuration;
        StartCoroutine(Immunity());        
        StartCoroutine(killCol());
        Vector2 currentVel = rb.velocity;
        rb.velocity = controls.Player.Move.ReadValue<Vector2>()*speed*2*dashLength;
        yield return new WaitForSeconds(dashDuration);
        rb.velocity = currentVel;
        dashing = false;
        batUI.chargeVals[0] -= 1;
        batUI.UpdateAbilities(0, batUI.chargeVals[0]);
    }

    private IEnumerator Fireball()
    {   
        if(batUI.chargeVals[1] <= 0) yield break;
        RuntimeManager.CreateInstance("event:/SFX/Spell_CastPower").start();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(controls.Player.MousePosition.ReadValue<Vector2>()); //Gets scrren position, converts to world position
        Vector2 relativePos = (worldPos - (Vector2)transform.position).normalized; // World position minus player position, then normalized
        StartCoroutine(Casting());
        yield return new WaitForSeconds(frameDelay);
        GameObject projectile = Instantiate(fireballProjectile);
        projectile.transform.position = transform.position;
        projectile.GetComponent<FireballProjectile>().Init(relativePos, fireballSpeed, fireballDamage);
        batUI.chargeVals[1] -= 1;
        batUI.UpdateAbilities(1, batUI.chargeVals[1]);
        yield break;
    }

    private IEnumerator Shockwave()
    {   
        if (raving || batUI.chargeVals[2] <= 0) yield break;
        RuntimeManager.CreateInstance("event:/SFX/Spell_CastAOE").start();
        batUI.chargeVals[2] -= 1;
        batUI.UpdateAbilities(2, batUI.chargeVals[2]);
        raving = true;
        //Debug.Log("shockwave!");
        StartCoroutine(Casting());
        shockwave.SetActive(true);
        shockwave.transform.position = (Vector2)transform.position;
        yield return new WaitForSeconds(frameDelay);
        
        List<Collider2D> hit = new List<Collider2D>();
        shockwave.GetComponent<Collider2D>().OverlapCollider(attackContactFilter, hit);
        foreach (Collider2D collider in hit)
        {
            Debug.Log(collider.name);
            if (collider.TryGetComponent(out EnemyBat enemyB))
            {
                enemyB.GetDamaged(attackDmg, shockwaveKB);
            }
            else if (collider.TryGetComponent(out EnemySkeleton enemyS))
            {
                enemyS.GetDamaged(attackDmg, shockwaveKB);
            }
            else if (collider.TryGetComponent(out EnemySpider enemySp))
            {
                enemySp.GetDamaged(attackDmg, shockwaveKB);
            }
            else if (collider.TryGetComponent(out BoneProjectile bone))
            {
                bone.Shockwave();
            }
            else if (collider.TryGetComponent(out SpiderProjectile bullet))
            {
                bullet.Shockwave();
            }

        }
        
        SpriteRenderer shockwaveSR = shockwave.GetComponent<SpriteRenderer>();

        foreach (Sprite frame in animShockwaveFX)
        {   
            shockwaveSR.sprite = frame;
            yield return new WaitForSeconds(frameDelay);
        }

        shockwave.SetActive(false);
        raving = false;

    }

    private IEnumerator Shield()
    {   
        if(shielded || batUI.chargeVals[3] <= 0) yield break;
        RuntimeManager.CreateInstance("event:/SFX/Spell_ShieldUp").start();
        batUI.chargeVals[3] -= 1;
        batUI.UpdateAbilities(3, batUI.chargeVals[3]);
        shielded = true;
        float shieldTimer = 0;

        immunityDuration = fortuneImmunityDuration;
        StartCoroutine(Casting());

        shield.SetActive(true);
        yield return new WaitForSeconds(frameDelay);
        StartCoroutine(Immunity());

        SpriteRenderer shieldSR = shield.GetComponent<SpriteRenderer>();

        while(shieldTimer <= fortuneImmunityDuration - 0.125f)
        {
            foreach (Sprite frame in animShieldFX)
            {   
                shieldSR.sprite = frame;
                yield return new WaitForSeconds(frameDelay);
                shieldTimer += frameDelay;
            }
            Debug.Log(shieldTimer);
            //Debug.Log(Time.timeScale);
        }

        RuntimeManager.CreateInstance("event:/SFX/Spell_ShieldDown").start();
        shield.SetActive(false);
        shielded = false;

    }

    public void GetDamaged(float damage)
    {
        if (immunity) return;
        RuntimeManager.CreateInstance("event:/SFX/Player_Hit").start();
        hp -= damage;
        Debug.Log(gameObject.name + "'s HP: " + hp);
        batUI.UpdateHealth((int) hp);
        if (hp <= 0)
        {
            Dead();
        }
        immunityDuration = hitImmunityDuration;
        StartCoroutine(Immunity());
    }

    private void Dead()
    {
        Destroy(gameObject);
    }

    private IEnumerator Immunity()
    {   
        //if(immunity) yield break;
        immunity = true;
        spriteTop.color = new Color(1, 1, 1, 0.5f);
        spriteBottom.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(immunityDuration);
        immunity = false;
        spriteTop.color = new Color(1, 1, 1, 1);
        spriteBottom.color = new Color(1, 1, 1, 1);
    }

    private IEnumerator killCol()
    {
        dash.transform.right = rb.velocity.normalized;
        List<Collider2D> hits = new List<Collider2D>();
        dash.GetComponent<Collider2D>().OverlapCollider(attackContactFilter, hits);
        foreach(Collider2D collider in hits)
        {            
            if (collider.TryGetComponent(out EnemyBat enemyB))
            {   
                Debug.Log("Bats Detected, ");
                collider.isTrigger = true;
            }
            else if (collider.TryGetComponent(out EnemySkeleton enemyS))
            {
                collider.isTrigger = true;
            }
            else if (collider.TryGetComponent(out EnemySpider enemySp))
            {
                collider.isTrigger = true;
            }
        }

        yield return new WaitForSeconds(dashDuration*1.2f);

        foreach(Collider2D collider in hits)
        {            
            if (collider.TryGetComponent(out EnemyBat enemyB))
            {
                collider.isTrigger = false;
            }
            else if (collider.TryGetComponent(out EnemySkeleton enemyS))
            {
                collider.isTrigger = false;
            }
            else if (collider.TryGetComponent(out EnemySpider enemySp))
            {
                collider.isTrigger = false;
            }
        }
        
    }

    private IEnumerator Casting()
    {   
        //Debug.Log("casting!");
        if(casting) yield break;
        casting = true;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(controls.Player.MousePosition.ReadValue<Vector2>());
        Vector2 relativePos = (worldPos - (Vector2)transform.position).normalized;

        spriteTop.flipX = relativePos.x > 0;
        foreach (Sprite sprit in animCastTop){
            spriteTop.sprite = sprit;
            yield return new WaitForSeconds(frameDelay);
        }

        casting = false;

    }

    public int GetHP()
    {
        return (int)hp;
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
