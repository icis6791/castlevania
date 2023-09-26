using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : MonoBehaviour, Enemy
{
    public GameObject firePrefab;  //�� ������
    public Transform fireSpawn;  //�� �߻� ��ġ
    Transform playerTr;
    PlayerCtrl player;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;
    AudioSource audioSource;
    public AudioClip damageClip;
    Vector3 playerDistance;
    public float speed = 1.5f;  //�ӵ�
    public float health = 70;  //ü��
    public bool facingRight = false;  //�¿���� �Ǵ�
    bool isDead = false;
    bool isAttack = false;
    int damage = 30;  //������
    float fireRate = 2f;  //ȭ�� �߻� ��Ÿ��.
    float nextFire;
    float t;  //�þ�� �ð�
    float t1;  //�پ��� �ð�

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
            //�÷��̾� �������� �̵�.
            if(Mathf.Abs(playerDistance.x) > 7 && Mathf.Abs(playerDistance.y) < 3 && !isAttack)
            {
                rb.velocity = new Vector2(speed * (playerDistance.x / Mathf.Abs(playerDistance.x)), rb.velocity.y);
               /* if(Mathf.Abs(playerDistance.x) < 1.5 && Mathf.Abs(playerDistance.y) > 2)
                {
                    StartCoroutine(Upper());
                }*/
            }

            //nextFire�� ����ð����� ����, �÷��̾���� �Ÿ��� 7������ �� �� �߻�.
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

    void Flip()  //�¿� ���� ��ȯ �޼���
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

    IEnumerator Fire()  //�� �߻� �޼���
    {
        anim.ResetTrigger("Fire");
        //�� ����.
        var fire = Instantiate(firePrefab, fireSpawn.position, fireSpawn.rotation);
        GameManager.instance.fireObjects.Add(fire);
        fire.GetComponent<BoarsFire>().SetDirection(facingRight);
        //nextFire�� ����ð� + ��Ÿ���� ����.
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
