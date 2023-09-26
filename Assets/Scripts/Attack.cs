using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    Animator anim;
    public int damage = 20;  //플레이어 데미지
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayAnimation()
    {
        anim.SetTrigger("Attack");
    }

    public void ForcePlus(int plus)
    {
        damage += plus;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //적 오브젝트에서 데미지를 받는 메서드 호출
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}
