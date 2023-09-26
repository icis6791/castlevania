using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarsFire : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sprite;
    int damage = 40;  //������
    Vector3 direction;

    [SerializeField]
    float speed = 8f;  //�ӵ�

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        //���Ͱ� ���� ���⿡ ���� �¿츦 �����ϰ�, ���⿡ �°� ������.
        //sprite = GetComponent<SpriteRenderer>();
        /*if (GameObject.Find("Boar(Clone)").GetComponent<Boar>().facingRight == false)
        {
            sprite.flipX = true;
            GetComponent<Rigidbody2D>().AddRelativeForce(Vector3.left * speed);
        }
        else
        {
            sprite.flipX = false;
            GetComponent<Rigidbody2D>().AddRelativeForce(Vector3.right * speed);
        }*/
        //Destroy(gameObject, 2.4f);
    }

    public void SetDirection(bool facingRight)
    {
        if(!facingRight)
        {
            sprite.flipX = true;
            direction = Vector3.left;
        }
        else
        {
            direction = Vector3.right;
        }
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //�÷��̾�� ������ �ֱ�.
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