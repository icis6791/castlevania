using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pirate : MonoBehaviour, Enemy
{
    Transform player;  //플레이어
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;
    AudioSource audioSource;
    public AudioClip damageClip;
    Vector3 playerDistance;  //플레이어와의 거리
    public float speed;
    public float health = 50;
    bool facingRight = false;
    bool isDead = false;
    public float attackRate = 3;

    float nextAttack;
    int damage = 30;
    public bool isAttack = false;
    float t;
    float t1;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PLAYER").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            /*//해적 앞에 바닥을 향하는 Ray로 앞이 낭떠러지 인지 아닌지 판단.
            Debug.DrawRay(front.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(front.position, Vector3.down, 2, LayerMask.GetMask("GROUND"));
            if (rayHit.collider == null)
            {
                rb.velocity = Vector2.zero;
                canMove = false;
                Flip();
                //rb.velocity = new Vector2(speed * (-(playerDistance.x / Mathf.Abs(playerDistance.x))), rb.velocity.y);
            }
            else
                canMove = true;*/

            playerDistance = player.transform.position - this.transform.position;
            if (Mathf.Abs(playerDistance.x) < 10 && Mathf.Abs(playerDistance.y) < 3 && !isAttack)
            {
                //(playerDistance.x / Mathf.Abs(playerDistance.x) 이 부분은 playerDistance의 부호를 가져오기 위함임.
                //그 부호를 speed랑 곱함. 쉽게 말해 왼쪽, 오른쪽 방향을 위한 것
                rb.velocity = new Vector2(speed * (playerDistance.x / Mathf.Abs(playerDistance.x)), rb.velocity.y);
                if (Mathf.Abs(playerDistance.x) < 2.5f && t > nextAttack)
                {
                    StartCoroutine(Attack());
                }
            }
            //속도로 애니메이션 재생
            anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

            /*if (rb.velocity.x > 0 && !facingRight)
            {
                Flip();
            }
            else if (rb.velocity.x < 0 && facingRight)
            {
                Flip();
            }*/
            if (playerDistance.x > 0 && !facingRight)
            {
                Flip();
            }
            else if (playerDistance.x < 0 && facingRight)
            {
                Flip();
            }
        }
    }

    private void Update()
    {
        t += Time.deltaTime;
        t1 -= Time.deltaTime * 50;
    }


    void Flip() //좌우 회전 메서드
    {
        if (!isAttack)
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    IEnumerator Attack()  //공격 메서드
    {
        rb.velocity = Vector2.zero;
        anim.SetTrigger("Attack");
        isAttack = true;
        nextAttack = t + attackRate;

        yield return new WaitForSeconds(2f);
        isAttack = false;
    }

    void NoAtk()
    {
        isAttack = false;
    }

    public void TakeDamage(int damage)  //공격받는 메서드
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(damageClip);  //피격 사운드
        }
        health -= damage;
        StartCoroutine(DamageCoroutine());
        if (health <= 0)
        {
            //사망애니메이션 재생 및 콜라이더 + 스프라이트 지우기
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            GetComponent<CapsuleCollider2D>().enabled = false;
            anim.SetTrigger("Die");
            isDead = true;
            StartCoroutine(Vanish());
        }
    }

    IEnumerator DamageCoroutine()  //공격받을 때 깜빡거리는 효과
    {
        sprite.color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(0.15f);
        sprite.color = new Color(1, 1, 1, 1);
    }

    IEnumerator Vanish()  //서서히 사라지는 효과
    {
        yield return new WaitForSeconds(1.2f);
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

    private void OnTriggerEnter2D(Collider2D other)  //검 공격 맞았을 때
    {
        //충돌체 태그가 PLAYER이면 스크립트를 가져와서 스크립트 안에 있는 데미지 받는 함수 호출.
        if (other.gameObject.CompareTag("PLAYER"))
        {
            var player = other.gameObject.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)  //몸통에 닿았을 때
    {
        if (other.gameObject.CompareTag("PLAYER"))
        {
            var player = other.gameObject.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.TakeDamage(20);
            }
        }
    }
}
