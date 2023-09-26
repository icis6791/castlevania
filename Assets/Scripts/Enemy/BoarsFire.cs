using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarsFire : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sprite;
    int damage = 40;  //데미지
    Vector3 direction;

    [SerializeField]
    float speed = 8f;  //속도

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        //몬스터가 보는 방향에 따라 좌우를 반전하고, 방향에 맞게 날리기.
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
        //플레이어에게 데미지 주기.
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
            //플레이어 공격에 닿으면 삭제.
            Destroy(gameObject);
        }

        /*//방을 벗어나면 삭제하기
        if (other.gameObject.CompareTag("ROOM"))
        {
            Destroy(gameObject);
        }*/
    }
}
