using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pirate : MonoBehaviour, Enemy
{
    Transform player;  //�÷��̾�
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;
    AudioSource audioSource;
    public AudioClip damageClip;
    Vector3 playerDistance;  //�÷��̾���� �Ÿ�
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
            /*//���� �տ� �ٴ��� ���ϴ� Ray�� ���� �������� ���� �ƴ��� �Ǵ�.
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
                //(playerDistance.x / Mathf.Abs(playerDistance.x) �� �κ��� playerDistance�� ��ȣ�� �������� ������.
                //�� ��ȣ�� speed�� ����. ���� ���� ����, ������ ������ ���� ��
                rb.velocity = new Vector2(speed * (playerDistance.x / Mathf.Abs(playerDistance.x)), rb.velocity.y);
                if (Mathf.Abs(playerDistance.x) < 2.5f && t > nextAttack)
                {
                    StartCoroutine(Attack());
                }
            }
            //�ӵ��� �ִϸ��̼� ���
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


    void Flip() //�¿� ȸ�� �޼���
    {
        if (!isAttack)
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    IEnumerator Attack()  //���� �޼���
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

    public void TakeDamage(int damage)  //���ݹ޴� �޼���
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(damageClip);  //�ǰ� ����
        }
        health -= damage;
        StartCoroutine(DamageCoroutine());
        if (health <= 0)
        {
            //����ִϸ��̼� ��� �� �ݶ��̴� + ��������Ʈ �����
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            GetComponent<CapsuleCollider2D>().enabled = false;
            anim.SetTrigger("Die");
            isDead = true;
            StartCoroutine(Vanish());
        }
    }

    IEnumerator DamageCoroutine()  //���ݹ��� �� �����Ÿ��� ȿ��
    {
        sprite.color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(0.15f);
        sprite.color = new Color(1, 1, 1, 1);
    }

    IEnumerator Vanish()  //������ ������� ȿ��
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

    private void OnTriggerEnter2D(Collider2D other)  //�� ���� �¾��� ��
    {
        //�浹ü �±װ� PLAYER�̸� ��ũ��Ʈ�� �����ͼ� ��ũ��Ʈ �ȿ� �ִ� ������ �޴� �Լ� ȣ��.
        if (other.gameObject.CompareTag("PLAYER"))
        {
            var player = other.gameObject.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)  //���뿡 ����� ��
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
