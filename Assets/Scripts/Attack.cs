using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    Animator anim;
    public int damage = 20;  //�÷��̾� ������
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
        //�� ������Ʈ���� �������� �޴� �޼��� ȣ��
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}
