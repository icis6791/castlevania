using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_B : MonoBehaviour
{
    public float speed;
    public int damage = 20;

    void OnEnable()
    {
        speed = Random.Range(10, 20);

        Invoke("Disable", 2f);
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void Disable()
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //�÷��̾�� ������ �ֱ�.
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
