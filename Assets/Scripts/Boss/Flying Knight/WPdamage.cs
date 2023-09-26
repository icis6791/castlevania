using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WPdamage : MonoBehaviour
{
    public int damage = 35;

    private void OnTriggerEnter2D(Collider2D other)
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
