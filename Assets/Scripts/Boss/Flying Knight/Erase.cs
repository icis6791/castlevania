using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erase : MonoBehaviour
{
    //���� ������ġ
    public Transform spawnA;
    public Transform spawnB;

    private void OnTriggerExit2D(Collider2D other)
    {
        //���Ⱑ Ʈ���ſ��� �������� ��ġ�� ������ġ ��ġ�� ȸ������ �ٲ�.
        if(other.CompareTag("BOSSWP_A"))
        {
            //print("�浹 ����");
            other.transform.position = spawnA.position;
            other.transform.rotation = spawnA.rotation;
        }
        if(other.CompareTag("BOSSWP_B"))
        {
            //print("�浹 ����");
            other.transform.position = spawnB.position;
            other.transform.rotation = spawnB.rotation;
        }
    }
}
