using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raven : MonoBehaviour
{
    SpriteRenderer sprite;
    public float speed;
    public int damage = 30;
    Karas karas;

    private void Awake()
    {
        karas = GameObject.FindWithTag("KARAS").GetComponent<Karas>();
    }

    void OnEnable()
    {
        speed = Random.Range(5, 8.5f);
        sprite = GetComponent<SpriteRenderer>();
        if(!karas.facingRight)
        {
            sprite.flipX = false;
            StartCoroutine(FireLeft());
        }
        else
        {
            sprite.flipX = true;
            StartCoroutine(FireRight());
        }
        Invoke("Disable", 3f);
    }

    IEnumerator FireRight()
    {
        while(this.gameObject.activeSelf)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator FireLeft()
    {
        while(this.gameObject.activeSelf)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            yield return null;
        }
    }

    private void Update()
    {
        if(!karas.facingRight)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }

    private void Disable()
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //플레이어에게 데미지 주기.
        if (other.gameObject.CompareTag("PLAYER"))
        {
            var player = other.gameObject.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Disable();
            }
        }
    }
}
