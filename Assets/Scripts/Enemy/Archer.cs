using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour, Enemy
{
    public GameObject arrowPrefab;  //화살 프리팹
    public Transform arrowSpawn;  //화살 발사 위치
    Transform player;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;
    AudioSource audioSource;
    public AudioClip damageClip;
    Vector3 playerDistance;
    public float speed = 1.2f;  //속도
    public float health = 40;  //체력
    public bool facingRight = false;  //좌우 방향
    bool isAttack = false;
    bool isDead;
    float fireRate = 2f;  //발사 쿨타임
    float nextFire;
    float t;  //늘어나는 시간
    float t1;  //줄어드는 시간
    int damage = 20;  //데미지

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PLAYER").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();  
    }

    void Update()
    {
        t += Time.deltaTime;
        t1 -= Time.deltaTime * 50;
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            playerDistance = player.position - transform.position;
            if (Mathf.Abs(playerDistance.x) > 7 && Mathf.Abs(playerDistance.y) < 3 && !isAttack)
            {
                rb.velocity = new Vector2(speed * (playerDistance.x / Mathf.Abs(playerDistance.x)), rb.velocity.y);
            }

            if (Mathf.Abs(playerDistance.x) <= 7 && nextFire < t)
            {
                anim.SetTrigger("Fire");
                isAttack = true;
            }
        }

        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        if (playerDistance.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (playerDistance.x < 0 && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        if (!isAttack && !isDead)
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    IEnumerator Fire()
    {
        anim.ResetTrigger("Fire");
        var arrow = Instantiate(arrowPrefab, arrowSpawn.position, arrowSpawn.rotation);
        GameManager.instance.fireObjects.Add(arrow);
        arrow.GetComponent<Arrow>().SetDirection(facingRight);
        nextFire = t + fireRate;
        yield return new WaitForSeconds(1.5f);
        isAttack = false;
    }

    public void TakeDamage(int damage)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(damageClip);
        }
        health -= damage;
        StartCoroutine(DamageCoroutine());
        if(health <= 0)
        {
            anim.SetTrigger("Die");
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            GetComponent<CapsuleCollider2D>().enabled = false;
            isDead = true;
            StartCoroutine(Vanish());
        }
    }

    IEnumerator DamageCoroutine()
    {
        sprite.color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(0.15f);
        sprite.color = new Color(1, 1, 1, 1);
    }

    IEnumerator Vanish()
    {
        yield return new WaitForSeconds(0.5f);
        Color color = sprite.color;
        t1 = 80;
        while (true)
        {
            color.a = t1 / 80;
            sprite.color = color;
            yield return null;
            if (t1 <= 0)
            {
                t1 = 0;
                Destroy(gameObject);
                break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("PLAYER"))
        {
            var player = other.gameObject.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }

    }
}
