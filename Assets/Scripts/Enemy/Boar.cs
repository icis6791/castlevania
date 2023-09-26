using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : MonoBehaviour, Enemy
{
    public GameObject firePrefab;  //불 프리팹
    public Transform fireSpawn;  //불 발사 위치
    Transform playerTr;
    PlayerCtrl player;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;
    AudioSource audioSource;
    public AudioClip damageClip;
    Vector3 playerDistance;
    public float speed = 1.5f;  //속도
    public float health = 70;  //체력
    public bool facingRight = false;  //좌우방향 판단
    bool isDead = false;
    bool isAttack = false;
    int damage = 30;  //데미지
    float fireRate = 2f;  //화염 발사 쿨타임.
    float nextFire;
    float t;  //늘어나는 시간
    float t1;  //줄어드는 시간

    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").transform;
        player = playerTr.gameObject.GetComponent<PlayerCtrl>();
        
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

    private void FixedUpdate()
    {
        if(!isDead)
        {
            playerDistance = playerTr.position - transform.position;
            //플레이어 방향으로 이동.
            if(Mathf.Abs(playerDistance.x) > 7 && Mathf.Abs(playerDistance.y) < 3 && !isAttack)
            {
                rb.velocity = new Vector2(speed * (playerDistance.x / Mathf.Abs(playerDistance.x)), rb.velocity.y);
               /* if(Mathf.Abs(playerDistance.x) < 1.5 && Mathf.Abs(playerDistance.y) > 2)
                {
                    StartCoroutine(Upper());
                }*/
            }

            //nextFire가 현재시간보다 적고, 플레이어와의 거리가 7이하일 때 불 발사.
            if(Mathf.Abs(playerDistance.x) <= 7 && nextFire < t)
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

    void Flip()  //좌우 방향 전환 메서드
    {
        if (!isAttack && !isDead)
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(damageClip);
        }
        health -= damage;
        StartCoroutine(DamageCoroutine());
        if (health <= 0)
        {
            anim.SetTrigger("Die");
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            GetComponent<PolygonCollider2D>().enabled = false;
            if(facingRight)
            {
                rb.AddForce(Vector2.left * 5, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(Vector2.right * 5, ForceMode2D.Impulse);
            }
            isDead = true;
            StartCoroutine(Vanish());
        }
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

    IEnumerator DamageCoroutine()
    {
        sprite.color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(0.15f);
        sprite.color = new Color(1, 1, 1, 1);
    }

    IEnumerator Fire()  //불 발사 메서드
    {
        anim.ResetTrigger("Fire");
        //불 생성.
        var fire = Instantiate(firePrefab, fireSpawn.position, fireSpawn.rotation);
        GameManager.instance.fireObjects.Add(fire);
        fire.GetComponent<BoarsFire>().SetDirection(facingRight);
        //nextFire에 현재시간 + 쿨타임을 대입.
        nextFire = t + fireRate;
        yield return new WaitForSeconds(1.5f);
        isAttack = false;
    }

    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PLAYER"))
        {
            var player = other.gameObject.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }*/

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
