using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sprite;
    int damage = 20;  //������
    Vector3 direction;

    public float speed = 9.5f;  //�ӵ�

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        //���Ͱ� ���� ���⿡ ���� �¿츦 �����ϰ�, ���⿡ �°� ������.
        /*if (GameObject.Find("Archer(Clone)").GetComponent<Archer>().facingRight == false)
        {
            sprite.flipX = false;
            GetComponent<Rigidbody2D>().AddRelativeForce(Vector3.left * speed);
        }
        else
        {
            sprite.flipX = true;
            GetComponent<Rigidbody2D>().AddRelativeForce(Vector3.right * speed);
        }
        Destroy(gameObject, 2.4f);*/
    }

    public void SetDirection(bool facingRight)
    {
        if (!facingRight)
        {
            sprite.flipX = false;
            direction = Vector3.left;
        }
        else
        {
            sprite.flipX = true;
            direction = Vector3.right;
        }
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //�÷��̾�� ������ �ֱ�
        if (other.gameObject.CompareTag("PLAYER"))
        {
            Destroy(gameObject);
            var player = other.gameObject.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }

        if (other.gameObject.CompareTag("ATTACK"))
        {
            //�÷��̾� ���ݿ� ������ ����.
            Destroy(gameObject);
        }

        /*//���� ����� �����ϱ�
        if (other.gameObject.CompareTag("ROOM"))
        {
            Destroy(gameObject);
        }*/
    }
}
